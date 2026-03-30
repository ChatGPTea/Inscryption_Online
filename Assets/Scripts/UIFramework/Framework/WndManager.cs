using System.Collections.Generic;
using UnityEngine;

public class WndManager : Singleton<WndManager>
{
    private Dictionary<string, BaseWnd> _allWnds;
    private Transform _canvas;


    public void Initial(Transform canvas)
    {
        _canvas = canvas;
        _allWnds = new Dictionary<string, BaseWnd>();
    }


    public void OpenWnd<T>() where T : BaseWnd, new()
    {
        var wndName = typeof(T).Name;
        if (_allWnds.ContainsKey(wndName))
        {
            _allWnds[wndName].OpenWnd();
        }
        else
        {
            var wnd = new T();
            wnd.CreateWnd(wndName, _canvas);
            wnd.Initial();
            _allWnds.Add(wndName, wnd);
        }
    }

    public void CloseWnd<T>() where T : BaseWnd, new()
    {
        var wndName = typeof(T).Name;
        if (_allWnds.ContainsKey(wndName)) _allWnds[wndName].CloseWnd();
    }

    public T GetWnd<T>() where T : BaseWnd, new()
    {
        var wndName = typeof(T).Name;
        if (_allWnds.ContainsKey(wndName)) return _allWnds[wndName] as T;
        return null;
    }

    public void DeleteWnd<T>() where T : BaseWnd, new()
    {
        var wndName = typeof(T).Name;
        if (_allWnds.ContainsKey(wndName))
        {
            _allWnds[wndName].DeleteWnd();
            _allWnds.Remove(wndName);
        }
    }
}