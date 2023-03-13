using System;

namespace Wokarol.GodConsole
{
    public interface IServiceProvider
    {
        object Get(Type type);
    }

    public interface IInjector
    {
        void Inject(GodConsole.CommandBuilder b);
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string route)
        {
            Route = route;
        }

        public string Route { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
    public class CommandRouteAttribute : Attribute
    {
        public CommandRouteAttribute(string route)
        {
            Route = route;
        }

        public string Route { get; set; }
    }
}
