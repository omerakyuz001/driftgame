using UnityEngine;
using UnityEngine.UI;

public class UIHud : MonoBehaviour
{
    public Text scoreText;
    public Text speedText;
    public Image rpmFill;

    public void UpdateHud(float score, float speed, float rpmNormalized)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score {Mathf.FloorToInt(score)}";
        }
        if (speedText != null)
        {
            speedText.text = $"Speed {speed:0.0}";
        }
        if (rpmFill != null)
        {
            rpmFill.fillAmount = Mathf.Clamp01(rpmNormalized);
        }
    }
}
