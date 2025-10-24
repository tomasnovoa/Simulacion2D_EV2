using UnityEngine;

public class Circle2D : MonoBehaviour
{
     public float radius = 0.5f;
    public Vector2 Center => transform.position;

    public static bool Overlaps(Circle2D a, Circle2D b)
    {
        float r = a.radius + b.radius;
        return (a.Center - b.Center).sqrMagnitude <= r * r;
    }

    public static bool Overlaps(Circle2D c, AABB2D b)
    {
        Vector2 p = c.Center;
        Vector2 q = new Vector2(
            Mathf.Clamp(p.x, b.Min.x, b.Max.x),
            Mathf.Clamp(p.y, b.Min.y, b.Max.y)
        );
        return (p - q).sqrMagnitude <= c.radius * c.radius;
    }
}
