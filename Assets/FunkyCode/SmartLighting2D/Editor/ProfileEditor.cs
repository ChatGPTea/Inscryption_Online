using FunkyCode.LightingSettings;
using UnityEditor;

namespace FunkyCode
{
    [CustomEditor(typeof(Profile))]
    public class ProfileEditor2 : Editor
    {
        public override void OnInspectorGUI()
        {
            var profile = target as Profile;

            ProfileEditor.DrawProfile(profile);
        }
    }
}