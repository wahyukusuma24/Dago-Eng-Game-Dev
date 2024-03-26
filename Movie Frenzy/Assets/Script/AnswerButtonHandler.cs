using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnswerButtonHandler : MonoBehaviour
{
    public string CorrectAnswer { get; set; }
    public QuestionLoader questionLoader;
    public LifeManager lifeManager;
    private TMP_Text buttonText;
    private Image buttonImage;
    private bool answerSelected = false;

    public void Start()
    {
        buttonText = GetComponentInChildren<TMP_Text>();
        buttonImage = GetComponent<Image>();

        if (buttonText == null)
        {
            Debug.Log("TMP_Text component not found");
        }

        if (buttonImage == null)
        {
            Debug.Log("Image component not found");
        }
    }

    public void CheckAnswer(string selectedOption)
    {
        if (!answerSelected)
        {
            answerSelected = true; // supaya gak ada pemilihan ganda

            if (selectedOption == CorrectAnswer)
            {
                Debug.Log("Correct Answer!");
                buttonImage.color = Color.green; 
                buttonText.color = Color.white; 
            }
            else
            {
                Debug.Log("Wrong Answer!");
                buttonImage.color = Color.red;
                buttonText.color = Color.white;
                lifeManager.LoseLife();            
            }

            DisableOtherButtonAnswer();
            questionLoader.CallIncrementAnsweredQuestions();
            StartCoroutine(ResetAllButtonAndLoadNextQuestion());
        }
    }

    IEnumerator ResetAllButtonAndLoadNextQuestion()
    {
        yield return new WaitForSeconds(0.3f);

        ResetButtonColor();
        EnableAllButtons();

        questionLoader.StartNextQuestion();
    }

    public void ResetButtonColor()
    {
        Color colorFromHex;
        ColorUtility.TryParseHtmlString("#110C66", out colorFromHex);
        buttonImage.color = Color.white;
        buttonText.color = colorFromHex;
    }

    private void DisableOtherButtonAnswer()
    {
        AnswerButtonHandler[] answerButtonHandlers = FindObjectsOfType<AnswerButtonHandler>();

        foreach (AnswerButtonHandler handler in answerButtonHandlers)
        {
            if (handler != this)
            {
                Button button = handler.GetComponent<Button>();
                button.interactable = false;
            }
        }
    }

    private void EnableAllButtons()
    {
        AnswerButtonHandler[] answerButtonHandlers =  FindObjectsOfType<AnswerButtonHandler>();

        foreach (AnswerButtonHandler handler in answerButtonHandlers)
        {
            if (handler != this)
            {
                Button button = handler.GetComponent<Button>();
                button.interactable = true;
            }
        }
    }

     public void ResetAnswerState()
    {
        answerSelected = false;
    }

}
