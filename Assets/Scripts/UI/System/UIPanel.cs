using System;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class UIPanel : MonoBehaviour
{
    private Canvas _canvas;
    public Canvas Canvas
    {
        get
        {
            if (_canvas == null)
                _canvas = this.GetComponent<Canvas>();

            return _canvas;
        }
    }

    public bool IsShown { get; private set; }

    protected GameManager GameManager { get; private set; }

    public event Action OnPanelShown;
    public event Action OnPanelHidden;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;
        OnInitialize();
    }

    public void HidePanel()
    {
        IsShown = false;
        OnHidePanel();
        OnPanelHidden?.Invoke();
    }

    public void ShowPanel()
    {
        IsShown = true;
        OnShowPanel();
        OnPanelShown?.Invoke();
    }

    protected virtual void OnInitialize()
    {

    }

    protected virtual void OnHidePanel()
    {
        Canvas.enabled = false;
    }

    protected virtual void OnShowPanel()
    {
        Canvas.enabled = true;
    }
}
