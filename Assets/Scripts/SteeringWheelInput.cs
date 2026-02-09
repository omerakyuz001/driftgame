using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SteeringWheelInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Steering")]
    public float maxSteeringAngle = 110f;

    [Header("Visuals")]
    public Image wheelImage;
    public Color idleColor = new Color(0.2f, 1f, 1f, 0.8f);
    public Color activeColor = new Color(1f, 1f, 1f, 1f);

    public float CurrentAngle { get; private set; }
    public float NormalizedAngle => Mathf.Clamp01(Mathf.Abs(CurrentAngle) / maxSteeringAngle);
    public bool InputEnabled { get; set; } = true;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (wheelImage != null)
        {
            wheelImage.color = idleColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!InputEnabled)
        {
            return;
        }

        UpdateAngle(eventData.position);
        if (wheelImage != null)
        {
            wheelImage.color = activeColor;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!InputEnabled)
        {
            return;
        }

        UpdateAngle(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!InputEnabled)
        {
            return;
        }

        CurrentAngle = 0f;
        UpdateWheelRotation();
        if (wheelImage != null)
        {
            wheelImage.color = idleColor;
        }
    }

    private void UpdateAngle(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, null, out var localPoint);
        var angle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;
        angle = Mathf.DeltaAngle(0f, angle);
        CurrentAngle = Mathf.Clamp(angle, -maxSteeringAngle, maxSteeringAngle);
        UpdateWheelRotation();
    }

    private void UpdateWheelRotation()
    {
        if (wheelImage != null)
        {
            wheelImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -CurrentAngle);
        }
    }
}
