using System.Collections.Generic;
using UnityEngine;

namespace Inscryption
{
    [CreateAssetMenu(fileName = "CardInfo", menuName = "Card/CardInfo")]
    public class CardInfo : ScriptableObject
    {
        public int CardID;
        public string Name;
        public int Health;
        public int Attack;
        public int Cost;
        public CostType CostType;
        public Sprite BackGround;
        public Sprite Face;
        public Sprite Face_Add;

        [Header("种族配置")] public List<CardType> CardTypes = new();
        public List<Race> Races = new();

        [Header("能力配置")] public List<Abilities> Abilities = new();
        public List<Sprite> Abilities_Sprite = new();

        [Header("是否有附加血迹/污渍")] public bool HasDecal;
        public Sprite Decal;

        [Header("可选项")] public bool isObstacle;
        public bool dontGenerateInLibrary;
    }
}