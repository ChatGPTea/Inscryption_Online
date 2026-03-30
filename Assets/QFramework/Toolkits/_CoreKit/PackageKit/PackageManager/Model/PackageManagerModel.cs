#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace QFramework
{
    internal interface IPackageManagerModel : IModel
    {
        List<PackageRepository> Repositories { get; set; }
    }

    internal class PackageManagerModel : AbstractModel, IPackageManagerModel
    {
        public PackageManagerModel()
        {
            Repositories = PackageInfosRequestCache.Get().PackageRepositories;
        }

        public bool VersionCheck
        {
            get => EditorPrefs.GetBool("QFRAMEWORK_VERSION_CHECK", true);
            set => EditorPrefs.SetBool("QFRAMEWORK_VERSION_CHECK", value);
        }

        public List<PackageRepository> Repositories { get; set; }

        protected override void OnInit()
        {
        }
    }
}
#endif