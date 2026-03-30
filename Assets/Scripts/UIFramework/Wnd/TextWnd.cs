using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inscryption
{
    public class TextWnd : BaseWnd
    {
        private Button BGBtn;
        private Button mEnemyDrawCard;
        private TMP_InputField mEnemyDrawCardInput;
        private Button mPlayerDrawMain;
        private TMP_InputField mPlayerDrawMainInput;
        private Button mPlayerDrawSecond;
        private TMP_InputField mPlayerDrawSecondInput;
        private Button mReduceCost;
        private TMP_InputField mReduceCostInput;
        private Button mEnemySelect;
        private TMP_InputField mEnemySelectInput;
        private Button mEnemySummon;
        private TMP_InputField mEnemySummonInput;
        private Button mSummonCard;
        private TMP_InputField mSummonCardInput_1;
        private TMP_InputField mSummonCardInput_2;
        private Toggle mIsMyCard;
        private Button mTargetSummon;
        private Button mAttack;
        private Button mDie;
        private TestMono mTestMono;

        public override void Initial()
        {
            mTestMono = SelfTransform.gameObject.AddComponent<TestMono>();

            BGBtn = SelfTransform.Find("BG_Huge").GetComponent<Button>();
            mPlayerDrawMain = SelfTransform.Find("PlayerDrawMain/Button").GetComponent<Button>();
            mPlayerDrawSecond = SelfTransform.Find("PlayerDrawSecond/Button").GetComponent<Button>();
            mEnemyDrawCard = SelfTransform.Find("EnemyDrawMain/Button").GetComponent<Button>();
            mReduceCost = SelfTransform.Find("ReduceCost/Button").GetComponent<Button>();
            mEnemySelect = SelfTransform.Find("EnemySelect/Button").GetComponent<Button>();
            mEnemySummon = SelfTransform.Find("EnemySummon/Button").GetComponent<Button>();
            mSummonCard = SelfTransform.Find("SummonCard/Button").GetComponent<Button>();
            mTargetSummon = SelfTransform.Find("TargetSummon/Button").GetComponent<Button>();
            mAttack = SelfTransform.Find("Attack/Button").GetComponent<Button>();
            mDie = SelfTransform.Find("Die/Button").GetComponent<Button>();
            
            mPlayerDrawMainInput = SelfTransform.Find("PlayerDrawMain/Input").GetComponent<TMP_InputField>();
            mPlayerDrawSecondInput = SelfTransform.Find("PlayerDrawSecond/Input").GetComponent<TMP_InputField>();
            mEnemyDrawCardInput = SelfTransform.Find("EnemyDrawMain/Input").GetComponent<TMP_InputField>();
            mReduceCostInput = SelfTransform.Find("ReduceCost/Input").GetComponent<TMP_InputField>();
            mEnemySelectInput = SelfTransform.Find("EnemySelect/Input").GetComponent<TMP_InputField>();
            mEnemySummonInput = SelfTransform.Find("EnemySummon/Input").GetComponent<TMP_InputField>();
            mSummonCardInput_1 = SelfTransform.Find("SummonCard/Input_1").GetComponent<TMP_InputField>();
            mSummonCardInput_2 = SelfTransform.Find("SummonCard/Input_2").GetComponent<TMP_InputField>();
        
            mIsMyCard = SelfTransform.Find("SummonCard/Toggle").GetComponent<Toggle>();

            BGBtn.onClick.AddListener(CloseWnd);
            mPlayerDrawMain.onClick.AddListener(PlayerDrawMainTest);
            mPlayerDrawSecond.onClick.AddListener(PlayerDrawSecondTest);
            mEnemyDrawCard.onClick.AddListener(EnemyDrawCardTest);
            mReduceCost.onClick.AddListener(ReduceCostTest);
            mEnemySelect.onClick.AddListener(EnemySelect);
            mEnemySummon.onClick.AddListener(EnemySummon);
            mSummonCard.onClick.AddListener(SummonCard);
            mTargetSummon.onClick.AddListener(TargetSummon);
            mAttack.onClick.AddListener(Attack);
            mDie.onClick.AddListener(Die);
        }

        public void SetAsFirst()
        {
            SelfTransform.SetAsLastSibling();
        }

        private void Die()
        {
            WndManager.Instance.OpenWnd<WinWnd>();
            WndManager.Instance.GetWnd<WinWnd>().SetFinishTip(false);
            GameNet.Instance.SyncWinResult();
            CloseWnd();
            
        }

        private void Attack()
        {
            mTestMono.Attack();
        }

        private void TargetSummon()
        {
            mTestMono.SummonCardInSlot(666, 0, false);
            mTestMono.SummonCardInSlot(666, 2, false);
            mTestMono.SummonCardInSlot(666, 4, false);
        }

        private void SummonCard()
        {
            mTestMono.SummonCardInSlot(int.Parse(mSummonCardInput_1.text), int.Parse(mSummonCardInput_2.text),
                mIsMyCard.isOn);
        }

        private void EnemySummon()
        {
            mTestMono.EnemySummon(int.Parse(mEnemySummonInput.text));
        }

        private void EnemySelect()
        {
            mTestMono.EnemySelect(int.Parse(mEnemySelectInput.text));
        }


        private void PlayerDrawMainTest()
        {
            if (mPlayerDrawMainInput.text == null) return;
            WndManager.Instance.GetWnd<GameWnd>().CardInHandManager
                .PlayerDrawMainCard(int.Parse(mPlayerDrawMainInput.text));
        }

        private void PlayerDrawSecondTest()
        {
            if (mPlayerDrawMainInput.text == null) return;
            WndManager.Instance.GetWnd<GameWnd>().CardInHandManager
                .PlayerDrawSecondCard(int.Parse(mPlayerDrawSecondInput.text));
        }

        private void EnemyDrawCardTest()
        {
            if (mPlayerDrawMainInput.text == null) return;
            WndManager.Instance.GetWnd<GameWnd>().CardInHandManager
                .EnemyDrawCard((int.Parse(mEnemyDrawCardInput.text)));
        }

        private void ReduceCostTest()
        {
            if (mPlayerDrawMainInput.text == null) return;
            WndManager.Instance.GetWnd<GameWnd>().ReduceCost(int.Parse(mReduceCostInput.text));
        }
    }
}