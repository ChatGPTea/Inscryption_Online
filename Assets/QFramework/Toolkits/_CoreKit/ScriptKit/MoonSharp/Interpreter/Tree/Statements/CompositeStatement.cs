using System.Collections.Generic;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Statements
{
    internal class CompositeStatement : Statement
    {
        private readonly List<Statement> m_Statements = new();

        public CompositeStatement(ScriptLoadingContext lcontext)
            : base(lcontext)
        {
            while (true)
            {
                var t = lcontext.Lexer.Current;
                if (t.IsEndOfBlock()) break;

                bool forceLast;

                var s = CreateStatement(lcontext, out forceLast);
                m_Statements.Add(s);

                if (forceLast) break;
            }

            // eat away all superfluos ';'s
            while (lcontext.Lexer.Current.Type == TokenType.SemiColon)
                lcontext.Lexer.Next();
        }


        public override void Compile(ByteCode bc)
        {
            if (m_Statements != null)
                foreach (var s in m_Statements)
                    s.Compile(bc);
        }
    }
}