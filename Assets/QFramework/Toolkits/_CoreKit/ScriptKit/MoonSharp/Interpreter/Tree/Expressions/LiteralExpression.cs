using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Expressions
{
    internal class LiteralExpression : Expression
    {
        public LiteralExpression(ScriptLoadingContext lcontext, DynValue value)
            : base(lcontext)
        {
            Value = value;
        }


        public LiteralExpression(ScriptLoadingContext lcontext, Token t)
            : base(lcontext)
        {
            switch (t.Type)
            {
                case TokenType.Number:
                case TokenType.Number_Hex:
                case TokenType.Number_HexFloat:
                    Value = DynValue.NewNumber(t.GetNumberValue()).AsReadOnly();
                    break;
                case TokenType.String:
                case TokenType.String_Long:
                    Value = DynValue.NewString(t.Text).AsReadOnly();
                    break;
                case TokenType.True:
                    Value = DynValue.True;
                    break;
                case TokenType.False:
                    Value = DynValue.False;
                    break;
                case TokenType.Nil:
                    Value = DynValue.Nil;
                    break;
                default:
                    throw new InternalErrorException("type mismatch");
            }

            if (Value == null)
                throw new SyntaxErrorException(t, "unknown literal format near '{0}'", t.Text);

            lcontext.Lexer.Next();
        }

        public DynValue Value { get; }

        public override void Compile(ByteCode bc)
        {
            bc.Emit_Literal(Value);
        }

        public override DynValue Eval(ScriptExecutionContext context)
        {
            return Value;
        }
    }
}