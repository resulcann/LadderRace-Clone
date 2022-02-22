using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Magiclab.UI.Widgets;

public class FinishSuccessPanel : UIPanel
{
    [Header("References - UI")]
    public Button doneButton;
    public Image emojiImage;
    public UIParticleEffect confettiEffect;

    private Sequence _emojiSequence;
    private Vector3 _emojiStartPos;

    private void Awake()
    {
        doneButton.onClick.AddListener(DoneButtonClicked);

        _emojiStartPos = emojiImage.transform.localPosition;
    }

    private void DoneButtonClicked()
    {
        GameManager.FullyFinishGameplay();
    }

    protected override void OnShowPanel()
    {
        base.OnShowPanel();

        _emojiSequence = DOTween.Sequence();
        _emojiSequence.Append(emojiImage.rectTransform.DOLocalMoveY(_emojiStartPos.y + 100, 0.5f).SetEase(Ease.InOutQuad));
        _emojiSequence.Append(emojiImage.rectTransform.DOPunchRotation(new Vector3(0f, 0f, 30f), 1f, 7, 1f));
        _emojiSequence.Insert(0.5f, emojiImage.rectTransform.DOScale(1.3f, 0.5f));
        _emojiSequence.Insert(1f, emojiImage.rectTransform.DOScale(1f, 0.5f));
        _emojiSequence.Append(emojiImage.rectTransform.DOLocalMoveY(_emojiStartPos.y, 0.5f).SetEase(Ease.InOutQuad));
        _emojiSequence.SetLoops(-1);
        _emojiSequence.Play();

        confettiEffect.Play();
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();

        _emojiSequence?.Kill();
        _emojiSequence = null;

        confettiEffect.Stop();

        emojiImage.transform.localPosition = _emojiStartPos;
        emojiImage.rectTransform.localRotation = Quaternion.identity;
        emojiImage.rectTransform.localScale = Vector3.one;
    }
}
