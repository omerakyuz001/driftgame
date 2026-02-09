using UnityEngine;

[RequireComponent(typeof(TrackManager))]
public class TrackVisualizer : MonoBehaviour
{
    [Header("Visuals")]
    public Color outerColor = new Color(0.2f, 1f, 0.9f, 1f);
    public Color innerColor = new Color(1f, 0.2f, 0.9f, 1f);
    public float lineWidth = 0.15f;
    public int segments = 140;

    private LineRenderer outerLine;
    private LineRenderer innerLine;
    private TrackManager track;

    private void Awake()
    {
        track = GetComponent<TrackManager>();
        CreateLines();
        DrawRing();
    }

    private void CreateLines()
    {
        outerLine = CreateLineRenderer("OuterRing", outerColor, lineWidth);
        innerLine = CreateLineRenderer("InnerRing", innerColor, lineWidth * 0.9f);
    }

    private LineRenderer CreateLineRenderer(string name, Color color, float width)
    {
        var lineObject = new GameObject(name);
        lineObject.transform.SetParent(transform, false);
        var line = lineObject.AddComponent<LineRenderer>();
        line.loop = true;
        line.useWorldSpace = false;
        line.positionCount = segments;
        line.startWidth = width;
        line.endWidth = width;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = color;
        line.endColor = color;
        return line;
    }

    private void DrawRing()
    {
        for (int i = 0; i < segments; i++)
        {
            var angle = i / (float)segments * Mathf.PI * 2f;
            var outerPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * track.outerRadius;
            var innerPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * track.innerRadius;
            outerLine.SetPosition(i, outerPosition);
            innerLine.SetPosition(i, innerPosition);
        }
    }
}
