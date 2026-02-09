using UnityEngine;

public class DriftCarController : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 3.6f;
    public float speedAmplitude = 1.4f;
    public float speedCycleRate = 0.5f;
    public float outwardDrift = 0.8f;
    public float inwardDrift = -1.1f;

    [Header("References")]
    public TrackManager track;
    public SteeringWheelInput steering;

    public float CurrentSpeed { get; private set; }
    public float RpmNormalized { get; private set; }

    private float angle;
    private float radius;
    private bool canDrive = true;

    public void Initialize(TrackManager trackManager, SteeringWheelInput steeringInput)
    {
        track = trackManager;
        steering = steeringInput;
        ResetCar(false);
    }

    private void Update()
    {
        if (!canDrive || track == null)
        {
            return;
        }

        var speedOffset = Mathf.Sin(Time.time * speedCycleRate * Mathf.PI * 2f);
        CurrentSpeed = baseSpeed + speedAmplitude * speedOffset;
        var maxSpeed = baseSpeed + speedAmplitude;
        var minSpeed = Mathf.Max(0.1f, baseSpeed - speedAmplitude);
        RpmNormalized = Mathf.InverseLerp(minSpeed, maxSpeed, CurrentSpeed);

        var steeringNormalized = steering != null ? steering.NormalizedAngle : 0.5f;
        var driftVelocity = Mathf.Lerp(outwardDrift, inwardDrift, steeringNormalized);
        radius += driftVelocity * Time.deltaTime * Mathf.Lerp(0.8f, 1.2f, RpmNormalized);

        radius = Mathf.Clamp(radius, track.innerRadius - 0.4f, track.outerRadius + 0.4f);
        angle += (CurrentSpeed / Mathf.Max(0.1f, radius)) * Time.deltaTime;

        var position = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        transform.position = position;
        var tangent = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));
        transform.up = tangent;

        if (track.IsOutsideRing(position))
        {
            canDrive = false;
            SendMessageUpwards("OnCarCrashed", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void ResetCar(bool keepAngle)
    {
        canDrive = true;
        radius = track != null ? (track.innerRadius + track.outerRadius) * 0.5f : 3.5f;
        if (!keepAngle)
        {
            angle = 0f;
        }
        var position = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        transform.position = position;
    }

    public void SetDriving(bool enabled)
    {
        canDrive = enabled;
    }
}
