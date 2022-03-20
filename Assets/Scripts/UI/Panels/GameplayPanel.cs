using UnityEngine;
using UnityEngine.UI;

public class GameplayPanel : UIPanel
{
    [Header("References - UI")]
    public Button settingsButton;
    public SettingsPopup settingsPopup;

    private void Awake()
    {
        settingsPopup.Hide();

        settingsButton.onClick.AddListener(SettingsButtonClicked);
    }

    private void SettingsButtonClicked()
    {
        settingsPopup.Show();
        Time.timeScale = 0f;
    }
}
