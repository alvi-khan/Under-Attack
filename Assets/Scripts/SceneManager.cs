using UnityEngine;

/// <summary>
/// Custom scene manager.
/// </summary>
public class SceneManager : MonoBehaviour
{
    private int _totalScenes, _currentScene;
    void Awake()
    {
        // Singleton pattern
        int sceneManagerCount = FindObjectsOfType<SceneManager>().Length;
        if (sceneManagerCount > 1)  Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);

        _totalScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
    }

    void Update()
    {
        // Leve skip debug key
        if (Debug.isDebugBuild && Input.GetKey(KeyCode.L))
            LoadNextScene();
    }

    public void LoadNextScene()
    {
        _currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        if (_currentScene == _totalScenes - 1) _currentScene = -1;  // don't go to Game Over scene
        UnityEngine.SceneManagement.SceneManager.LoadScene((_currentScene + 1) % (_totalScenes - 1));
    }

    /// <summary>
    /// Loads the Game Over scene
    /// </summary>
    public void EndGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_totalScenes - 1);
    }
}
