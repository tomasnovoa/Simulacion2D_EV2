using UnityEngine;

public class SlingshotController2D : MonoBehaviour
{
   [Header("Honda")]
    public Transform pivot;
    public float maxStretch = 3f;
    public float launchPower = 10f;

    [Header("Proyectil")]
    public PhysicsBody2D projectilePrefab;

    private Camera _cam;
    private bool _dragging;
    private Vector2 _dragWorld;
    private PhysicsBody2D _current;

    private void Awake() => _cam = Camera.main;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _dragging = true;
            _dragWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            SpawnProjectile();
        }

        if (_dragging)
        {
            _dragWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 stretch = (Vector2)pivot.position - _dragWorld;
            if (stretch.magnitude > maxStretch)
                stretch = stretch.normalized * maxStretch;

            if (_current != null)
                _current.transform.position = pivot.position - (Vector3)stretch;
        }

        if (Input.GetMouseButtonUp(0) && _dragging)
        {
            _dragging = false;
            if (_current != null)
            {
                Vector2 stretch = (Vector2)pivot.position - (Vector2)_current.transform.position;
                Vector2 v0 = stretch * launchPower;
                _current.velocity = v0;
                _current.useGravity = false; // en Space la gravedad es radial de pozos
                _current = null;
            }
        }
    }

    private void SpawnProjectile()
    {
        if (_current != null) return;
        _current = Instantiate(projectilePrefab, pivot.position, Quaternion.identity);
        _current.tag = "Projectile";
        var circle = _current.GetComponent<Circle2D>();
        if (circle == null) _current.gameObject.AddComponent<Circle2D>();
    }
}
