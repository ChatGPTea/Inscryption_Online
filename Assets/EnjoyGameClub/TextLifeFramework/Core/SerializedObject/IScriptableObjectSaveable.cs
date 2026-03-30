/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

namespace EnjoyGameClub.TextLifeFramework.Core.SerializedObject
{
    public interface IScriptableObjectSaveable
    {
        public object Save(bool isRootAsset = false, object rootObject = null, string rootPath = "");
    }

    public interface IScriptableObjectCloneable
    {
        public object Clone();
    }
}