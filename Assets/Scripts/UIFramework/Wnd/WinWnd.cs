using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inscryption
{
    public class WinWnd : BaseWnd, ICanGetSystem
    {
        private Image mFire;
        private TMP_Text mTipText;
        private TMP_Text mContinueText;
        private Button mContinueButton;

        public override void Initial()
        {
            GameManager.Instance.TurnOnAndDownLight(true);

            mFire = SelfTransform.Find("Fire").GetComponent<Image>();
            mTipText = SelfTransform.Find("Tip").GetComponent<TMP_Text>();
            mContinueText = SelfTransform.Find("Continue").GetComponent<TMP_Text>();
            mContinueButton = SelfTransform.Find("Fade").GetComponent<Button>();
            mContinueButton.interactable = false;

            mFire.transform.GetComponent<FadeInEffect>().Init();
            mTipText.transform.GetComponent<FadeInEffect>().Init();

            mContinueButton.onClick.AddListener(BackToLobby);
            this.GetSystem<ITimeSystem>()
                .AddDelayTask(2f, () =>
                {
                    mContinueText.transform.GetComponent<FadeInEffect>().Init();
                    mContinueButton.interactable = true;
                });
        }

        public void SetFinishTip(bool isVictory)
        {
            if (WndManager.Instance?.GetWnd<GameWnd>() != null)
            {
                if (isVictory)
                {
                    mTipText.text = "<shake>胜利<@shake>";
                    WndManager.Instance.GetWnd<GameWnd>().PlayerWinGame();
                }
                else
                {
                    mTipText.text = "<shake>失败<@shake>";
                    WndManager.Instance.GetWnd<GameWnd>().PlayerLoseGame();
                }
            }
        }

        private void BackToLobby()
        {
            this.GetSystem<ITurnSystem>().State.Value = TurnState.NoMatch;
            GameManager.Instance.TurnOnAndDownLight(false);
            if (LobbyNet.Instance.LocalRoomInfo.isMatch)
            {
                WndManager.Instance.DeleteWnd<GameWnd>();
                WndManager.Instance.OpenWnd<LobbyWnd>();
                WndManager.Instance.DeleteWnd<WinWnd>();
            }
            else
            {
                WndManager.Instance.DeleteWnd<GameWnd>();
                WndManager.Instance.OpenWnd<RoomWnd>();
                WndManager.Instance.DeleteWnd<WinWnd>();
                if (LobbyNet.Instance.IsHostPlayer())
                {
                    GameNet.Instance.SyncBack();
                }
                else
                {
                    WndManager.Instance.GetWnd<RoomWnd>().mIsReadyToggle.isOn = false;
                    WndManager.Instance.GetWnd<RoomWnd>().OnReadyToggle(false);
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return Inscription.Interface;
        }
    }
}