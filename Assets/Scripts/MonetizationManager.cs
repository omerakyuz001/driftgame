using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonetizationManager : MonoBehaviour
{
    [Header("Interstitial")]
    public int runsBetweenInterstitials = 3;

    [Header("Cosmetics")]
    public List<Color> carColors = new List<Color>
    {
        new Color(1f, 0.3f, 0.9f, 1f),
        new Color(0.3f, 1f, 0.9f, 1f),
        new Color(1f, 0.8f, 0.2f, 1f)
    };

    public void TryShowInterstitial(int runCount)
    {
        if (runsBetweenInterstitials <= 0)
        {
            return;
        }

        if (runCount % runsBetweenInterstitials == 0)
        {
            Debug.Log("[Ads] Showing interstitial placeholder.");
        }
    }

    public void ShowRewarded(Action onComplete)
    {
        StartCoroutine(MockRewardedRoutine(onComplete));
    }

    private IEnumerator MockRewardedRoutine(Action onComplete)
    {
        Debug.Log("[Ads] Playing rewarded ad placeholder.");
        yield return new WaitForSeconds(1.2f);
        onComplete?.Invoke();
    }
}
