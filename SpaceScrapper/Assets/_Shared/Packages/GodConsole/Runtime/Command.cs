using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Wokarol.GodConsole
{
    internal class Command
    {
        public readonly string AbsolutePath;
        public readonly List<Argument> Arguments;
        public readonly Delegate Action;

        public Command(string absolutePath, List<Argument> arguments, Delegate action)
        {
            Arguments = arguments;
            Action = action;
            AbsolutePath = absolutePath;
        }

        internal readonly struct Argument
        {
            public readonly Type Type;
            public readonly string Name;
            public readonly bool IsSimple;

            public Argument(Type type, string name, bool isSimple)
            {
                Type = type;
                Name = name;
                IsSimple = isSimple;
            }
        }
    }

    internal class CommandNode
    {
        private Dictionary<string, CommandNode> childCommands;

        public readonly List<Command> SelfCommands = new();

        public CommandNode GetOrCreate(string key)
        {
            childCommands ??= new();

            if (childCommands.TryGetValue(key, out var node))
                return node;

            node = new CommandNode();
            childCommands.Add(key, node);
            return node;
        }

        public bool TryGet(string key, [NotNullWhen(true)] out CommandNode node)
        {
            node = null;
            if (childCommands == null)
                return false;

            return childCommands.TryGetValue(key, out node);
        }

        public IEnumerable<string> GetAllBeginningWith(string input)
        {
            if (childCommands == null)
                yield break;

            foreach (var kvp in childCommands)
            {
                if (kvp.Key.StartsWith(input))
                yield return kvp.Key;
            }
        }
    }
}
