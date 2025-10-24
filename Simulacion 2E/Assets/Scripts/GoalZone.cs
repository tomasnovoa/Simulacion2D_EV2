using UnityEngine;

public class GoalZone : MonoBehaviour
{
   public Vector2 halfSize = new Vector2(0.6f, 0.6f);

    private void Update()
    {
        var projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (var go in projectiles)
        {
            var c = go.GetComponent<Circle2D>();
            if (c == null) continue;
            if (Overlaps(c))
            {
                GameManager.Instance?.OnWin();
                enabled = false;
                break;
            }
        }
    }
    private bool Overlaps(Circle2D c)
    {
        Vector2 center = transform.position;
        Vector2 p = c.Center;
        Vector2 q = new Vector2(
            Mathf.Clamp(p.x, center.x - halfSize.x, center.x + halfSize.x),
            Mathf.Clamp(p.y, center.y - halfSize.y, center.y + halfSize.y)
        );
        return (p - q).sqrMagnitude <= c.radius * c.radius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, (Vector3)(halfSize * 2f));
    }
}
