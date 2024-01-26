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
        LevelOne
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
        }
    }
}
