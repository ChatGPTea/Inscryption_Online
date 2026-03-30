using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Inscryption
{
    [CustomEditor(typeof(CardToolTip_SO))]
    public class CardAbilityLibHelper : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CardToolTip_SO toolTipSO = (CardToolTip_SO)target;

            if (GUILayout.Button("自动补全印记信息"))
            {
                FillMissingAbilities(toolTipSO);
                EditorUtility.SetDirty(toolTipSO);
                AssetDatabase.SaveAssets();
            }
        }

        private void FillMissingAbilities(CardToolTip_SO toolTipSO)
        {
            Abilities[] allAbilities = System.Enum.GetValues(typeof(Abilities)).Cast<Abilities>().ToArray();
        
            foreach (Abilities ability in allAbilities)
            {
                if (ability == Abilities.None) continue; 
            
                bool exists = toolTipSO.abilityTips.Exists(tip => tip.abilities == ability);

                if (!exists)
                {
                    AbilityTip newTip = new AbilityTip
                    {
                        abilities = ability,
                        abliltyName = "", 
                        description = ""  
                    };

                    toolTipSO.abilityTips.Add(newTip);
                    Debug.Log($"自动补全印记信息: {ability}");
                }
            }
        
            toolTipSO.abilityTips = toolTipSO.abilityTips
                .OrderBy(tip => (int)tip.abilities)
                .ToList();
        }
    }
}
