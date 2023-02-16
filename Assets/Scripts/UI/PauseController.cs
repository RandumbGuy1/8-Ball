using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerRef player;
    private bool paused = false;
    private bool inEditor = false;

    void Awake() => player.PlayerInput.OnPauseToggle += HandlePause;
    void OnDestroy() => player.PlayerInput.OnPauseToggle -= HandlePause;

    private void HandlePause(bool pause)
    {
        if (!pause) return;

        paused = !paused;

        GameManager.Instance.SetState(paused ? GameState.Paused : inEditor ? GameState.Editor : GameState.Gameplay);
        pauseMenu.SetActive(paused);
    }

    public void UnPause()
    {
        HandlePause(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleEditor()
    {
        inEditor = !inEditor;
        HandlePause(true);

        GameManager.Instance.SetState(inEditor ? GameState.Editor : GameState.Gameplay);
        pauseMenu.SetActive(false);
    }
}
