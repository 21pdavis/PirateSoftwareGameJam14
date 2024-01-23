using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameStage Stage = GameStage.Menu;

    public enum GameStage
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
}
