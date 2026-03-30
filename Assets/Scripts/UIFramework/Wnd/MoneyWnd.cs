using UnityEngine.Playables;
using UnityEngine.UI;

public class MoneyWnd : BaseWnd
{
    /// <summary>
    ///     初始化UI窗口，获取相对应的组件
    /// </summary>
    private Button mExitBtn;

    /// <summary>
    ///     Timeline组件
    /// </summary>
    private PlayableDirector mReDrawTimeline;

    public override void Initial()
    {
        //获取UI组件
        mExitBtn = SelfTransform.Find("ExitBtn").GetComponent<Button>();

        //获取Timeline
        mReDrawTimeline = SelfTransform.Find("DirectorReDraw").GetComponent<PlayableDirector>();

        //绑定Button音效
        mExitBtn.gameObject.AddComponent<CommandButtonMono>();

        //绑定Button跳转
        mExitBtn.onClick.AddListener(JumpToAuthor);
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

    private void JumpToAuthor()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<AuthorWnd>();
        WndManager.Instance.GetWnd<AuthorWnd>().ReDrawAnim();
    }

    #endregion
}