using UnityEngine;

/// <summary>
/// Class to handle behaviour of play button on main menu.
/// Exists to bypass issues caused by attaching button directly to scene manager.
/// </summary>
public class ButtonHandler : MonoBehaviour
{
    public void LoadNextScene()
    {
        SceneManager sceneManager = FindObjectOfType<SceneManager>();
        sceneManager.LoadNextScene();
    }
}
