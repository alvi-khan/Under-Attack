using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public void LoadNextScene()
    {
        SceneManager sceneManager = FindObjectOfType<SceneManager>();
        sceneManager.LoadNextScene();
    }
}
