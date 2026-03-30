using MoonSharp.Interpreter.Compatibility.Frameworks;

namespace MoonSharp.Interpreter.Compatibility
{
    public static class Framework
    {
        private static readonly FrameworkCurrent s_FrameworkCurrent = new();

        public static FrameworkBase Do => s_FrameworkCurrent;
    }
}