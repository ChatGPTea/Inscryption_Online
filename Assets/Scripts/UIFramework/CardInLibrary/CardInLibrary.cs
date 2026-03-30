using System;
using System.Collections;
using DG.Tweening;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using System.Text.RegularExpressions;

namespace Inscryption
{
    public class CardInLibrary : InscryptionController
    {
        //能力组件
        private Transform mAbility_1;
        private Transform mAbility_2;
        private Transform mAbility_3;

        private TMP_Text mAttackText;
        private TMP_Text mHPText;
        private TMP_Text mNameText;
        private TMP_Text mAbilityText;

        private Image Ability_1_1;
        private Image Ability_2_1;
        private Image Ability_2_2;
        private Image Ability_3_1;
        private Image mCounter;

        private Image mFace;
        private CardInfo mCurrentCardInfo;

        private int mCurrentCardID;
        private CardInHandData mData;
        private CountInfo mCountInfo;

        private Button mButton;

        //长按相关
        private bool longPressTriggered = false; // 新增标志位
        private float longPressDuration = 0.65f; // 长按时间阈值
        private bool isLongPress = false;
        private float pressTime = 0;

        private void Awake()
        {
            mAttackText = transform.Find("AttackText").GetComponent<TMP_Text>();
            mHPText = transform.Find("HealthText").GetComponent<TMP_Text>();
            mNameText = transform.Find("Name").GetComponent<TMP_Text>();
            mAbilityText = transform.Find("AbilityText").GetComponent<TMP_Text>();
            Ability_1_1 = transform.Find("Ability_1/Ability_1_1").GetComponent<Image>();
            Ability_2_1 = transform.Find("Ability_2/Ability_2_1").GetComponent<Image>();
            Ability_2_2 = transform.Find("Ability_2/Ability_2_2").GetComponent<Image>();
            Ability_3_1 = transform.Find("Ability_3/Ability_3_1").GetComponent<Image>();
            mCounter = transform.Find("Count").GetComponent<Image>();
            mFace = transform.Find("Face").GetComponent<Image>();
            mAbility_1 = transform.Find("Ability_1").gameObject.transform;
            mAbility_2 = transform.Find("Ability_2").gameObject.transform;
            mAbility_3 = transform.Find("Ability_3").gameObject.transform;
            mButton = transform.GetComponent<Button>();
            mButton.onClick.AddListener(OnClick);
            //读取SO文件
            mCountInfo = Resources.Load("SO/Count/费用图片") as CountInfo;
        }

        private void OnClick()
        {
            Varification();
            WndManager.Instance.GetWnd<CardListWnd>().OpenCardInfo(mCurrentCardID);
            if (longPressTriggered)
            {
                longPressTriggered = false; // 重置
                return;
            }

            DeckSystem.Instance.AddCardToDeck(mCurrentCardID);
        }

        public void Init(int cardId)
        {
            mCurrentCardID = cardId;
            mCurrentCardInfo = this.SendQuery(new FindCardInfoWithIDQuery(cardId));
            mData = new CardInHandData
            {
                CardId = mCurrentCardInfo.CardID,
                Health = mCurrentCardInfo.Health,
                Attack = mCurrentCardInfo.Attack,
                Cost = mCurrentCardInfo.Cost,
                Abilities = mCurrentCardInfo.Abilities,
                Abilities_Sprite = mCurrentCardInfo.Abilities_Sprite
            };
            Varification();
            ReDraw();
        }

        public void OnPointerDown()
        {
            isLongPress = true;
            pressTime = Time.time;
            longPressTriggered = false; // 每次按下重置
        }

        public void OnPointerUp()
        {
            isLongPress = false;
        }


        void Update()
        {
            if (isLongPress && Time.time - pressTime >= longPressDuration)
            {
                longPressTriggered = true; // 标记长按已触发
                isLongPress = false;
                WndManager.Instance.GetWnd<CardListWnd>().OpenCardInfo(mCurrentCardID);
            }
        }

