using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class HTTPHandler : MonoBehaviour
{
    public static HTTPHandler Instance;

    private void Start()
    {
        Instance = this;
    }

    public void GET(string resource, UnityAction<string> OnResponse,UnityAction<string> OnError)
    {
        StartCoroutine(GETCoroutine(resource,OnResponse,OnError));
    }

    IEnumerator GETCoroutine(string resource,UnityAction<string> OnResponse,UnityAction<string> OnError)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(resource);
        yield return unityWebRequest;
        if (string.IsNullOrEmpty(unityWebRequest.error))
        {
            string result = unityWebRequest.downloadHandler.text;
            OnResponse(result);
        }
        else
        {
            OnError(unityWebRequest.error);
        }
    }

    public void POST(string resource,Dictionary<string,string> formData, UnityAction<string> OnResponse, UnityAction<string> OnError)
    {
        StartCoroutine(POSTCoroutine(resource, formData, OnResponse, OnError));
    }

    IEnumerator POSTCoroutine(string resource,Dictionary<string,string> formData, UnityAction<string> OnResponse, UnityAction<string> OnError)
    {
        List<IMultipartFormSection> sections = new List<IMultipartFormSection>();
        foreach (var item in formData)
        {
            sections.Add(new MultipartFormDataSection(item.Key, item.Value));
        }
        
        UnityWebRequest unityWebRequest = UnityWebRequest.Post(ConfigManager.Instance.ConfigInfo.httpserver + resource,sections);
        
        yield return unityWebRequest.SendWebRequest();
        if (string.IsNullOrEmpty(unityWebRequest.error))
        {
            string result = unityWebRequest.downloadHandler.text;
            OnResponse(result);
        }
        else
        {
            OnError(unityWebRequest.error);
        }
    }
}