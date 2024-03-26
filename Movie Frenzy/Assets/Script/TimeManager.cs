using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class TimeManager : MonoBehaviour
{
    public float totalTime = 150f;
    public TMP_Text timerText;
    public string nameScene;

    private float currentTime;
    private bool isTimerRunning = false;

    private void Start()
    {
        currentTime = totalTime;
        UpdateTimerText();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                EndGame();
            }

            UpdateTimerText();
        }
    }

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        if (minutes < 0)
        {
            minutes = 0; 
            seconds = 0; 
        }
        else if (seconds < 0)
        {
            seconds = 0;
        }

        string timerString = string.Format("{0}:{1}", minutes, seconds.ToString("D2"));

        timerText.text = timerString;

        if (currentTime <= 10f)
        {
            timerText.color = Color.red;
        }
    }

    private void EndGame()
    {
        isTimerRunning = false;
        SceneManager.LoadScene(nameScene);
        Debug.LogError("Game Over!!!");
    }
}
