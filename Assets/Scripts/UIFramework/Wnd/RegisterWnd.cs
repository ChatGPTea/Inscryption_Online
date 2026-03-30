using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class RegisterWnd : BaseWnd
{

    /// <summary>
    ///     初始化UI窗口，获取相对应的组件
    /// </summary>
    private Button mExitBtn;
    private Button mCreateBtn;
    private Button mSendBtn;
    private TMP_InputField mRegisterUsername;
    private TMP_InputField mRegisterCode;
    private TMP_InputField mRegisterNickname;
    private TMP_InputField mRegisterPassword;
    private TMP_Text mTip;


    /// <summary>
    ///     Timeline组件
    /// </summary>
    private PlayableDirector mReDrawTimeline;
    
    private RegisterMono mRegisterMono;


    public override void Initial()
    {
        SelfTransform.gameObject.AddComponent<RegisterMono>();
        mRegisterMono = SelfTransform.GetComponent<RegisterMono>();
        //获取UI组件
        mExitBtn = SelfTransform.Find("ExitBtn").GetComponent<Button>();
        mCreateBtn = SelfTransform.Find("ConfirmBtn").GetComponent<Button>();
        mSendBtn = SelfTransform.Find("SendCodeBtn").GetComponent<Button>();
        mRegisterUsername = SelfTransform.Find("Account/AccountInput").GetComponent<TMP_InputField>();
        mRegisterCode = SelfTransform.Find("Code/CodeInput").GetComponent<TMP_InputField>();
        mRegisterNickname = SelfTransform.Find("NickName/NickNameInput").GetComponent<TMP_InputField>();
        mRegisterPassword = SelfTransform.Find("Password/PassWordInput").GetComponent<TMP_InputField>();
        mTip = SelfTransform.Find("Tip_1").GetComponent<TMP_Text>();

        //获取Timeline
        mReDrawTimeline = SelfTransform.Find("DirectorReDraw").GetComponent<PlayableDirector>();

        //绑定Button音效
        mExitBtn.gameObject.AddComponent<CommandButtonMono>();
        mCreateBtn.gameObject.AddComponent<CommandButtonMono>();
        mSendBtn.gameObject.AddComponent<CommandButtonMono>();

        //绑定Button跳转
        mExitBtn.onClick.AddListener(JumpToLogin);
        mCreateBtn.onClick.AddListener(OnConfirmClick);
        mSendBtn.onClick.AddListener(OnSendClick);
    }

    #region 网络相关:验证码

    bool IsValidEmail(string email)
    {
        string pattern =  @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(email);
    }
    
    private void OnSendClick()
    {
        if (mRegisterUsername.text == "")
        {
            ShowTip("邮箱不能为空");
            return;
        }
        
        bool isEmail = IsValidEmail(mRegisterUsername.text);

        if (!isEmail)
        {
            ShowTip("邮箱形式不合法");
            return;
        }
        
        mRegisterMono.SendCode();
        
        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("username", mRegisterUsername.text);
        HTTPHandler.Instance.POST("user/sendcode",formData,OnSendCodeRepose,OnSendCodeError);
    }

    void OnSendCodeRepose(string msg)
    {
        ServiceResult<string> result = JsonConvert.DeserializeObject<ServiceResult<string>>(msg);
        ShowTip(result.msg);
    }
    
    void OnSendCodeError(string msg)
    {
        ServiceResult<string> result = JsonConvert.DeserializeObject<ServiceResult<string>>(msg);
        ShowTip(result.msg);
    }

    #endregion

    #region 网络相关：注册

    private void OnConfirmClick()
    {
        if (mRegisterUsername.text == "" && mRegisterPassword.text == "" && mRegisterCode.text == "" &&
            mRegisterNickname.text == "")
        {
            ShowTip("注册前不能有信息为空");
            return;
        }
        Dictionary<string,string> formData = new Dictionary<string, string>();
        formData.Add("username", mRegisterUsername.text);
        formData.Add("password", mRegisterPassword.text);
        formData.Add("code",mRegisterCode.text);
        formData.Add("nickname", mRegisterNickname.text);
        HTTPHandler.Instance.POST("user/register",formData,OnRegisterResponse,OnRegisterError);
    }

    void OnRegisterResponse(string msg)
    {
        ServiceResult<string> result = JsonConvert.DeserializeObject<ServiceResult<string>>(msg);
        switch (result.code)
        {
            case 1001:
            {
                JumpToLogin();
            }
                break;
            case -1:
            {
                ShowTip(result.msg);
            }
                break;
            case 0:
            {
                ShowTip(result.msg);
            }
                break;
        }
    }

    void OnRegisterError(string msg)
    {
        Debug.Log(msg);
    }

    #endregion
    
    private void ShowTip(string msg)
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

    private void JumpToLogin()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<LoginWnd>();
        WndManager.Instance.GetWnd<LoginWnd>().ReDrawAnim();
    }

    private void JumpToRoom()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<LoginWnd>();
        WndManager.Instance.GetWnd<LoginWnd>().ReDrawAnim();
    }

    #endregion
}