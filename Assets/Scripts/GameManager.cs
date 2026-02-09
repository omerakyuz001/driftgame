using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DriftCarController car;
    public TrackManager track;
    public SteeringWheelInput steering;
    public UIHud hud;
    public GameOverScreen gameOverScreen;
    public MonetizationManager monetization;

    private bool isGameOver;
    private float score;
    private int runCount;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        score += Time.deltaTime;
        if (hud != null)
        {
            hud.UpdateHud(score, car != null ? car.CurrentSpeed : 0f, car != null ? car.RpmNormalized : 0f);
        }
    }

    public void OnCarCrashed()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        if (car != null)
        {
            car.SetDriving(false);
        }
        if (steering != null)
        {
            steering.InputEnabled = false;
        }
        if (gameOverScreen != null)
        {
            gameOverScreen.Show(score, RestartRun, ContinueRun);
        }
    }

    private void RestartRun()
    {
        runCount++;
        if (monetization != null)
        {
            monetization.TryShowInterstitial(runCount);
        }
        score = 0f;
        ResumeRun(false);
    }

    private void ContinueRun()
    {
        if (monetization == null)
        {
            ResumeRun(true);
            return;
        }

        monetization.ShowRewarded(() => ResumeRun(true));
    }

    private void ResumeRun(bool keepScore)
    {
        if (!keepScore)
        {
            score = 0f;
        }
        isGameOver = false;
        if (car != null)
        {
            car.ResetCar(true);
            car.SetDriving(true);
        }
        if (steering != null)
        {
            steering.InputEnabled = true;
        }
        if (gameOverScreen != null)
        {
            gameOverScreen.Hide();
        }
    }
}
