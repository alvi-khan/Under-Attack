using System.Collections;
using UnityEngine;

/// <summary>
/// Preloads actual level scenes while on intermediate scene.
/// Combining with custom scene manager causes issues.
/// </summary>
public class ScenePreloader : MonoBehaviour
{
    [SerializeField] private float minimumLoadTime = 3f;
    void Start()
    {
        StartCoroutine(PreloadNextScene());
    }

    IEnumerator PreloadNextScene()
    {
        int totalScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        int currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        if (currentScene == totalScenes - 1) currentScene = -1; // don't load Game Over scene.

        AsyncOperation mainScene = UnityEngine.SceneManagement.SceneManager
            .LoadSceneAsync((currentScene + 1) % (totalScenes-1));

        // wait until minimum load time has passed to activate scene
        mainScene.allowSceneActivation = false;
        yield return new WaitForSeconds(minimumLoadTime);
        mainScene.allowSceneActivation = true;
    }
}
