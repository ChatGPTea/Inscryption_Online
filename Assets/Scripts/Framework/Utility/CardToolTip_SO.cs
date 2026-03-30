using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inscryption
{
    [CreateAssetMenu(fileName = "CardToolTiopSO", menuName = "Card/CardToolTipSO")]
    public class CardToolTip_SO : ScriptableObject
    {
        public List<AbilityTip> abilityTips;

        public AbilityTip GetCardAbilityTip(Abilities abilities)
        {
            return abilityTips.Find(i => i.abilities == abilities);
        }
    }
}