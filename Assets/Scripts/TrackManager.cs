using UnityEngine;

public class TrackManager : MonoBehaviour
{
    [Header("Ring Settings")]
    public float innerRadius = 2.4f;
    public float outerRadius = 4.6f;

    public Vector2 Center => Vector2.zero;

    public bool IsOutsideRing(Vector2 position)
    {
        var distance = Vector2.Distance(position, Center);
        return distance <= innerRadius || distance >= outerRadius;
    }

    public float ClampRadius(float radius)
    {
        return Mathf.Clamp(radius, innerRadius + 0.05f, outerRadius - 0.05f);
    }
}
