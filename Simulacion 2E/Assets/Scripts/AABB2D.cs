using UnityEngine;

public class AABB2D : MonoBehaviour
{
   public Vector2 halfSize = new Vector2(0.5f, 0.5f);

    public Vector2 Center => transform.position;
    public Vector2 Min => Center - halfSize;
    public Vector2 Max => Center + halfSize;

    public static bool Overlaps(AABB2D a, AABB2D b)
    {
        return !(a.Max.x < b.Min.x || a.Min.x > b.Max.x || a.Max.y < b.Min.y || a.Min.y > b.Max.y);
    }

    public static Vector2 GetMTV(AABB2D a, AABB2D b)
    {
        float dx1 = b.Max.x - a.Min.x;
        float dx2 = a.Max.x - b.Min.x;
        float dy1 = b.Max.y - a.Min.y;
        float dy2 = a.Max.y - b.Min.y;

        float mx = (dx1 < dx2) ? dx1 : -dx2;
        float my = (dy1 < dy2) ? dy1 : -dy2;

        return Mathf.Abs(mx) < Mathf.Abs(my) ? new Vector2(mx, 0f) : new Vector2(0f, my);
    }
}
