/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    public interface IBind
    {
        string TypeName { get; }

        string Comment { get; }

        Transform Transform { get; }
    }

    public interface IBindGroup
    {
        string TemplateName { get; }
    }
}