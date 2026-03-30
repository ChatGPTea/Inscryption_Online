using System.Collections.Generic;
using MoonSharp.Interpreter.Debugging;

namespace MoonSharp.Interpreter.Execution.VM
{
    internal sealed partial class Processor
    {
        private class DebugContext
        {
            public readonly List<SourceRef> BreakPoints = new();
            public IDebugger DebuggerAttached;
            public DebuggerAction.ActionType DebuggerCurrentAction = DebuggerAction.ActionType.None;
            public int DebuggerCurrentActionTarget = -1;
            public bool DebuggerEnabled = true;
            public int ExStackDepthAtStep = -1;
            public SourceRef LastHlRef;
            public bool LineBasedBreakPoints;
        }
    }
}