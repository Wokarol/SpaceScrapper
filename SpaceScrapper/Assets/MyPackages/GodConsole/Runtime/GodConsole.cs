using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;
using static Wokarol.GodConsole.Command;

namespace Wokarol.GodConsole
{
    public class GodConsole : MonoBehaviour
    {
        [SerializeField] private bool includeDefaultAssembly = false;
        [SerializeField] private List<string> assembliesToInclude = new();

        private IServiceProvider serviceProvider;
        private readonly List<Command> commands = new();
        private CommandNode root = new();

        private void Start()
        {
            TryGetComponent(out serviceProvider);

            foreach (var injector in GetComponentsInChildren<IInjector>())
            {
                InjectFrom(injector);
            }

            AddStaticCommands();

            Debug.Log($"<color=cyan>God Console:</color> Registered {commands.Count} commands", this);
        }

        public void InjectFrom(IInjector injector)
        {
            injector.Inject(new CommandBuilder(this));
        }

        public void Execute(string input)
        {
            string[] segments = SegmentInput(input);

            var node = root;
            for (int i = 0; i < segments.Length; i++)
            {
                string segment = segments[i];
                if (node.TryGet(segment, out var command))
                {
                    node = command;
                }
                else break;

                if (TryToExecuteCommand(node.SelfCommands, segments.AsSpan((i + 1)..^0)))
                {
                    return;
                }
            }

            Debug.LogWarning("There is no method matching");
        }

        public void FindSuggestions(string input, ref List<string> results)
        {
            results.Clear();
            string[] segments = SegmentInput(input);

            var node = root;
            for (int i = 0; i < segments.Length - 1; i++)
            {
                string segment = segments[i];
                if (node.TryGet(segment, out var innerCommandNode))
                {
                    node = innerCommandNode;
                }
                else return;
            }


            if (node.TryGet(segments[^1], out var foundCommandNode))
            {

                foreach (var key in foundCommandNode.GetAllBeginningWith(""))
                {
                    results.Add($"{string.Join(' ', segments)} {key}");
                }
            }
            else
            {
                string leftover = segments[^1];
                foreach (var key in node.GetAllBeginningWith(leftover))
                {
                    if (segments.Length <= 1)
                    {
                        results.Add($"{key}");
                    }
                    else
                    {
                        results.Add($"{string.Join(' ', segments[0..^1])} {key}");
                    }
                }
            }
        }

        internal Command FindClosestFittingCommand(string input, out int usedSegments, out int allSegments)
        {
            string[] segments = SegmentInput(input);
            allSegments = segments.Length;

            Command command = null;
            usedSegments = -1;

            var node = root;
            for (int i = 0; i < segments.Length; i++)
            {
                string segment = segments[i];
                if (node.TryGet(segment, out var innerCommandNode))
                {
                    node = innerCommandNode;

                    if (node.SelfCommands.Count > 0)
                    {
                        command = node.SelfCommands[0];
                        usedSegments = i + 1;
                    }
                }
                else break;
            }

            return command;
        }

        private void AddStaticCommands()
        {
            if (assembliesToInclude.Count == 0 && !includeDefaultAssembly)
                return;

            HashSet<string> assemblyNames = new(assembliesToInclude);

            if (includeDefaultAssembly)
            {
                assemblyNames.Add("Assembly-CSharp");
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => assemblyNames.Contains(a.GetName().Name));

            Debug.Log($"<color=cyan>God Console:</color> Found {assemblies.Count()} matching assemblies");

            var methods = assemblies
                .SelectMany(a => a.GetTypes())
                .SelectMany(a => a.GetMethods(BindingFlags.Static | BindingFlags.Public))
                .Select(a => (attribute: a.GetCustomAttribute<CommandAttribute>(), method: a))
                .Where(a => a.attribute != null);

            foreach (var (attribute, method) in methods)
            {
                var routeAttributes = method.DeclaringType.GetCustomAttributes<CommandRouteAttribute>();
                string absolutePath = $"{string.Join(" ", routeAttributes.Select(a => a.Route).Reverse())} {attribute.Route}".Trim();

                List<Command.Argument> arguments = new();
                foreach (var arg in method.GetParameters())
                {
                    var type = arg.ParameterType;
                    bool isSimple = type.IsPrimitive || type == typeof(string);

                    arguments.Add(new(type, arg.Name ?? "-", isSimple));
                }

                Add(new Command(absolutePath, arguments, CreateStaticDelegate(method)));
            }


            static Delegate CreateStaticDelegate(MethodInfo methodInfo)
            {
                var parmTypes = methodInfo.GetParameters().Select(parm => parm.ParameterType);
                var parmAndReturnTypes = parmTypes.Append(methodInfo.ReturnType).ToArray();
                var delegateType = Expression.GetDelegateType(parmAndReturnTypes);

                return methodInfo.CreateDelegate(delegateType);
            }
        }

        private void Add(Command command)
        {
            commands.Add(command);

            var node = root;
            string[] path = SegmentInput(command.AbsolutePath);
            foreach (var segment in path)
            {
                node = node.GetOrCreate(segment);
            }

            node.SelfCommands.Add(command);
        }

        private string[] SegmentInput(string path)
        {
            return Regex.Replace(path, @"\s+", " ").Trim().Split(' ');
        }

        private bool TryToExecuteCommand(List<Command> selfCommands, Span<string> passedArguments)
        {
            foreach (var selfCommand in selfCommands)
            {
                var outcome = TryToFillCommandArguments(selfCommand, passedArguments, out var invokeParameters);

                if (outcome == ParseOutcome.Ok)
                {
                    selfCommand.Action.DynamicInvoke(invokeParameters);
                    return true;
                }
                if (outcome == ParseOutcome.Unsupported)
                {
                    return true;
                }
            }
            return false;
        }

