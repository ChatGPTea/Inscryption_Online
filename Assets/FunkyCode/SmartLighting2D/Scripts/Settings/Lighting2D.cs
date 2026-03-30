using FunkyCode.LightingSettings;
using UnityEngine;
using QualitySettings = FunkyCode.LightingSettings.QualitySettings;

namespace FunkyCode
{
    public static class Lighting2D
    {
        public const int VERSION = 20221100;
        public const string VERSION_STRING = "2022.11.0";

        public static Lighting2DMaterials Materials = new();

        // profile
        private static Profile profile;

        private static ProjectSettings projectSettings;

        public static bool Disable => false;

        // lightmaps
        public static LightmapPreset[] LightmapPresets => Profile.lightmapPresets.list;

        // quality
        public static QualitySettings QualitySettings => Profile.qualitySettings;

        // day lighting
        public static DayLightingSettings DayLightingSettings => Profile.dayLightingSettings;

        public static RenderingMode RenderingMode => ProjectSettings.renderingMode;

        public static CoreAxis CoreAxis => Profile.qualitySettings.coreAxis;

        // set & get
        public static Color DarknessColor
        {
            set => LightmapPresets[0].darknessColor = value;
            get => LightmapPresets[0].darknessColor;
        }

        public static float Resolution
        {
            set => LightmapPresets[0].resolution = value;
            get => LightmapPresets[0].resolution;
        }

        public static Profile Profile
        {
            get
            {
                if (profile != null) return profile;

                if (ProjectSettings != null) profile = ProjectSettings.Profile;

                if (profile == null)
                {
                    profile = Resources.Load("Profiles/Default Profile") as Profile;

                    if (profile == null) Debug.LogError("Light 2D: Default Profile not found");
                }

                return profile;
            }
        }

        public static ProjectSettings ProjectSettings
        {
            get
            {
                if (projectSettings != null) return projectSettings;

                projectSettings = Resources.Load("Settings/Project Settings") as ProjectSettings;

                return projectSettings;
            }
        }

        // methods
        public static void UpdateByProfile(Profile setProfile)
        {
            if (setProfile == null)
            {
                Debug.Log("Light 2D: Update Profile is Missing");
                return;
            }

            // set profile also
            profile = setProfile;
        }

        public static void RemoveProfile()
        {
            profile = null;
        }
    }
}