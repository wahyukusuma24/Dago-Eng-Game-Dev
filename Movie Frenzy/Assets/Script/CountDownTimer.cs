    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CountDownTimer : MonoBehaviour
{
    // 
    
    public TMP_Text countdownText;
    public string sceneToLoad = "Add Scene To Load";

    private int timer = 3;

    void Start()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        for (int i = timer; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
