using UnityEngine;
using System.Collections.Generic;
public class CollisionManager2D : MonoBehaviour
{
    private List<AABB2D> _aabbs = new();
    private List<Circle2D> _circles = new();


    private void LateUpdate()
    {
        _aabbs.Clear(); _circles.Clear();
        _aabbs.AddRange(FindObjectsByType<AABB2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
        _circles.AddRange(FindObjectsByType<Circle2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));

        // AABB vs AABB
        for (int i = 0; i < _aabbs.Count; i++)
        for (int j = i + 1; j < _aabbs.Count; j++)
            HandleAABB(_aabbs[i], _aabbs[j]);

        // Circle vs Circle
        for (int i = 0; i < _circles.Count; i++)
        for (int j = i + 1; j < _circles.Count; j++)
            HandleCircleCircle(_circles[i], _circles[j]);

        // Circle vs AABB
        foreach (var c in _circles)
        foreach (var b in _aabbs)
            HandleCircleAABB(c, b);
    }
    private static void ApplyImpulse(PhysicsBody2D a, PhysicsBody2D b, Vector2 normal, float restitution)
    {
        Vector2 va = a ? a.velocity : Vector2.zero;
        Vector2 vb = b ? b.velocity : Vector2.zero;
        float invA = a ? a.InvMass : 0f;
        float invB = b ? b.InvMass : 0f;
        float invSum = invA + invB;
        if (invSum <= 0f) return;

        Vector2 rv = vb - va;
        float velN = Vector2.Dot(rv, normal);
        if (velN > 0f) return;

        float e = restitution;
        if (a) e = Mathf.Min(e, a.restitution);
        if (b) e = Mathf.Min(e, b.restitution);

        float j = -(1f + e) * velN / invSum;
        Vector2 impulse = j * normal;

        if (a && invA > 0f) a.velocity -= impulse * invA;
        if (b && invB > 0f) b.velocity += impulse * invB;
    }

private static void DistributeSeparation(Transform ta, Transform tb, Vector2 mtv, float invA, float invB)
{
    float invSum = invA + invB;
    if (invSum <= 0f) return;

    Vector3 sepA = (Vector3)(mtv * (invA / invSum));
    Vector3 sepB = (Vector3)(mtv * (invB / invSum));

    ta.position -= sepA;  // mover A en dirección opuesta
    tb.position += sepB;  // mover B en dirección MTV
}

    private static void HandleAABB(AABB2D a, AABB2D b)
    {
        if (!AABB2D.Overlaps(a, b)) return;
        var ba = a.GetComponent<PhysicsBody2D>();
        var bb = b.GetComponent<PhysicsBody2D>();

        Vector2 mtv = AABB2D.GetMTV(a, b);
        DistributeSeparation(a.transform, b.transform, mtv, ba ? ba.InvMass : 0f, bb ? bb.InvMass : 0f);
        Vector2 n = mtv.normalized;
        ApplyImpulse(ba, bb, n, 0.8f);
    }

    private static void HandleCircleCircle(Circle2D a, Circle2D b)
    {
        if (!Circle2D.Overlaps(a, b)) return;
        var ba = a.GetComponent<PhysicsBody2D>();
        var bb = b.GetComponent<PhysicsBody2D>();

        Vector2 d = (Vector2)(b.Center - a.Center);
        float dist = d.magnitude;
        if (dist <= 1e-5f) return;
        float pen = (a.radius + b.radius) - dist;
        Vector2 n = d / dist;

        DistributeSeparation(a.transform, b.transform, n * pen, ba ? ba.InvMass : 0f, bb ? bb.InvMass : 0f);
        ApplyImpulse(ba, bb, n, 0.8f);
    }

    private static void HandleCircleAABB(Circle2D c, AABB2D b)
    {
        if (!Circle2D.Overlaps(c, b)) return;
        var bc = c.GetComponent<PhysicsBody2D>();
        var bb = b.GetComponent<PhysicsBody2D>();

        Vector2 p = c.Center;
        Vector2 q = new Vector2(Mathf.Clamp(p.x, b.Min.x, b.Max.x), Mathf.Clamp(p.y, b.Min.y, b.Max.y));
        Vector2 d = p - q;
        float dist = d.magnitude;
        if (dist <= 1e-5f) return;
        float pen = c.radius - dist;
        Vector2 n = d / dist;

        DistributeSeparation(c.transform, b.transform, n * pen, bc ? bc.InvMass : 0f, bb ? bb.InvMass : 0f);
        ApplyImpulse(bc, bb, n, 0.8f);
    }
}
