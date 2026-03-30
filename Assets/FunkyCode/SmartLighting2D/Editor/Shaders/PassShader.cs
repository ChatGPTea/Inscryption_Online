using UnityEditor;
using UnityEngine;

namespace FunkyCode
{
    public class PassShader : ShaderGUI
    {
        public enum EnumPass
        {
            None,
            Pass1,
            Pass2,
            Pass3,
            Pass4,
            Pass5,
            Pass6,
            Pass7,
            Pass8
        }

        private readonly string[] PassPopup =
            { "None", "Pass 1", "Pass 2", "Pass 3", "Pass 4", "Pass 5", "Pass 6", "Pass 7", "Pass 8" };

        private bool IsPassEnabled(Material material, int passId)
        {
            return material.IsKeywordEnabled("SL2D_PASS_" + passId);
        }

        private bool IsActive(Material material)
        {
            for (var i = 0; i <= 8; i++)
                if (IsPassEnabled(material, i))
                    return true;

            return false;
        }

        private void SetPass(Material material, int passId)
        {
            material.SetInt("_PassId", passId);

            for (var i = 0; i <= 8; i++)
            {
                var passName = "SL2D_PASS_" + i;

                if (i == passId)
                    material.EnableKeyword(passName);
                else
                    material.DisableKeyword(passName);
            }
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            var material = materialEditor.target as Material;

            if (!IsActive(material))
            {
                Debug.Log("is inactive");

                SetPass(material, 1);
            }
            else
            {
                for (var i = 1; i <= 8; i++)
                {
                    var preset = LightmapShaders.ActivePassLightmaps[i];

                    var name = preset != null ? " (" + preset.name + ")" : "";
                    var passName = "Pass " + i + name;

                    PassPopup[i] = passName;
                }

                var passId = material.GetInt("_PassId");

                var newPassId = EditorGUILayout.Popup("Pass", passId, PassPopup);

                if (newPassId != passId) SetPass(material, newPassId);
            }

            base.OnGUI(materialEditor, properties);
        }
    }
}