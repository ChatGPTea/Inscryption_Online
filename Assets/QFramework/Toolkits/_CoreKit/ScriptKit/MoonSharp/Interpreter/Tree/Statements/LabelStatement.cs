using System.Collections.Generic;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Statements
{
    internal class LabelStatement : Statement
    {
        private readonly List<GotoStatement> m_Gotos = new();
        private RuntimeScopeBlock m_StackFrame;


        public LabelStatement(ScriptLoadingContext lcontext)
            : base(lcontext)
        {
            CheckTokenType(lcontext, TokenType.DoubleColon);
            NameToken = CheckTokenType(lcontext, TokenType.Name);
            CheckTokenType(lcontext, TokenType.DoubleColon);

            SourceRef = NameToken.GetSourceRef();
            Label = NameToken.Text;

            lcontext.Scope.DefineLabel(this);
        }

        public string Label { get; private set; }
        public int Address { get; private set; }
        public SourceRef SourceRef { get; private set; }
        public Token NameToken { get; }

        internal int DefinedVarsCount { get; private set; }
        internal string LastDefinedVarName { get; private set; }

        internal void SetDefinedVars(int definedVarsCount, string lastDefinedVarsName)
        {
            DefinedVarsCount = definedVarsCount;
            LastDefinedVarName = lastDefinedVarsName;
        }

        internal void RegisterGoto(GotoStatement gotostat)
        {
            m_Gotos.Add(gotostat);
        }


        public override void Compile(ByteCode bc)
        {
            bc.Emit_Clean(m_StackFrame);

            Address = bc.GetJumpPointForLastInstruction();

            foreach (var gotostat in m_Gotos)
                gotostat.SetAddress(Address);
        }

        internal void SetScope(RuntimeScopeBlock runtimeScopeBlock)
        {
            m_StackFrame = runtimeScopeBlock;
        }
    }
}