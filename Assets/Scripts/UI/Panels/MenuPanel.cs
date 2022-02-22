using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : UIPanel
{
    [Header("References - UI")]
    public Button startButton;
    public Button settingsButton;
    public SettingsPopup settingsPopup;

    private void Awake()
    {
        settingsPopup.Hide();

        startButton.onClick.AddListener(StartButtonClicked);
        settingsButton.onClick.AddListener(SettingsButtonClicked);
    }

    private void StartButtonClicked()
    {
        GameManager.StartGameplay();
    }

    private void SettingsButtonClicked()
    {
        settingsPopup.Show();
    }
}
