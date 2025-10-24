using UnityEngine;

public class GravitySystem2D : MonoBehaviour
{
   private RadialGravityWell2D[] _wells;
    private PhysicsBody2D[] _bodies;

    private void Update()
    {
     _wells = FindObjectsByType<RadialGravityWell2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    _bodies = FindObjectsByType<PhysicsBody2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

    foreach (var b in _bodies)
    {
        if (b.isStaticBody) continue;

        Vector2 total = Vector2.zero;
        foreach (var w in _wells)
            total += w.ForceOn(b);

        // Atenuar por estado de fluido
        var fluid = b.GetComponent<FluidState2D>();
        float gMul = fluid ? fluid.currentGravityMultiplier : 1f;
        total *= gMul;

        if (total != Vector2.zero)
            b.AddForce(total);
    }
    }
}
