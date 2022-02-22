using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Magiclab.UI.Widgets;

public class FinishFailPanel : UIPanel
{
    [Header("References - UI")]
    public Button retryButton;
    public Image emojiImage;

    private Sequence _emojiSequence;
    private Vector3 _emojiStartPos;

    private void Awake()
    {
        retryButton.onClick.AddListener(RetryButtonClicked);

        _emojiStartPos = emojiImage.transform.localPosition;
    }

    private void RetryButtonClicked()
    {
        GameManager.RetryGameplay();
    }

    protected override void OnShowPanel()
    {
        base.OnShowPanel();

        _emojiSequence = DOTween.Sequence();
        _emojiSequence.Append(emojiImage.rectTransform.DOLocalMoveY(_emojiStartPos.y + 100, 1.5f).SetEase(Ease.InOutQuad));
        _emojiSequence.Append(emojiImage.rectTransform.DOLocalMoveY(_emojiStartPos.y, 1.5f).SetEase(Ease.InOutQuad));
        _emojiSequence.SetLoops(-1);
        _emojiSequence.Play();
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();

        _emojiSequence?.Kill();
        _emojiSequence = null;

        emojiImage.transform.localPosition = _emojiStartPos;
        emojiImage.rectTransform.localRotation = Quaternion.identity;
        emojiImage.rectTransform.localScale = Vector3.one;
    }
}
