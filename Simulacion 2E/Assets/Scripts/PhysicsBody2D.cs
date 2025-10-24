using UnityEngine;

public class PhysicsBody2D : MonoBehaviour
{
  [Header("Dinámica")]
    public bool isStaticBody = false;
    public float mass = 1f;
    public float restitution = 0.6f;
    public float linearDamping = 0.02f;
    public bool useGravity = false;
    public float gravityScale = 1f;

    [Header("Estado")]
    public Vector2 velocity;
    private Vector2 _forces;

    public float InvMass => isStaticBody || mass <= 0f ? 0f : 1f / mass;

    public void AddForce(Vector2 f)
    {
        if (!isStaticBody) _forces += f;
    }

    public void ClearForces() => _forces = Vector2.zero;

    private void Update()
    {
        if (isStaticBody) return;

        float dt = Time.deltaTime;
        Vector2 g = useGravity ? new Vector2(0f, -9.81f * gravityScale) : Vector2.zero;

        Vector2 acc = _forces * InvMass + g;
        velocity += acc * dt;
        velocity *= Mathf.Clamp01(1f - linearDamping * dt);
        transform.position += (Vector3)(velocity * dt);

        ClearForces();
    }
}
