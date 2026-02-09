using UnityEngine;

public static class NeonSpriteFactory
{
    private static Sprite squareSprite;
    private static Sprite ringSprite;

    public static Sprite GetSquareSprite()
    {
        if (squareSprite != null)
        {
            return squareSprite;
        }

        var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        squareSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        return squareSprite;
    }

    public static Sprite GetRingSprite(int size, float thickness)
    {
        if (ringSprite != null)
        {
            return ringSprite;
        }

        var texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Bilinear;
        var center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        var outerRadius = size * 0.5f;
        var innerRadius = outerRadius - thickness;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                var distance = Vector2.Distance(new Vector2(x, y), center);
                var alpha = distance <= outerRadius && distance >= innerRadius ? 1f : 0f;
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        texture.Apply();
        ringSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return ringSprite;
    }
}
