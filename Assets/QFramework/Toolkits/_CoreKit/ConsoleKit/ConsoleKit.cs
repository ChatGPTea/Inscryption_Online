using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ConsoleKit
    {
        private static readonly LogModule mDefaultLogModule = new();
        private static readonly FrameworkModule mDefaultFrameworkModule = new();

        private static readonly List<ConsoleModule> mModules = new()
        {
            mDefaultLogModule,
            mDefaultFrameworkModule
        };

        public static IReadOnlyList<ConsoleModule> Modules => mModules;

        public static void AddModule(ConsoleModule module)
        {
            mModules.Add(module);
        }

        public static void RemoveAllModules()
        {
            mModules.RemoveAll(m => m != mDefaultLogModule && m != mDefaultFrameworkModule);
        }

        public static void CreateWindow()
        {
            new GameObject("ConsoleKit")
                .AddComponent<ConsoleWindow>();
        }

        public static int GetDefaultIndex()
        {
            var index = mModules.FindIndex(m => m.Default);

            if (index == -1) index = 0;

            return index;
        }
    }
}