using UnityEngine;

public class FluidState2D : MonoBehaviour
{
 [Header("Atenuación de Pozo de Gravedad")]
    public float gravityMultiplierInFluid = 0f; // 0 = sin atracción en agua
    public float lingerTime = 0.05f;           // "pegajosidad" de estado

    [HideInInspector] public float currentGravityMultiplier = 1f;
    private float _timer;

    public void PingInFluid()
    {
        _timer = Mathf.Max(_timer, lingerTime);
    }

    private void Update()
    {
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            currentGravityMultiplier = gravityMultiplierInFluid;
        }
        else
        {
            currentGravityMultiplier = 1f;
        }
    }
    
}
