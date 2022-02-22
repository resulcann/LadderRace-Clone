using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class GameplayController : MonoBehaviour
{
    public bool IsActive { get; private set; }

    public Action<bool> OnGameplayFinished;

    public void StartGameplay()
    {
        IsActive = true;
    }

    public void RetryGameplay()
    {
        IsActive = true;
    }

    private void FinishGameplay(bool success)
    {
        IsActive = false;

        var hapticType = success ? HapticTypes.Success : HapticTypes.Failure;

        MMVibrationManager.Haptic(hapticType);
        OnGameplayFinished?.Invoke(success);
    }

    private void Update()
    {
        if (!IsActive)
            return;

        if (Input.GetKeyDown(KeyCode.U))
        {
            FinishGameplay(true);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            FinishGameplay(false);
        }
    }
}
