/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;

namespace QFramework
{
    public abstract class CodeScope : ICodeScope
    {
        public bool Semicolon { get; set; }

        public void Gen(ICodeWriter writer)
        {
            GenFirstLine(writer);

            new OpenBraceCode().Gen(writer);

            writer.IndentCount++;

            foreach (var code in Codes) code.Gen(writer);

            writer.IndentCount--;

            new CloseBraceCode(Semicolon).Gen(writer);
        }

        public List<ICode> Codes { get; set; } = new();

        protected abstract void GenFirstLine(ICodeWriter codeWriter);
    }
}