using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Text finalScoreText;
    public Button restartButton;
    public Button continueButton;

    private Action onRestart;
    private Action onContinue;

    public void Initialize()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() => onRestart?.Invoke());
        }
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(() => onContinue?.Invoke());
        }
    }

    public void Show(float score, Action restartAction, Action continueAction)
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score\n{Mathf.FloorToInt(score)}";
        }
        onRestart = restartAction;
        onContinue = continueAction;
        SetVisible(true);
    }

    public void Hide()
    {
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup == null)
        {
            return;
        }
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;
    }
}
