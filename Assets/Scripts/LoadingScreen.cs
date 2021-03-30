using TMPro;
using UnityEngine;

/// <summary>
/// Controls behaviour of text shown on loading screens.
/// </summary>
public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private int goldOnNextLevelStart = 100;
    private TMP_Text _information;

    void Start()
    {
        _information = GetComponent<TMP_Text>();
        DisplayInformation();
    }

    void DisplayInformation()
    {
        int currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int totalScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

        if (currentScene == totalScenes - 1 || currentScene == totalScenes - 2) // if win/lose screen
        {
            _information.SetText("Final Score: " + GameData.Score);
            GameData.Score = 0;
            GameData.Gold = 0;
        }
        else
        {
            _information.SetText("Gold: +" + goldOnNextLevelStart);
            GameData.Gold += goldOnNextLevelStart;
        }
    }
}
