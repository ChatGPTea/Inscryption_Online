using System;
using Inscryption;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterMono : InscryptionController
{
    private Button mSendBtn;
    private TMP_Text mTip;
    
    private float mCooldownTime = 60f; // 冷却时间60秒
    private float mCurrentCooldown = 0f;
    private bool mIsCoolingDown = false;

    public void Start()
    {
        mSendBtn = GameObject.Find("SendCodeBtn").GetComponent<Button>();
        mTip = GameObject.Find("Tip_2").GetComponent<TextMeshProUGUI>();
    }
    
    public void SendCode()
    {
        mSendBtn.interactable = false;
        StartCooldown();
    }
    
    private void StartCooldown()
    {
        mIsCoolingDown = true;
        mCurrentCooldown = mCooldownTime;
    }
    
    private void Update()
    {
        if (mIsCoolingDown)
        {
            mCurrentCooldown -= Time.deltaTime;
            
            if (mCurrentCooldown <= 0f)
            {
                // 冷却结束
                mIsCoolingDown = false;
                mSendBtn.interactable = true;
                mTip.text = "C:/Inscryption/Online: 核验已重置，可再次发送验证码";
            }
            else
            {
                // 更新冷却显示
                int secondsLeft = Mathf.CeilToInt(mCurrentCooldown);
                mTip.text = $"C:/Inscryption/Online: 验证码冷却中... <color=#D7E2A3>{secondsLeft}秒</color>后可再次尝试";
            }
        }
    }
}