        private ParseOutcome TryToParseSimpleArgument(Type type, string source, out object result)
        {
            result = null;

            if (type == typeof(string))
            {
                result = source;
            }
            else if (type == typeof(int))
            {
                if (int.TryParse(source, out var number))
                    result = number;
                else
                    return ParseOutcome.FailedToParse;
            }
            else if (type == typeof(float))
            {
                if (float.TryParse(source, out var number))
                    result = number;
                else
                    return ParseOutcome.FailedToParse;
            }
            else if (type == typeof(bool))
            {
                switch (source.ToLower())
                {
                    case "true":
                    case "t":
                    case "1":
                    case "yes":
                    case "y":
                    case "on":
                    case "en":
                    case "enable":
                        result = true;
                        break;

                    case "false":
                    case "f":
                    case "0":
                    case "no":
                    case "n":
                    case "off":
                    case "dis":
                    case "disable":
                        result = false;
                        break;

                    default:
                        return ParseOutcome.FailedToParse;
                }
            }
            else
            {
                return ParseOutcome.Unsupported;
            }

            return ParseOutcome.Ok;
        }

        private ParseOutcome TryToFillCommandArguments(Command selfCommand, Span<string> passedArguments, out object[] invokeParameters)
        {
            invokeParameters = new object[selfCommand.Arguments.Count];

            int passedArgumentIndex = 0; // Index for passed arguments is kept separate to skip injected services
            for (int j = 0; j < selfCommand.Arguments.Count; j++)
            {
                var argumentData = selfCommand.Arguments[j];

                if (argumentData.IsSimple)
                {
                    if (passedArgumentIndex >= passedArguments.Length)
                    {
                        return ParseOutcome.FailedToParse;
                    }

                    string argument = passedArguments[passedArgumentIndex];
                    passedArgumentIndex++;

                    var outcome = TryToParseSimpleArgument(argumentData.Type, argument, out invokeParameters[j]);

                    if (outcome == ParseOutcome.FailedToParse)
                    {
                        return ParseOutcome.FailedToParse;
                    }

                    if (outcome == ParseOutcome.Unsupported)
                    {
                        Debug.LogError($"Parameter <{argumentData.Name} : {argumentData.Type.Name}> cannot be parsed as the type is not supported. Thrown while filling arguments for {selfCommand.AbsolutePath}");
                        return ParseOutcome.Unsupported;
                    }
                }
                else
                {
                    if (serviceProvider == null)
                    {
                        Debug.LogError($"Parameter <{argumentData.Name} : {argumentData.Type.Name}> cannot be parsed as there is no {nameof(IServiceProvider)}. Thrown while filling arguments for {selfCommand.AbsolutePath}");
                        return ParseOutcome.Unsupported;
                    }
                    invokeParameters[j] = serviceProvider.Get(argumentData.Type);
                }
            }

            if (passedArgumentIndex != passedArguments.Length)
            {
                return ParseOutcome.FailedToParse;
            }

            return ParseOutcome.Ok;
        }


        private enum ParseOutcome
        {
            Ok, FailedToParse, Unsupported
        }

        public readonly struct CommandBuilder
        {
            private readonly GodConsole console;
            private readonly string path;

            public CommandBuilder(GodConsole console, string path = "")
            {
                this.console = console;
                this.path = path;
            }

            public CommandBuilder Add(string route, Action action) => Add(route, (Delegate)action);
            public CommandBuilder Add<T>(string route, Action<T> action) => Add(route, (Delegate)action);
            public CommandBuilder Add<T1, T2>(string route, Action<T1, T2> action) => Add(route, (Delegate)action);
            public CommandBuilder Add<T1, T2, T3>(string route, Action<T1, T2, T3> action) => Add(route, (Delegate)action);
            public CommandBuilder Add<T1, T2, T3, T4>(string route, Action<T1, T2, T3, T4> action) => Add(route, (Delegate)action);
            public CommandBuilder Add<T1, T2, T3, T4, T5>(string route, Action<T1, T2, T3, T4, T5> action) => Add(route, (Delegate)action);
            public CommandBuilder Add<T1, T2, T3, T4, T5, T6>(string route, Action<T1, T2, T3, T4, T5, T6> action) => Add(route, (Delegate)action);
            public CommandBuilder Add<T1, T2, T3, T4, T5, T6, T7>(string route, Action<T1, T2, T3, T4, T5, T6, T7> action) => Add(route, (Delegate)action);
            public CommandBuilder Add<T1, T2, T3, T4, T5, T6, T7, T8>(string route, Action<T1, T2, T3, T4, T5, T6, T7, T8> action) => Add(route, (Delegate)action);
            public CommandBuilder Add<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string route, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action) => Add(route, (Delegate)action);
            public CommandBuilder Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string route, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action) => Add(route, (Delegate)action);
            public CommandBuilder Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string route, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action) => Add(route, (Delegate)action);
            public CommandBuilder Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string route, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action) => Add(route, (Delegate)action);

            public CommandBuilder Add(string route, Delegate action)
            {
                string absolutePath = $"{path} {route}".Trim();

                List<Command.Argument> arguments = new();
                foreach (var arg in action.GetMethodInfo().GetParameters())
                {
                    var type = arg.ParameterType;
                    bool isSimple = type.IsPrimitive || type == typeof(string);

                    arguments.Add(new(type, arg.Name ?? "-", isSimple));
                }

                console.Add(new Command(absolutePath, arguments, action));
                return this;
            }

            public CommandBuilder Group(string route)
            {
                if (string.IsNullOrEmpty(path))
                    return new CommandBuilder(console, route);
                else
                    return new CommandBuilder(console, $"{path} {route}");
            }
        }
    }
}
