using System.Collections.Generic;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.UI;

public class LoginWnd : BaseWnd
{
    private Button mConfirmBtn;

    /// <summary>
    ///     初始化UI窗口，获取相对应的组件
    /// </summary>
    private Button mExitBtn;
    private TMP_InputField mLoginUsername;
    private TMP_InputField mPassword;
    private TMP_Text mTip;

    /// <summary>
    ///     Timeline组件
    /// </summary>
    private PlayableDirector mReDrawTimeline;

    private Button mRegisterBtn;

    public override void Initial()
    {
        //获取UI组件
        mExitBtn = SelfTransform.Find("ExitBtn").GetComponent<Button>();
        mConfirmBtn = SelfTransform.Find("ConfirmBtn").GetComponent<Button>();
        mRegisterBtn = SelfTransform.Find("RegisterBtn").GetComponent<Button>();
        mTip = SelfTransform.Find("Tip_1").GetComponent<TMP_Text>();

        //获取Timeline
        mReDrawTimeline = SelfTransform.Find("DirectorReDraw").GetComponent<PlayableDirector>();

        //绑定Button音效
        mExitBtn.gameObject.AddComponent<CommandButtonMono>();
        mConfirmBtn.gameObject.AddComponent<CommandButtonMono>();
        mRegisterBtn.gameObject.AddComponent<CommandButtonMono>();

        //绑定Button跳转
        mExitBtn.onClick.AddListener(JumpToStart);
        mConfirmBtn.onClick.AddListener(OnLoginClick);
        mRegisterBtn.onClick.AddListener(JumpToRegister);
        
        //登录注册相关UI
        mLoginUsername = SelfTransform.Find("Account/AccountInput").GetComponent<TMP_InputField>();
        mPassword = SelfTransform.Find("Password/PassWordInput").GetComponent<TMP_InputField>();
    }

    #region 登录注册方面

    private void OnLoginClick()
    {
        if (mLoginUsername.text == "" && mPassword.text == "")
        {
            ShowTip("用户名或密码不能为空");
            return;
        }
        ShowTip("挑战者信息已登入");
        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("username", mLoginUsername.text);
        formData.Add("password", mPassword.text);
        string resource = "user/login";
        HTTPHandler.Instance.POST(resource, formData, OnResponse, OnError);
    }

    void OnResponse(string msg)
    {
        ServiceResult<UserData> result = JsonConvert.DeserializeObject<ServiceResult<UserData>>(msg);

        switch (result.code)
        {
            case 1001:
            {
                UserData userData = result.data[0];
                ConfigManager.Instance.SaveUserData(userData);
                JumpToLobby();
                
                //启动客户端
                StartNet.Instance.StartClient(ConfigManager.Instance.ConfigInfo.gameserver,(ushort)ConfigManager.Instance.ConfigInfo.port);
            }
                break;
            case -1:
            {
                ShowTip("邮箱账号错误，请检查是否填写正确的邮箱账号");
            }
                break;
            case -2 :
            {
                ShowTip("不要重复登录");
            }
                break;
            case -3 :
            {
                ShowTip(msg);
            }
                break;
        }
    }

    void OnError(string msg)
    {
        ShowTip(msg);
    }

    #endregion

    public void ShowTip(string msg)
    {
        mTip.text = "C:/Inscryption/Online: " + msg;
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

    private void JumpToLobby()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<LobbyWnd>();
        WndManager.Instance.GetWnd<LobbyWnd>().ReDrawAnim();
    }

    private void JumpToRegister()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<RegisterWnd>();
        WndManager.Instance.GetWnd<RegisterWnd>().ReDrawAnim();
    }

    #endregion
}