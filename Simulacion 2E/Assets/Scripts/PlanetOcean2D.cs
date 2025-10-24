using UnityEngine;

public class PlanetOcean2D : MonoBehaviour
{
     [Header("Agua (banda circular)")]
    public float waterRadius = 4f;        // radio exterior de la banda de agua
    public float waterThickness = 0.6f;   // grosor de la banda de agua

    [Header("Flotabilidad")]
    public float fluidDensity = 1f;   // escala de empuje
    public float buoyancyG = 9.81f;   // "g" efectivo del fluido
    public float drag = 0.7f;         // rozamiento en el fluido
    public string affectsTag = "";    // vacío = todos, o por ejemplo "Buoyant"

    private void Update()
    {
        // Busca todos los cuerpos físicos manuales de la escena
        var bodies = FindObjectsByType<PhysicsBody2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var body in bodies)
        {
            if (body == null || body.isStaticBody) continue;
            if (!string.IsNullOrEmpty(affectsTag) && !body.CompareTag(affectsTag)) continue;

            var circle = body.GetComponent<Circle2D>();
            if (circle == null) continue; // este modelo asume cuerpos circulares

            ApplyBuoyancy(body, circle);
        }
    }

    private void ApplyBuoyancy(PhysicsBody2D body, Circle2D circle)
    {
    Vector2 center = transform.position;
    Vector2 pos = body.transform.position;
    float d = Vector2.Distance(pos, center);
    float Rb = Mathf.Max(1e-4f, circle.radius);

    float outer = Mathf.Max(0f, waterRadius);
    float inner = Mathf.Max(0f, waterRadius - Mathf.Max(0f, waterThickness));

    float segMin = Mathf.Max(inner, d - Rb);
    float segMax = Mathf.Min(outer, d + Rb);
    float overlap = Mathf.Max(0f, segMax - segMin);
    float fraction = Mathf.Clamp01(overlap / (2f * Rb));
    if (fraction <= 0f) return;

    Vector2 up = (pos - center);
    float dist = up.magnitude;
    if (dist < 1e-4f) return;
    up /= dist;

    // Empuje radial (hacia afuera) + drag lineal y cuadrático
    Vector2 v = body.velocity;
    Vector2 F_buoy = up * (fluidDensity * buoyancyG * body.mass * fraction);
    Vector2 F_dlin = -v * drag * fraction;
    float quadraticDrag = 0.6f; // expuesto si prefieres
    Vector2 F_dquad = -v * v.magnitude * quadraticDrag * fraction;

    // Opcional: "tensión superficial" hacia la línea media del anillo
    float bandMid = 0.5f * (outer + inner);
    float k_surface = 0f; // 2..6 para pegarse más a la superficie
    float x = d - bandMid;
    Vector2 F_surface = -k_surface * x * up * fraction;

    body.AddForce(F_buoy + F_dlin + F_dquad + F_surface);

    // Avisar al sistema de gravedad que este cuerpo está en fluido
    var fluid = body.GetComponent<FluidState2D>();
    if (fluid == null) fluid = body.gameObject.AddComponent<FluidState2D>();
    fluid.PingInFluid();
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja el anillo de agua para depuración
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.6f);
        DrawCircle(transform.position, waterRadius);
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.25f);
        DrawCircle(transform.position, Mathf.Max(0f, waterRadius - waterThickness));
    }

    private void DrawCircle(Vector3 c, float r, int segments = 64)
    {
        if (r <= 0f) return;
        Vector3 prev = c + new Vector3(r, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float ang = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 next = c + new Vector3(Mathf.Cos(ang) * r, Mathf.Sin(ang) * r, 0f);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}
