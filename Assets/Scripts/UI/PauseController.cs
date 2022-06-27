using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private SettingsMenu settings;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerRef player;
    private bool paused = false;

    void Awake() => player.PlayerInput.OnPauseToggle += HandlePause;
    void OnDestroy() => player.PlayerInput.OnPauseToggle -= HandlePause;

    private void HandlePause(bool pause)
    {
        if (!pause) return;

        paused = !paused;

        if (!paused) settings.CloseMenus();

        GameManager.Instance.SetState(paused ? GameState.Paused : GameState.Gameplay);
        player.CameraBody.SetCursorState(!paused);
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
}
