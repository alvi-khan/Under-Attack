using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private float minimumSceneLoadTime = 3f;
    private int _totalScenes, _currentScene;
    void Awake()
    {
        int sceneManagerCount = FindObjectsOfType<SceneManager>().Length;
        if (sceneManagerCount > 1)  Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);

        _totalScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
    }

    void Update()
    {
        if (Debug.isDebugBuild && Input.GetKey(KeyCode.L))
            LoadNextScene();
    }

    public void LoadNextScene()
    {
        _currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        if (_currentScene == _totalScenes - 1) _currentScene = -1;
        UnityEngine.SceneManagement.SceneManager.LoadScene((_currentScene + 1) % (_totalScenes - 1));
    }

    public void EndGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_totalScenes - 1);
    }
}
