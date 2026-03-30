using UnityEngine.Playables;
using UnityEngine.UI;

public class AuthorWnd : BaseWnd
{
    /// <summary>
    ///     初始化UI窗口，获取相对应的组件
    /// </summary>
    private Button mAuthorInfoBtn;

    private Button mDonationBtn;
    private Button mExitBtn;
    private Button mInfo_1;
    private Button mInfo_2;

    /// <summary>
    ///     Timeline组件
    /// </summary>
    private PlayableDirector mReDrawTimeline;

    private Button mServerBtn;
    private Button mSpecialBtn;

    public override void Initial()
    {
        //获取UI组件
        mAuthorInfoBtn = SelfTransform.Find("AuthorInfoBtn").GetComponent<Button>();
        mSpecialBtn = SelfTransform.Find("SpecialBtn").GetComponent<Button>();
        mDonationBtn = SelfTransform.Find("DonationBtn").GetComponent<Button>();
        mServerBtn = SelfTransform.Find("ServerBtn").GetComponent<Button>();
        mInfo_1 = SelfTransform.Find("InfoBtn_1").GetComponent<Button>();
        mInfo_2 = SelfTransform.Find("InfoBtn_2").GetComponent<Button>();
        mExitBtn = SelfTransform.Find("ExitBtn").GetComponent<Button>();

        //获取Timeline
        mReDrawTimeline = SelfTransform.Find("DirectorReDraw").GetComponent<PlayableDirector>();

        //绑定Button音效
        mAuthorInfoBtn.gameObject.AddComponent<CommandButtonMono>();
        mSpecialBtn.gameObject.AddComponent<CommandButtonMono>();
        mDonationBtn.gameObject.AddComponent<CommandButtonMono>();
        mServerBtn.gameObject.AddComponent<CommandButtonMono>();
        mInfo_1.gameObject.AddComponent<CommandButtonMono>();
        mInfo_2.gameObject.AddComponent<CommandButtonMono>();
        mExitBtn.gameObject.AddComponent<CommandButtonMono>();

        //绑定Button跳转
        mExitBtn.onClick.AddListener(JumpToStart);
        mAuthorInfoBtn.onClick.AddListener(JumpToAuthorInfo);
        mSpecialBtn.onClick.AddListener(JumpToSpecialInfo);
        mDonationBtn.onClick.AddListener(JumpToDonationWnd);
    }



    #region Timeline动画

    //刷新动画
    public void ReDrawAnim()
    {
        if (mReDrawTimeline.state == PlayState.Playing) mReDrawTimeline.Stop();
        mReDrawTimeline.Play();
    }

    #endregion

    #region UI跳转

    private void JumpToStart()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<StartWnd>();
        WndManager.Instance.GetWnd<StartWnd>().ReDrawAnim();
    }

    private void JumpToAuthorInfo()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<AuthorInfoWnd>();
        WndManager.Instance.GetWnd<AuthorInfoWnd>().ReDrawAnim();
    }

    private void JumpToSpecialInfo()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<SpecialWnd>();
        WndManager.Instance.GetWnd<SpecialWnd>().ReDrawAnim();
    }
    
    private void JumpToDonationWnd()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<MoneyWnd>();
        WndManager.Instance.GetWnd<MoneyWnd>().ReDrawAnim();
    }

    #endregion
}