using System.Collections;
using UnityEngine;

public class LoadingPanel : UIPanel
{
    [Header("Settings")]
    public float loadingDuration = 3f;
    public float extendedDuration = 3f;

    public delegate void LoadingFinishedHandler(bool extended);
    public event LoadingFinishedHandler OnLoadingFinished;

    protected override void OnShowPanel()
    {
        base.OnShowPanel();

        StartCoroutine(WaitLoadingDuration(false));
    }

    public void ExtendLoading()
    {
        StartCoroutine(WaitLoadingDuration(true));
    }

    private IEnumerator WaitLoadingDuration(bool extended)
    {
        float waitDuration = extended ? extendedDuration : loadingDuration;
        yield return new WaitForSeconds(waitDuration);

        OnLoadingFinished?.Invoke(extended);
    }
}
