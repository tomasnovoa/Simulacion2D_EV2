using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
     public static GameManager Instance { get; private set; }
    public enum GameState { Start, Playing, Win, Lose }
    public GameState State { get; private set; } = GameState.Start;

    public KeyCode startKey = KeyCode.Space;
    public KeyCode restartKey = KeyCode.R;
    public float maxTime = 170f;

    private float _t0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(restartKey))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        switch (State)
        {
            case GameState.Start:
                if (Input.GetKeyDown(startKey)) { State = GameState.Playing; _t0 = Time.time; }
                break;
            case GameState.Playing:
                if (Time.time - _t0 > maxTime) OnLose();
                break;
        }
    }

    public void OnWin() { if (State == GameState.Playing) State = GameState.Win; }
    public void OnLose() { if (State == GameState.Playing) State = GameState.Lose; }

    private void OnGUI()
    {
        string msg = State switch
        {
            GameState.Start => "Espacio: iniciar | R: reiniciar",
            GameState.Playing => "Jugando... R: reiniciar",
            GameState.Win => "¡Ganaste! R: reiniciar",
            GameState.Lose => "Tiempo agotado. R: reiniciar",
            _ => ""
        };
        GUI.Label(new Rect(10, 10, 420, 24), msg);
    }
}
