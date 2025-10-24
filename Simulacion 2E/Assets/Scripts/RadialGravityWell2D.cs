using UnityEngine;

public class RadialGravityWell2D : MonoBehaviour
{
   public float mass = 50f;         // masa equivalente del pozo
    public float G = 20f;            // constante de gravedad efectiva
    public float minRadius = 0.5f;   // evita singularidad
    public float influenceRadius = 10f;

    public Vector2 Center => transform.position;

    public Vector2 ForceOn(PhysicsBody2D body)
    {
        Vector2 r = (Vector2)Center - (Vector2)body.transform.position;
        float dist = Mathf.Max(minRadius, r.magnitude);
        if (dist > influenceRadius) return Vector2.zero;
        Vector2 dir = r / dist;
        float f = G * mass * body.mass / (dist * dist);
        return dir * f;
    }
}
