using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (FindObjectOfType<GameBootstrap>() != null)
        {
            return;
        }

        var bootstrapObject = new GameObject("GameBootstrap");
        bootstrapObject.AddComponent<GameBootstrap>();
    }

    private void Awake()
    {
        SetupCamera();
        var track = CreateTrack();
        var steering = CreateUi(out var hud, out var gameOverScreen);
        var car = CreateCar(track, steering);
        var monetization = CreateMonetization();
        var manager = gameObject.AddComponent<GameManager>();
        manager.car = car;
        manager.track = track;
        manager.steering = steering;
        manager.hud = hud;
        manager.gameOverScreen = gameOverScreen;
        manager.monetization = monetization;

        if (gameOverScreen != null)
        {
            gameOverScreen.Initialize();
        }
    }

    private void SetupCamera()
    {
        var camera = Camera.main;
        if (camera == null)
        {
            var cameraObject = new GameObject("Main Camera");
            camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }

        camera.orthographic = true;
        camera.orthographicSize = 6.8f;
        camera.transform.position = new Vector3(0f, 0f, -10f);
        camera.backgroundColor = new Color(0.04f, 0.05f, 0.08f, 1f);
    }

    private TrackManager CreateTrack()
    {
        var trackObject = new GameObject("NeonTrack");
        var track = trackObject.AddComponent<TrackManager>();
        var visualizer = trackObject.AddComponent<TrackVisualizer>();
        visualizer.outerColor = new Color(0.2f, 1f, 0.9f, 1f);
        visualizer.innerColor = new Color(1f, 0.2f, 0.9f, 1f);
        return track;
    }

    private DriftCarController CreateCar(TrackManager track, SteeringWheelInput steering)
    {
        var carObject = new GameObject("DriftCar");
        var spriteRenderer = carObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = NeonSpriteFactory.GetSquareSprite();
        spriteRenderer.color = new Color(1f, 0.35f, 0.85f, 1f);
        carObject.transform.localScale = new Vector3(0.35f, 0.6f, 1f);

        var smokeObject = new GameObject("DriftSmoke");
        smokeObject.transform.SetParent(carObject.transform, false);
        smokeObject.transform.localPosition = new Vector3(0f, -0.3f, 0f);
        var smoke = smokeObject.AddComponent<ParticleSystem>();
        ConfigureSmoke(smoke);

        var controller = carObject.AddComponent<DriftCarController>();
        controller.Initialize(track, steering);
        return controller;
    }

    private void ConfigureSmoke(ParticleSystem smoke)
    {
        var main = smoke.main;
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.2f, 1f, 0.9f, 0.7f),
            new Color(1f, 0.2f, 0.9f, 0.7f));
        main.startSize = 0.25f;
        main.startLifetime = 0.6f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = smoke.emission;
        emission.rateOverTime = 28f;

        var shape = smoke.shape;
        shape.angle = 18f;
        shape.radius = 0.05f;

        var colorOverLifetime = smoke.colorOverLifetime;
        colorOverLifetime.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(0.7f, 0f), new GradientAlphaKey(0f, 1f) });
        colorOverLifetime.color = gradient;
    }

    private MonetizationManager CreateMonetization()
    {
        return gameObject.AddComponent<MonetizationManager>();
    }

    private SteeringWheelInput CreateUi(out UIHud hud, out GameOverScreen gameOverScreen)
    {
        hud = null;
        gameOverScreen = null;

        var canvasObject = new GameObject("UI");
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        canvasObject.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<EventSystem>() == null)
        {
            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        var hudObject = new GameObject("HUD");
        hudObject.transform.SetParent(canvasObject.transform, false);
        var hudRect = hudObject.AddComponent<RectTransform>();
        hudRect.anchorMin = new Vector2(0.5f, 1f);
        hudRect.anchorMax = new Vector2(0.5f, 1f);
        hudRect.pivot = new Vector2(0.5f, 1f);
        hudRect.anchoredPosition = new Vector2(0f, -80f);
        hudRect.sizeDelta = new Vector2(600f, 220f);

        var speedText = CreateText(hudObject.transform, "SpeedText", 30, TextAnchor.UpperCenter);
        speedText.rectTransform.anchoredPosition = new Vector2(0f, -10f);

        var rpmBackground = CreateImage(hudObject.transform, "RPMBackground", new Color(0.1f, 0.2f, 0.25f, 0.8f));
        rpmBackground.rectTransform.sizeDelta = new Vector2(360f, 18f);
        rpmBackground.rectTransform.anchoredPosition = new Vector2(0f, -70f);
        rpmBackground.type = Image.Type.Sliced;

        var rpmFill = CreateImage(rpmBackground.transform, "RPMFill", new Color(0.3f, 1f, 0.9f, 1f));
        rpmFill.rectTransform.anchorMin = new Vector2(0f, 0f);
        rpmFill.rectTransform.anchorMax = new Vector2(1f, 1f);
        rpmFill.rectTransform.offsetMin = Vector2.zero;
        rpmFill.rectTransform.offsetMax = Vector2.zero;
        rpmFill.type = Image.Type.Filled;
        rpmFill.fillMethod = Image.FillMethod.Horizontal;
        rpmFill.fillOrigin = 0;

        var scoreText = CreateText(hudObject.transform, "ScoreText", 26, TextAnchor.UpperCenter);
        scoreText.rectTransform.anchoredPosition = new Vector2(0f, -110f);

        hud = hudObject.AddComponent<UIHud>();
        hud.scoreText = scoreText;
        hud.speedText = speedText;
        hud.rpmFill = rpmFill;

        var steeringObject = new GameObject("SteeringWheel");
        steeringObject.transform.SetParent(canvasObject.transform, false);
        var steeringRect = steeringObject.AddComponent<RectTransform>();
        steeringRect.anchorMin = new Vector2(0.5f, 0f);
        steeringRect.anchorMax = new Vector2(0.5f, 0f);
        steeringRect.pivot = new Vector2(0.5f, 0.5f);
        steeringRect.anchoredPosition = new Vector2(0f, 180f);
        steeringRect.sizeDelta = new Vector2(240f, 240f);

        var steeringImage = steeringObject.AddComponent<Image>();
        steeringImage.sprite = NeonSpriteFactory.GetRingSprite(128, 10f);
        steeringImage.color = new Color(0.2f, 1f, 1f, 0.7f);

        var steeringInput = steeringObject.AddComponent<SteeringWheelInput>();
        steeringInput.wheelImage = steeringImage;

        gameOverScreen = CreateGameOverScreen(canvasObject.transform);

        return steeringInput;
    }

    private GameOverScreen CreateGameOverScreen(Transform parent)
    {
        var panelObject = new GameObject("GameOverPanel");
        panelObject.transform.SetParent(parent, false);
        var panelRect = panelObject.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        var panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.75f);

        var canvasGroup = panelObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        var titleText = CreateText(panelObject.transform, "FinalScore", 40, TextAnchor.MiddleCenter);
        titleText.rectTransform.anchoredPosition = new Vector2(0f, 120f);

        var restartButton = CreateButton(panelObject.transform, "RestartButton", "Restart", new Vector2(0f, -20f));
        var continueButton = CreateButton(panelObject.transform, "ContinueButton", "Continue (Ad)", new Vector2(0f, -120f));

        var screen = panelObject.AddComponent<GameOverScreen>();
        screen.canvasGroup = canvasGroup;
        screen.finalScoreText = titleText;
        screen.restartButton = restartButton;
        screen.continueButton = continueButton;
        return screen;
    }

    private Text CreateText(Transform parent, string name, int fontSize, TextAnchor alignment)
    {
        var textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        var text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = new Color(0.85f, 0.95f, 1f, 1f);
        text.text = name;
        var rectTransform = text.rectTransform;
        rectTransform.sizeDelta = new Vector2(500f, 60f);
        return text;
    }

    private Image CreateImage(Transform parent, string name, Color color)
    {
        var imageObject = new GameObject(name);
        imageObject.transform.SetParent(parent, false);
        var image = imageObject.AddComponent<Image>();
        image.sprite = NeonSpriteFactory.GetSquareSprite();
        image.color = color;
        var rectTransform = image.rectTransform;
        rectTransform.sizeDelta = new Vector2(300f, 20f);
        return image;
    }

    private Button CreateButton(Transform parent, string name, string label, Vector2 position)
    {
        var buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);
        var image = buttonObject.AddComponent<Image>();
        image.sprite = NeonSpriteFactory.GetSquareSprite();
        image.color = new Color(0.2f, 0.9f, 1f, 0.9f);

        var button = buttonObject.AddComponent<Button>();
        var rect = buttonObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(320f, 70f);
        rect.anchoredPosition = position;

        var text = CreateText(buttonObject.transform, "Label", 26, TextAnchor.MiddleCenter);
        text.rectTransform.sizeDelta = rect.sizeDelta;
        text.text = label;
        text.color = new Color(0.02f, 0.05f, 0.08f, 1f);

        return button;
    }
}
