using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Magiclab.MarketingSDK.Core;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;

    [Header("References - Panels")]
    public LoadingPanel loadingPanel;
    public MenuPanel menuPanel;
    public GameplayPanel gameplayPanel;
    public FinishSuccessPanel finishSuccessPanel;
    public FinishFailPanel finishFailPanel;

    private List<UIPanel> allPanels = new List<UIPanel>();

    private void Awake()
    {
        allPanels.Add(loadingPanel);
        allPanels.Add(menuPanel);
        allPanels.Add(gameplayPanel);
        allPanels.Add(finishSuccessPanel);
        allPanels.Add(finishFailPanel);

        HideAllPanels(true);

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        loadingPanel.OnLoadingFinished += LoadingPanel_OnLoadingFinished;
    }

    private void LoadingPanel_OnLoadingFinished(bool extended)
    {
        if (!extended && !MarketingSDK.RemoteConfig.IsRemoteFetched)
        {
            loadingPanel.ExtendLoading();
        }
        else if (extended || MarketingSDK.RemoteConfig.IsRemoteFetched)
        {
            foreach (var panel in allPanels)
            {
                panel.Initialize(gameManager);
            }

            gameManager.InitializeAfterLoading();
        }
    }

    private void GameManager_OnGameStateChanged(GameState oldGameState, GameState newGameState)
    {
        if (newGameState == GameState.Loading)
        {
            ShowPanel(loadingPanel);
        }
        else if (newGameState == GameState.Menu)
        {
            ShowPanel(menuPanel);
        }
        else if (newGameState == GameState.Gameplay)
        {
            ShowPanel(gameplayPanel);
        }
        else if (newGameState == GameState.FinishSuccess)
        {
            ShowPanel(finishSuccessPanel);
        }
        else if (newGameState == GameState.FinishFail)
        {
            ShowPanel(finishFailPanel);
        }
        else
        {
            HideAllPanels();
        }
    }

    private void HideAllPanels(bool forceHide = false)
    {
        foreach (var panel in allPanels)
        {
            if (panel.IsShown || forceHide)
                panel.HidePanel();
        }
    }

    private void ShowPanel(UIPanel panel)
    {
        HideAllPanels();

        panel.ShowPanel();
    }
}
