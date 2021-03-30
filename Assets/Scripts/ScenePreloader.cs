using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (currentScene == totalScenes - 1) currentScene = -1;
        AsyncOperation mainScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((currentScene + 1) % (totalScenes-1));
        mainScene.allowSceneActivation = false;
        yield return new WaitForSeconds(minimumLoadTime);
        mainScene.allowSceneActivation = true;
    }
}
