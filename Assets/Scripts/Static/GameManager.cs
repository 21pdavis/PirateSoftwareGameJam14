using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState Stage = GameState.Menu;

    public enum GameState
    {
        Menu,
        IntroDialogue,
        LevelOne
    }

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameManager");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void TransitionTo(GameState stage)
    {
        switch (stage)
        {
            case GameState.Menu:

                break;
            case GameState.IntroDialogue:

                break;
            case GameState.LevelOne:

                break;
        }
    }
}
