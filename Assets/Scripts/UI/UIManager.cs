using System;
using System.Collections.Generic;
using FairyGUI;

public class UIManager : SingleBase<UIManager>
{
    private UIManager() { }

    private Dictionary<string, GComponent> panels = new Dictionary<string, GComponent>();
    private Dictionary<string, object> controllers = new Dictionary<string, object>();
    private bool _isInitialized = false;

    private void Init()
    {
        GRoot.inst.SetContentScaleFactor(1284, 2778, UIContentScaler.ScreenMatchMode.MatchWidth);
        LoadPackage("All");
        LoadPackage("Other");
        All.AllBinder.BindAll();
        UIConfig.defaultFont = "QiuYeYuanTi-16";
        _isInitialized = true;
    }

    public void LoadPackage(string packageName)
    {
        UIPackage package = UIPackage.AddPackage("UI/" + packageName);
        // foreach (var dependency in package.dependencies)
        // {
        //     UIPackage.AddPackage(dependency["name"]);
        // }
    }

    /// <summary>
    /// 面板逻辑与导出内容不分离时使用
    /// </summary>
    public T ShowPanel<T>(string packageName) where T : GComponent
    {
        if (!_isInitialized)
            Init();

        string panelName = typeof(T).Name;
        LoadPackage(packageName);

        if (panels.ContainsKey(panelName))
        {
            T panel = panels[panelName] as T;
            panel.visible = true;
            return panel;
        }
        else
        {
            GComponent panel = UIPackage.CreateObject(packageName, panelName).asCom;

            GRoot.inst.AddChild(panel);
            panel.MakeFullScreen();
            panel.AddRelation(GRoot.inst, RelationType.Size);

            panels.Add(panelName, panel);
            panel.fairyBatching = true;
            return panel as T;
        }
    }

    /// <summary>
    /// 面板逻辑与导出内容分离时使用
    /// </summary>
    public (TView view, TCtrl ctrl) ShowPanel<TView, TCtrl>(string packageName)
        where TView : GComponent
        where TCtrl : class
    {
        TView view = ShowPanel<TView>(packageName);

        string key = typeof(TView).Name;
        if (!controllers.ContainsKey(key))
        {
            TCtrl ctrl = (TCtrl)Activator.CreateInstance(typeof(TCtrl), view);
            controllers.Add(key, ctrl);
        }

        return (view, controllers[key] as TCtrl);
    }

    public void HidePanel<T>(bool removeFromScene) where T : GComponent
    {
        string panelName = typeof(T).Name;

        if (!panels.ContainsKey(panelName))
            return;

        if (removeFromScene)
        {
            if (controllers.ContainsKey(panelName))
            {
                if (controllers[panelName] is IDisposable disposable)
                    disposable.Dispose();
                controllers.Remove(panelName);
            }

            panels[panelName].Dispose();
            panels.Remove(panelName);
        }
        else
            panels[panelName].visible = false;
    }
}