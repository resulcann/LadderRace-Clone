using UnityEngine;
using UnityEngine.UI;
using Magiclab.MarketingSDK.Core;

[RequireComponent(typeof(CanvasGroup))]
public class SettingsPopup : MonoBehaviour
{
    [Header("References - UI")]
    public Button closeButton;
    public Button termsOfUseButton;
    public Button privacyPolicyButton;
    public Toggle vibrationToggle;

    private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
                _canvasGroup = this.GetComponent<CanvasGroup>();

            return _canvasGroup;
        }
    }

    public bool IsShown { get; private set; }

    private void Awake()
    {
        closeButton.onClick.AddListener(CloseButtonClicked);
        termsOfUseButton.onClick.AddListener(TermsOfUseButtonClicked);
        privacyPolicyButton.onClick.AddListener(PrivacyPolicyButtonClicked);
        vibrationToggle.onValueChanged.AddListener(VibrationToggleValueChanged);
    }

    public void Show()
    {
        vibrationToggle.isOn = GameManager.Instance.IsVibrationEnabled;

        CanvasGroup.alpha = 1f;
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;

        IsShown = true;
    }

    public void Hide()
    {
        CanvasGroup.alpha = 0f;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;

        IsShown = false;
    }

    private void CloseButtonClicked()
    {
        Hide();
    }

    private void TermsOfUseButtonClicked()
    {
        MarketingSDK.DataCollectionManager.OpenTermsOfUsePage();
    }

    private void PrivacyPolicyButtonClicked()
    {
        MarketingSDK.DataCollectionManager.OpenPrivacyPolicyPage();
    }

    private void VibrationToggleValueChanged(bool value)
    {
        if (!IsShown)
            return;

        GameManager.Instance.IsVibrationEnabled = value;
    }
}
