using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
    public int maxLives = 3;
    public int currentLives;
    public TMP_Text lifeText;
    public string nameScene;

    private void Start()
    {
        currentLives = maxLives;
        UpdateLifeText();
    }

    public void LoseLife()
    {
        currentLives--;
        UpdateLifeText();

        if (currentLives <= 0)
        {
            EndGame();
        }
    }

    private void UpdateLifeText()
    {
        lifeText.text = currentLives.ToString();
    }

    private void EndGame()
    {
        SceneManager.LoadScene(nameScene);
        Debug.LogError("Game Over!!!");
    }
}
