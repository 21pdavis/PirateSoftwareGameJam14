using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button newGame;
    [SerializeField] private Button exit;

    // Start is called before the first frame update
    void Start()
    {
        newGame.onClick.AddListener(NewGame);
        exit.onClick.AddListener(Exit);
    }

    private void NewGame()
    {
        SceneManager.LoadScene("IntroDialogue");
    }

    private void Exit()
    {
        // https://discussions.unity.com/t/application-quit-not-working/130493
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
