using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls behaviour of loading screens.
/// </summary>
public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private int goldOnNextLevelStart = 100;
    [SerializeField] private float textDisplayTime = 1f;
    [SerializeField] private float minimumLoadTime = 3f;

    private TMP_Text _information;
    private bool _textDisplayDone;

    void Start()
    {
        StartCoroutine(PreloadNextScene());
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
            _textDisplayDone = true;
        }
        else
        {
            _information.SetText("Gold: " + GameData.Gold);
            StartCoroutine(UpdateGold());
        }
    }

    IEnumerator UpdateGold()
    {
        yield return new WaitForSeconds(textDisplayTime);
        int finalGold = GameData.Gold + goldOnNextLevelStart;

        while (GameData.Gold < finalGold)
        {
            GameData.Gold++;
            _information.SetText("Gold: " + GameData.Gold);
            yield return new WaitForEndOfFrame();
        }
        _textDisplayDone = true;
    }

    IEnumerator PreloadNextScene()
    {
        int totalScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        int currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        if (currentScene == totalScenes - 1) currentScene = -1; // don't load Game Over scene.

        AsyncOperation mainScene = UnityEngine.SceneManagement.SceneManager
            .LoadSceneAsync((currentScene + 1) % (totalScenes-1));

        // wait until minimum load time has passed and text is done updating to load next scene
        mainScene.allowSceneActivation = false;
        while (!_textDisplayDone) yield return null;
        yield return new WaitForSeconds(minimumLoadTime);
        mainScene.allowSceneActivation = true;
    }
}
