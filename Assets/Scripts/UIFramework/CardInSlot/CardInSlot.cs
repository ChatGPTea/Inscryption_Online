using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FunkyCode.Rendering.Universal;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace Inscryption
{
    public class CardInSlot : InscryptionController
    {
        //卡牌状态
        public CardInSlotState mCurrentState;

        //能力组件
        private Transform mAbility_1;
        private Transform mAbility_2;
        private Transform mAbility_3;


        //获取UI组件
        private TMP_Text mAttackText;
        private TMP_Text mHPText;
        private TMP_Text mNameText;
        private Image Ability_1_1;
        private Image Ability_2_1;
        private Image Ability_2_2;
        private Image Ability_3_1;
        private Image mDecal;
        public Image mFace;
        private Image mFace_Add;
        public Image mBG;
        public Image mCounter;
        private Image mMark;
        private CountInfo mCountInfo;
        public CardInfo mCurrentCardInfo;
        public int mCurrentCardID;
        public int mValue = 1;
        public Image Waterborne;

        //可保存，传递数据
        public CardInHandData mData;

        //Bool标志位
        public bool isMycard = true;

        //特殊能力
        public List<Effect> Effects = new List<Effect>();
        public Dictionary<Buff, int> Buffs = new Dictionary<Buff, int>();
        public int OriginATK;

        private void Awake()
        {
            //获取UI组件
            mBG = transform.Find("BG").GetComponent<Image>();
            mDecal = transform.Find("Decal").GetComponent<Image>();
            mFace = transform.Find("Face").GetComponent<Image>();
            mFace_Add = transform.Find("Face_Add").GetComponent<Image>();
            mCounter = transform.Find("Counter").GetComponent<Image>();
            mMark = transform.Find("Mark").GetComponent<Image>();
            mNameText = transform.Find("Name").GetComponent<TMP_Text>();
            mAttackText = transform.Find("Attack").GetComponent<TMP_Text>();
            mHPText = transform.Find("HP").GetComponent<TMP_Text>();
            Waterborne = transform.Find("Waterborne").GetComponent<Image>();
            //获取能力组件
            mAbility_1 = transform.Find("Ability_1").gameObject.transform;
            mAbility_2 = transform.Find("Ability_2").gameObject.transform;
            mAbility_3 = transform.Find("Ability_3").gameObject.transform;
            Ability_1_1 = transform.Find("Ability_1/Ability_1_1").GetComponent<Image>();
            Ability_2_1 = transform.Find("Ability_2/Ability_2_1").GetComponent<Image>();
            Ability_2_2 = transform.Find("Ability_2/Ability_2_2").GetComponent<Image>();
            Ability_3_1 = transform.Find("Ability_3/Add/Ability_3_1").GetComponent<Image>();
            //读取SO文件
            mCountInfo = Resources.Load("SO/Count/费用图片") as CountInfo;

            //设置初始状态
            mCurrentState = CardInSlotState.Idle;
        }

        /// <summary>
        ///     初始化手牌,正常手段献祭
        /// </summary>
        public void Init(int cardId, CardInHandData data, bool MyCard)
        {
            mCurrentCardID = cardId;
            mCurrentCardInfo = this.SendQuery(new FindCardInfoWithIDQuery(cardId));
            isMycard = MyCard;
            mData = data;

            InitOriginValue();
            ReDraw();
            AddEffect();
        }

        //数据不继承初始化
        public void Init(int cardId, bool MyCard)
        {
            mCurrentCardID = cardId;
            mCurrentCardInfo = this.SendQuery(new FindCardInfoWithIDQuery(cardId));
            isMycard = MyCard;
            
            int currentHealth = mData.Health;  // 保存旧值
            int currentAttack = mData.Attack;
            
            mData = new CardInHandData
            {
                CardId = mCurrentCardInfo.CardID,
                Health = mCurrentCardInfo.Health,
                Attack = mCurrentCardInfo.Attack,
                Cost = mCurrentCardInfo.Cost,
                Abilities = mCurrentCardInfo.Abilities,
                Abilities_Sprite = mCurrentCardInfo.Abilities_Sprite
            };
            InitOriginValue();
            ReDraw();
            AddEffect();
        }

        //数据继承初始化
        public void Init(int cardId)
        {
            mCurrentCardID = cardId;
            mCurrentCardInfo = this.SendQuery(new FindCardInfoWithIDQuery(cardId));
            mData = new CardInHandData
            {
                CardId = mCurrentCardInfo.CardID,
                Health = mData.Health,
                Attack = mData.Attack,
                Cost = mCurrentCardInfo.Cost,
                Abilities = mCurrentCardInfo.Abilities,
                Abilities_Sprite = mCurrentCardInfo.Abilities_Sprite
            };
            InitOriginValue();
            ReDraw();
            AddEffect();
        }

        public void OnPlaceEffect()
        {
            for (var j = 0; j < Effects.Count; j++)
            {
                var Effect = Effects[j];
                Effect.Init();
                Effect.OnPlaceExecute();
            }
        }

        /// <summary>
        /// 设置原始值
        /// </summary>
        private void InitOriginValue()
        {
            OriginATK = mData.Attack;
        }

        public void ReDraw()
        {
            mBG.sprite = mCurrentCardInfo.BackGround == null ? mBG.sprite : mCurrentCardInfo.BackGround;
            mDecal.sprite = mCurrentCardInfo.Decal == null ? mDecal.sprite : mCurrentCardInfo.Decal;
            mFace.sprite = mCurrentCardInfo.Face == null ? mFace.sprite : mCurrentCardInfo.Face;
            mFace_Add.sprite = mCurrentCardInfo.Face_Add == null ? mFace_Add.sprite : mCurrentCardInfo.Face_Add;
            mNameText.text = mCurrentCardInfo.Name == null ? mNameText.text : mCurrentCardInfo.Name;
            mAttackText.text = mData.Attack.ToString();
            mHPText.text = mData.Health.ToString();

            //条件判断
            if (mCountInfo.FindSpriteWithCostAndCostType(mData.Cost, mCurrentCardInfo.CostType) == null)
            {
                mCounter.gameObject.SetActive(false);
            }
            else
            {
                mCounter.gameObject.SetActive(true);
                mCounter.sprite = mCountInfo.FindSpriteWithCostAndCostType(mData.Cost, mCurrentCardInfo.CostType);
            }

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
                Ability_1_1.sprite = mData.Abilities_Sprite[0];
            }
            else if (mData.Abilities.Count == 2)
            {
                mAbility_1.gameObject.SetActive(false);
                mAbility_2.gameObject.SetActive(true);
                mAbility_3.gameObject.SetActive(false);
                Ability_2_1.sprite = mData.Abilities_Sprite[0];
                Ability_2_2.sprite = mData.Abilities_Sprite[1];
            }
            else if (mData.Abilities.Count == 3)
            {
                //TODO
            }
        }

        //献祭相关 以及 查看相关

        public void Select()
        {
            var _randomNum = Random.Range(1, 11);
            AudioKit.PlaySound("resources://Sound/Cards/card" + _randomNum);
            if (mCurrentState == CardInSlotState.OnAnim) return;
            if (this.GetSystem<ITurnSystem>().State.Value == TurnState.AfterDrawCard ||
                this.GetSystem<ITurnSystem>().State.Value == TurnState.PlayerFirstTurnBegin)
            {
                bool needSacrifice = WndManager.Instance.GetWnd<GameWnd>().CurrentCardID != -1;
                if (mCurrentState == CardInSlotState.Idle && isMycard && needSacrifice && !mCurrentCardInfo.isObstacle)
                {
                    ProvideSacrifice();
                }
                else if (mCurrentState == CardInSlotState.OnSacrifice && isMycard && needSacrifice)
                {
                    QuitSacrifice();
                }
                else if (mCurrentState == CardInSlotState.Idle && isMycard && !needSacrifice)
                {
                    if (WndManager.Instance.GetWnd<GameWnd>().isReady) return;
                    WndManager.Instance.GetWnd<GameWnd>().ReDrawCardInfoTip(gameObject,false);
                }
                else if (mCurrentState == CardInSlotState.Idle && !isMycard)
                {
                    if (needSacrifice||WndManager.Instance.GetWnd<GameWnd>().isReady) return;
                    if (WndManager.Instance.GetWnd<GameWnd>().mCost is -1000 or 0)
                    {
                        WndManager.Instance.GetWnd<GameWnd>().TryUseEnemyCardInSlot();
                    }

                    Debug.Log("观察敌方造物");
                    WndManager.Instance.GetWnd<GameWnd>().ReDrawCardInfoTip(gameObject,true);

                }
            }
        }

        public void ProvideSacrifice()
        {
            var Cost = WndManager.Instance.GetWnd<GameWnd>().mCost;
            if (Cost is -1000 or 0)
            {
                //提供献祭失败
                return;
            }

            //提供献祭成功
            mCurrentState = CardInSlotState.OnSacrifice;
            WndManager.Instance.GetWnd<GameWnd>().SacrificeList.Add(this);
            WndManager.Instance.GetWnd<GameWnd>().ReduceCost(mValue);
            mMark.gameObject.SetActive(true);
            mMark.color = new Color32(156, 36, 49, 255);
        }

        public void QuitSacrifice()
        {
            WndManager.Instance.GetWnd<GameWnd>().ReduceCost(-mValue);
            WndManager.Instance.GetWnd<GameWnd>().SacrificeList.Remove(this);
            mMark.gameObject.SetActive(false);
            mCurrentState = CardInSlotState.Idle;
        }

        //献祭成功
        public void QuitSacrifice(bool isSuccess)
        {
            if (isSuccess)
            {
                WndManager.Instance.GetWnd<GameWnd>().SacrificeList.Remove(this);
            }

            mMark.color = Color.black;
            mCurrentState = CardInSlotState.Idle;

            transform.DOKill();
            DOVirtual.DelayedCall(0.1f, () => mMark.gameObject.SetActive(false));
            if (transform.GetComponent<ManyLives生生不息>() != null)
            {
                transform.GetComponent<ManyLives生生不息>().Init();
                transform.GetComponent<ManyLives生生不息>().AddCount();
                return;
            }

            mCounter.gameObject.SetActive(false);
            Sequence quitSequence = DOTween.Sequence();
            quitSequence.Append(transform.DOScale(0.55f, 0.3f).SetEase(Ease.OutQuad))
                .Join(mBG.DOColor(Color.black, 0.3f))
                .Join(mFace.DOColor(Color.black, 0.3f));
            quitSequence.Append(mBG.DOColor(new Color32(0, 0, 0, 0), 0.3f))
                .Join(mFace.DOColor(new Color32(0, 0, 0, 0), 0.3f));
            quitSequence.OnComplete(() =>
            {
                this.SendCommand(new RemoveCardInSlotCommand(transform.parent.parent.gameObject,
                    this.GetComponentInParent<Slot>().slotIndex,
                    isMycard));
                Destroy(gameObject);
            });
        }

        #region 翻转相关

        /// <summary>
        /// 播放翻转动画,变化成另一种卡卡牌
        /// </summary>
        public void PlayFlipAnimation(int cardID)
        {
            Sequence flipSequence = DOTween.Sequence();
            flipSequence.Append(transform.DOScaleX(0f, 0.2f).SetEase(Ease.InBack))
                .AppendCallback(() => { Init(cardID, isMycard); })
                .Append(transform.DOScaleX(0.6f, 0.2f).SetEase(Ease.OutBack));
        }
        
        public void PlayFlipAnimation(int cardID,bool needData)
        {
            Sequence flipSequence = DOTween.Sequence();
            flipSequence.Append(transform.DOScaleX(0f, 0.2f).SetEase(Ease.InBack))
                .AppendCallback(() => { Init(cardID); })
                .Append(transform.DOScaleX(0.6f, 0.2f).SetEase(Ease.OutBack));
        }

        /// <summary>
        /// 水袭动画
        /// </summary>
        public void PlayFlipAnimation(bool isOn)
        {
            Sequence flipSequence = DOTween.Sequence();
            flipSequence.Append(transform.DOScaleX(0f, 0.2f).SetEase(Ease.InBack))
                .AppendCallback(() => { Waterborne.gameObject.SetActive(isOn); })
                .Append(transform.DOScaleX(0.6f, 0.2f).SetEase(Ease.OutBack));
        }

        //翻转冲刺能手的图标
        public void ChangeDirection(bool Right, Abilities abilitieIcon)
        {
            int index = mData.Abilities.IndexOf(abilitieIcon);
            if (mData.Abilities.Count == 1)
            {
                Ability_1_1.transform.localScale = Right ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            }
            else if (mData.Abilities.Count == 2)
            {
                if (index == 1)
                {
                    Ability_2_1.transform.localScale = Right ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
                }
                else if (index == 2)
                {
                    Ability_2_2.transform.localScale = Right ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
                }
            }
            else if (mData.Abilities.Count == 3)
            {
                //TODO
            }
        }

        #endregion


        #region 利用名字找脚本

        //Effect能力相关
        public void AddEffect()
        {
            Effects.Clear();
            GetComponents<Effect>()?.ToList().ForEach(effect => Destroy(effect));
            AddScriptByString("BaseAttack");
            //Effects.Add(GetComponent<BaseAttack>());
            if (mData.Abilities.Count != 0)
            {
                foreach (var abi in mData.Abilities)
                {
                    AddScriptByString(abi.ToString());
                    var effectComponent = GetComponent(abi.ToString()) as Effect;
                    //Effects.Add(effectComponent);
                }
            }

            StartCoroutine(AddEffectToListCoroutine());
        }

        public IEnumerator AddEffectToListCoroutine()
        {
            yield return null;
            // 获取GameObject上所有继承自Effect的组件
            var allEffectComponents = GetComponents<Effect>();

            foreach (var effect in allEffectComponents)
            {
                if (effect != null && !Effects.Contains(effect))
                {
                    Effects.Add(effect);
                }
            }

            OnPlaceEffect();
        }

        void AddScriptByString(string typeName)
        {
            System.Type scriptType = System.Type.GetType(typeName);

            if (scriptType != null)
            {
                Component component = gameObject.AddComponent(scriptType);
            }
            else
            {
                Debug.LogError($"找不到脚本类型: {typeName}");

                // 尝试在项目程序集中查找
                scriptType = FindTypeInAssemblies(typeName);
                if (scriptType != null)
                {
                    Component component = gameObject.AddComponent(scriptType);
                }
            }
        }

        System.Type FindTypeInAssemblies(string typeName)
        {
            // 搜索所有已加载的程序集
            foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Type type = assembly.GetType(typeName);
                if (type != null) return type;

                // 也尝试查找带命名空间的版本
                type = assembly.GetType("MyGame." + typeName);
                if (type != null) return type;
            }

            return null;
        }

        #endregion

        #region 造物交互部分

        public void GetDamage(int damage)
        {
            mData.Health -= damage;
            if (mData.Health <= 0) mData.Health = 0;
            ReDraw();
        }

        public void GetDamage(int damage, GameObject target)
        {
            AudioKit.PlaySound("resources://Sound/Cards/cardattack_card");
            if (mData.Health == 0)
            {
                return;
            }


            for (var j = 0; j < Effects.Count; j++)
            {
                var Effect = Effects[j];
                Effect.Init();
                Effect.AfterTakeAttackExecute();
            }

            mData.Health -= damage;
            if (mData.Health <= 0)
            {
                mData.Health = 0;
                DieWithEffect();
            }

            if (transform?.GetComponentInChildren<SharpQuills尖刺铠甲>() != null)
            {
                target.GetComponent<CardInSlot>()?.GetDamage(1, this.gameObject);
            }

            ReDraw();
        }

        public void DieWithEffect()
        {
            AudioKit.PlaySound("resources://Sound/Cards/card_death");
            foreach (var effect in Effects)
            {
                effect.OnDieExecute();
            }

            //动画，造物消失
            mMark.color = Color.black;
            mCurrentState = CardInSlotState.Idle;

            transform.DOKill();
            mCounter.gameObject.SetActive(false);
            DOVirtual.DelayedCall(0.1f, () => mMark.gameObject.SetActive(false));
            Sequence quitSequence = DOTween.Sequence();
            quitSequence.Append(transform.DOScale(0.55f, 0.3f).SetEase(Ease.OutQuad))
                .Join(mBG.DOColor(Color.black, 0.3f))
                .Join(mFace.DOColor(Color.black, 0.3f));
            quitSequence.Append(mBG.DOColor(new Color32(0, 0, 0, 0), 0.3f))
                .Join(mFace.DOColor(new Color32(0, 0, 0, 0), 0.3f));
            quitSequence.OnComplete(() =>
            {
                this.SendCommand(new RemoveCardInSlotCommand(transform.parent.parent.gameObject,
                    this.GetComponentInParent<Slot>().slotIndex,
                    isMycard));
                Destroy(gameObject);
            });
        }

        #endregion
    }
}