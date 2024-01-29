using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState Stage = GameState.Menu;

    public enum GameState
    {
        Menu,
        IntroDialogue,
        LevelOne,
        Outro,
        Conclusion
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TransitionTo(GameState stage)
    {
        switch (stage)
        {
            case GameState.Menu:
                SceneManager.LoadScene("Menu");
                break;
            case GameState.IntroDialogue:
                SceneManager.LoadScene("IntroDialogue");
                break;
            case GameState.LevelOne:
                SceneManager.LoadScene("LevelOne");
                break;
            case GameState.Outro:
                SceneManager.LoadScene("Outro");
                break;
            case GameState.Conclusion:
                // https://discussions.unity.com/t/application-quit-not-working/130493
#if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
                break;
        }
    }
}
