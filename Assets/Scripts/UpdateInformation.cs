using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateInformation : MonoBehaviour
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

        if (currentScene == totalScenes - 1 || currentScene == totalScenes - 2)
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
