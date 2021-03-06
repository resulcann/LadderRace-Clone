using System;
using UnityEngine;
using Magiclab.MarketingSDK.Core;
using Magiclab.Utility.GenericUtilities;
using MoreMountains.NiceVibrations;
using TMPro;


public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [Header("References")]
    public GameplayController gameplayController;
    public DynamicSettings dynamicSettings;

    public GameState CurrentGameState { get; private set; }

    public bool IsLoadingFinished => CurrentGameState != GameState.None && CurrentGameState != GameState.Loading;
    public int numberOfCoins;
    

    [HideInInspector] public int aiSuccessPercentage = 50;
    public bool IsVibrationEnabled
    {
        get => PlayerPrefs.GetInt(PlayerPrefKeys.IsVibrationEnabled, 1) == 1;
        set 
        { 
            if (value != IsVibrationEnabled)
            {
                PlayerPrefs.SetInt(PlayerPrefKeys.IsVibrationEnabled, value ? 1 : 0);
                VibrationSettingChanged(value);
            }
        }
    }

    public bool IsMusicEnabled
    {
        get => PlayerPrefs.GetInt(PlayerPrefKeys.IsMusicEnabled, 1) == 1;
        set 
        { 
            if (value != IsMusicEnabled)
            {
                PlayerPrefs.SetInt(PlayerPrefKeys.IsMusicEnabled, value ? 1 : 0);
                MusicSettingChanged(value);
            }
        }
    }

    public event Action<GameState /*Old*/, GameState /*New*/> OnGameStateChanged;


    private void Awake()
    {
        Instance = this;
        
        SRDebug.Instance.PanelVisibilityChanged += SRDebug_PanelVisibilityChanged;

        numberOfCoins = PlayerPrefs.GetInt("NumberOfCoins", 0);
    }

    private void SRDebug_PanelVisibilityChanged(bool isVisible)
    {
        Time.timeScale = isVisible ? 0f : 1f;
    }

    private void Start()
    {
        gameplayController.OnGameplayFinished += GameplayController_OnGameplayFinished;
        dynamicSettings.OnTestFinished += DynamicSettings_OnTestFinished;

        VibrationSettingChanged(IsVibrationEnabled);
        MusicSettingChanged(IsMusicEnabled);

        ChangeCurrentGameState(GameState.Loading);
    }

    private void DynamicSettings_OnTestFinished(bool testPassed)
    {
        if (testPassed)
            return;

        QualitySettings.shadows = ShadowQuality.Disable;
    }

    private void VibrationSettingChanged(bool enabled)
    {
        MMVibrationManager.SetHapticsActive(enabled);
    }
    private void MusicSettingChanged(bool enabled)
    {
        Camera.main.GetComponent<AudioSource>().volume = enabled ? 0.1f : 0f;
    }

    public void ChangeCurrentGameState(GameState newGameState)
    {
        var oldGameState = CurrentGameState;
        CurrentGameState = newGameState;
        OnGameStateChanged?.Invoke(oldGameState, CurrentGameState);
    }

    public void InitializeAfterLoading()
    {
#if UNITY_EDITOR
        Application.targetFrameRate = 9999;
#else
        Application.targetFrameRate = RemoteConfigs.TargetFrameRate;
#endif

        MarketingSDK.RemoteConfig.SetLoadingCompleted();

        dynamicSettings.Initialize(RemoteConfigs.PerformanceMinFPS);
        SetRemoteValues();

        ChangeCurrentGameState(GameState.Menu);
    }

    private void SetRemoteValues()
    {
        
    }

    public void StartGameplay()
    {
        gameplayController.StartGameplay();

        ChangeCurrentGameState(GameState.Gameplay);
    }

    private void GameplayController_OnGameplayFinished(bool success)
    {
        var gameState = success ? GameState.FinishSuccess : GameState.FinishFail;
        ChangeCurrentGameState(gameState);
    }

    public void FullyFinishGameplay()
    {
        MarketingSDK.Ads.ShowInterstitial();

        ChangeCurrentGameState(GameState.Menu);
    }

    public void RetryGameplay()
    {
        gameplayController.RetryGameplay();

        ChangeCurrentGameState(GameState.Gameplay);
    }
}

public enum GameState
{
    None,
    Loading,
    Menu,
    Gameplay,
    FinishSuccess,
    FinishFail
}
