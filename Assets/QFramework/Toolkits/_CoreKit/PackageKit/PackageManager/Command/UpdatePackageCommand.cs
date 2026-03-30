/****************************************************************************
 * Copyright (c) 2015 - 2023 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;

namespace QFramework
{
    internal class UpdatePackageCommand : AbstractCommand
    {
        private readonly PackageVersion mInstalledVersion;

        private readonly PackageRepository mPackageRepository;

        public UpdatePackageCommand(PackageRepository packageRepository, PackageVersion installedVersion)
        {
            mPackageRepository = packageRepository;
            mInstalledVersion = installedVersion;
        }

        protected override void OnExecute()
        {
            RenderEndCommandExecutor.PushCommand(() =>
            {
                AssetDatabase.Refresh();

                EditorWindow.GetWindow<PackageKitWindow>().Close();

                this.SendCommand(new InstallPackageCommand(mPackageRepository,
                    () => { PackageHelper.DeletePackageFiles(mInstalledVersion); }));
            });
        }
    }
}
#endif