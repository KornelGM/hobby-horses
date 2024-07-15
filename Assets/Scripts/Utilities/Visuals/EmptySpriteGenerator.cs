using UnityEngine;

public static class EmptySpriteGenerator
{    public static Sprite GenerateEmptySprite()
    {
        Texture2D texture = new Texture2D(64, 64);

        Color[] fillPixels = new Color[texture.width * texture.height];
        for (int i = 0; i < fillPixels.Length; i++)
        {
            fillPixels[i] = i % 4 == 0 ? Color.white : Color.grey;
        }
        texture.SetPixels(fillPixels);
        texture.Apply();

        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite sprite = Sprite.Create(texture, rect, pivot);

        return sprite;
    }
}
