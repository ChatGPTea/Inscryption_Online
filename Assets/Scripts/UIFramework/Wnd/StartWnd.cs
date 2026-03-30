using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class StartWnd : BaseWnd
{
    private Button mAuthorBtn;
    private Button mExitAccountBtn;
    private Button mExitBtn;
    private string currentVersion = "0.0.6";
    private bool NoFitVersion = false;
    /// <summary>
    ///     初始化UI窗口，获取相对应的组件
    /// </summary>
    private Button mLoginBtn;

    private PlayableDirector mReDrawTimeline;
    private Button mSoloBtn;
    private TMP_Text mVersionText;

    /// <summary>
    ///     Timeline组件
    /// </summary>
    private PlayableDirector mStartInitTimeline;

    public override void Initial()
    {
        //获取UI组件
        mLoginBtn = SelfTransform.Find("LoginBtn").GetComponent<Button>();
        mExitAccountBtn = SelfTransform.Find("SettingBtn").GetComponent<Button>();
        mSoloBtn = SelfTransform.Find("SoloBtn").GetComponent<Button>();
        mAuthorBtn = SelfTransform.Find("AuthorBtn").GetComponent<Button>();
        mExitBtn = SelfTransform.Find("ExitBtn").GetComponent<Button>();
        mVersionText = SelfTransform.Find("V").GetComponent<TextMeshProUGUI>();

        //获取Timeline
        mStartInitTimeline = SelfTransform.Find("DirectorInit").GetComponent<PlayableDirector>();
        mReDrawTimeline = SelfTransform.Find("DirectorReDraw").GetComponent<PlayableDirector>();

        //绑定Button音效
        mLoginBtn.gameObject.AddComponent<CommandButtonMono>();
        mExitAccountBtn.gameObject.AddComponent<CommandButtonMono>();
        mSoloBtn.gameObject.AddComponent<CommandButtonMono>();
        mAuthorBtn.gameObject.AddComponent<CommandButtonMono>();
        mExitBtn.gameObject.AddComponent<CommandButtonMono>();

        //绑定Button跳转
        mLoginBtn.onClick.AddListener(OnStartClick);
        mAuthorBtn.onClick.AddListener(JumpToAuthor);
        mExitBtn.onClick.AddListener(Exit);
        mExitAccountBtn.onClick.AddListener(JumpToBugWnd);

        // 第一次启动播放初始动画
        InitialAnim();
        GetLatestVersion();
    }

    #region 版本号相关逻辑

    private void GetLatestVersion()
    {
        string resource = "user/get-latest-version";
    
        // 构造 POST 数据，传当前版本号
        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("current_version", currentVersion);
    
        HTTPHandler.Instance.POST(resource, formData, OnVersionResponse, OnError);
    }

    void OnVersionResponse(string msg)
    {
        Debug.Log("原始响应: " + msg);
    
        JObject result = JsonConvert.DeserializeObject<JObject>(msg);
    
        if (result == null)
        {
            Debug.LogError("解析结果为空");
            return;
        }
    
        int code = (int)result["code"];
    
        if (code == 1001)
        {
            // 直接取 data 里的 version_number
            // 用索引器取，不要用强转
            JToken dataToken = result["data"];
            string versionNumber = dataToken["version_number"].ToString();
        
            Debug.Log("version_number 值: [" + versionNumber + "]");
        
            if (mVersionText != null)
            {
                if (currentVersion == versionNumber)
                {
                    mVersionText.text = "V " + versionNumber;
                }
                else
                {
                    NoFitVersion = true;
                    mVersionText.text = "当前版本为: V" + currentVersion + " 最新版本为: V" + versionNumber;
                }
            }
        }
        else
        {
            Debug.Log("获取版本号失败: " + (string)result["msg"]);
        }
    }

    #endregion



    #region 登录相关逻辑

    private void OnStartClick()
    {
        if (NoFitVersion) return;
        UserData userData = ConfigManager.Instance.UserData;
        if (userData != null)
        {
            Dictionary<string, string> formData = new Dictionary<string, string>();
            formData.Add("username", userData.username);
            formData.Add("token", userData.token);
            string resource = "user/auto-login";
            HTTPHandler.Instance.POST(resource, formData, OnResponse, OnError);
        }
        else
        {
            JumpToLogin();
        }
    }

    void OnResponse(string msg)
    {
        ServiceResult<UserData> result = JsonConvert.DeserializeObject<ServiceResult<UserData>>(msg);
        switch (result.code)
        {
            case 0:
            case -1:
            case -2:
            case -3:
            {
                JumpToLogin();
            }
                break;
            case 1001:
            {
                ConfigManager.Instance.SaveUserData(result.data[0]);
                JumpToAutoLogin();
            }
                break;
        }
    }

    void OnError(string msg)
    {
        Debug.Log(msg);
    }

    #endregion

    #region Timeline动画

    //初始化动画
    private void InitialAnim()
    {
        mStartInitTimeline.Play();
    }

    //刷新动画
    public void ReDrawAnim()
    {
        if (mStartInitTimeline.state == PlayState.Playing) mStartInitTimeline.Stop();

        if (mReDrawTimeline.state == PlayState.Playing) mReDrawTimeline.Stop();
        mReDrawTimeline.Play();
    }

    //左键取消动画
    public void ExitAnim()
    {
        if (mStartInitTimeline.state == PlayState.Playing)
        {
            var duration = mStartInitTimeline.duration;
            mStartInitTimeline.time = duration;
        }
    }

    #endregion

    #region UI跳转

    private void JumpToAutoLogin()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<AutoLoginWnd>();
        WndManager.Instance.GetWnd<AutoLoginWnd>().InitialAnim();
        WndManager.Instance.GetWnd<AutoLoginWnd>().ReDrawAnim();
    }

    private void JumpToLogin()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<LoginWnd>();
        WndManager.Instance.GetWnd<LoginWnd>().ReDrawAnim();
    }

    private void JumpToAuthor()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<AuthorWnd>();
        WndManager.Instance.GetWnd<AuthorWnd>().ReDrawAnim();
    }
    
    private void JumpToBugWnd()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<BugWnd>();
        WndManager.Instance.GetWnd<BugWnd>().ReDrawAnim();
    }

    private void Exit()
    {
        //TODO 登出
        Application.Quit();
    }

    #endregion
}