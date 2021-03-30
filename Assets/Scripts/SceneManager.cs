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
    }

    void Update()
    {
        if (Debug.isDebugBuild && Input.GetKey(KeyCode.L))
            LoadNextScene();
    }

    public void LoadNextScene()
    {
        _totalScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        _currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        UnityEngine.SceneManagement.SceneManager.LoadScene((_currentScene + 1) % _totalScenes);
    }
}
