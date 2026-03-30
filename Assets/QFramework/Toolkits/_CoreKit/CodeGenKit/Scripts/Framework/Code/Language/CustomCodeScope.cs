/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    public class CustomCodeScope : CodeScope
    {
        public CustomCodeScope(string firstLine)
        {
            mFirstLine = firstLine;
        }

        private string mFirstLine { get; }

        protected override void GenFirstLine(ICodeWriter codeWriter)
        {
            codeWriter.WriteLine(mFirstLine);
        }
    }

    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope CustomScope(this ICodeScope self, string firstLine, bool semicolon,
            Action<CustomCodeScope> customCodeScopeSetting)
        {
            var custom = new CustomCodeScope(firstLine);
            custom.Semicolon = semicolon;
            customCodeScopeSetting(custom);
            self.Codes.Add(custom);
            return self;
        }
    }
}