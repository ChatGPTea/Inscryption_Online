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
    public class RootCode : ICodeScope
    {
        public List<ICode> Codes { get; set; } = new();


        public void Gen(ICodeWriter writer)
        {
            foreach (var code in Codes) code.Gen(writer);
        }
    }
}