        public void Varification()
        {
            var hasChoice = DeckSystem.Instance.IsCardInCurrentDeck(mCurrentCardID);
            mButton.interactable = !hasChoice;
        }

        private void ReDraw()
        {
            var path = "PixelArt/pixel" + mCurrentCardInfo.Face.name;
            if (Resources.Load<Sprite>(path) == null)
            {
                mFace.gameObject.SetActive(false);
                mNameText.gameObject.SetActive(true);
                mNameText.text = mCurrentCardInfo.Name == null ? mNameText.text : mCurrentCardInfo.Name;
            }
            else
            {
                mFace.gameObject.SetActive(true);
                mNameText.gameObject.SetActive(false);
                mFace.sprite = Resources.Load<Sprite>(path);
            }

            //条件判断
            if (mCountInfo.FindPixelSpriteWithCostAndCostType(mData.Cost, mCurrentCardInfo.CostType) == null)
            {
                mCounter.gameObject.SetActive(false);
            }
            else
            {
                mCounter.gameObject.SetActive(true);
                mCounter.sprite = mCountInfo.FindPixelSpriteWithCostAndCostType(mData.Cost, mCurrentCardInfo.CostType);
            }

            mAttackText.text = mData.Attack.ToString();
            mHPText.text = mData.Health.ToString();

            //条件判断
            foreach (var ability in mData.Abilities_Sprite)
            {
                var abilityName = "pixel" + ability.name;
                if (Resources.Load<Sprite>($"PixelArt/{abilityName}") == null)
                {
                    mAbility_1.gameObject.SetActive(false);
                    mAbility_2.gameObject.SetActive(false);
                    mAbility_3.gameObject.SetActive(false);
                    mAbilityText.gameObject.SetActive(true);
                    ReDrawAbility();
                    return;
                }
            }


            mAbilityText.gameObject.SetActive(false);

            if (mData.Abilities.Count == 0)
            {
                mAbility_1.gameObject.SetActive(false);
                mAbility_2.gameObject.SetActive(false);
                mAbility_3.gameObject.SetActive(false);
            }
            else if (mData.Abilities.Count == 1)
            {
                mAbility_1.gameObject.SetActive(true);
                mAbility_2.gameObject.SetActive(false);
                mAbility_3.gameObject.SetActive(false);
                var abilityPath = "PixelArt/pixel" + mData.Abilities_Sprite[0].name;
                Ability_1_1.sprite = Resources.Load<Sprite>(abilityPath);
            }
            else if (mData.Abilities.Count == 2)
            {
                mAbility_1.gameObject.SetActive(false);
                mAbility_2.gameObject.SetActive(true);
                mAbility_3.gameObject.SetActive(false);
                var abilityPath1 = "PixelArt/pixel" + mData.Abilities_Sprite[0].name;
                var abilityPath2 = "PixelArt/pixel" + mData.Abilities_Sprite[1].name;

                Ability_2_1.sprite = Resources.Load<Sprite>(abilityPath1);
                ;
                Ability_2_2.sprite = Resources.Load<Sprite>(abilityPath2);
                ;
            }
            else if (mData.Abilities.Count == 3)
            {
                //TODO
            }
        }

        private void ReDrawAbility()
        {
            mAbilityText.text = "";
            foreach (var ability in mData.Abilities)
            {
                if (ability == Abilities.Ants种族蚂蚁)
                    continue;
                mAbilityText.text += RemoveEnglishPrefix(ability.ToString()) + "\n";
            }
        }

        private string RemoveEnglishPrefix(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // 匹配第一个中文字符的位置
            Match match = Regex.Match(input, @"[\u4e00-\u9fa5]");

            if (match.Success)
            {
                // 返回从第一个中文开始到结尾的所有内容
                return input.Substring(match.Index);
            }
            else
            {
                // 没有中文，返回空或原字符串（根据需求）
                return input;
            }
        }
    }
}