using UnityEngine.Playables;

public class AutoLoginWnd : BaseWnd
{
    /// <summary>
    ///     Timeline组件
    /// </summary>
    private PlayableDirector mInitTimeline;

    private PlayableDirector mReDrawTimeline;

    public override void Initial()
    {
        //获取Timeline
        mInitTimeline = SelfTransform.Find("DirectorAwake").GetComponent<PlayableDirector>();
        mReDrawTimeline = SelfTransform.Find("DirectorLoad").GetComponent<PlayableDirector>();
        mInitTimeline.stopped += JumpToLobby;

        // 第一次启动播放初始动画
        InitialAnim();
        ReDrawAnim();
    }

    #region UI跳转

    private void JumpToLobby(PlayableDirector director)
    {
        StartNet.Instance.StartClient(ConfigManager.Instance.ConfigInfo.gameserver,
            (ushort)ConfigManager.Instance.ConfigInfo.port);
        CloseWnd();
        WndManager.Instance.OpenWnd<LobbyWnd>();
        WndManager.Instance.GetWnd<LobbyWnd>().ReDrawAnim();
    }

    #endregion

    #region Timeline动画

    //初始化动画
    public void InitialAnim()
    {
        if (mInitTimeline.state == PlayState.Playing) return;

        mInitTimeline.Play();
    }

    //刷新动画
    public void ReDrawAnim()
    {
        if (mReDrawTimeline.state == PlayState.Playing) mReDrawTimeline.Stop();

        mReDrawTimeline.Play();
    }

    #endregion
}