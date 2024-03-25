using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnswerCounter : MonoBehaviour
{
    public TMP_Text counterText;
    private int totalAnsweredQuestion = 0;

    public void Start()
    {
        UpdateCounterText();
    }

    public void IncrementAnsweredQuestions()
    {
        totalAnsweredQuestion++;
        UpdateCounterText();
    }

    private void UpdateCounterText()
    {
        counterText.text = "Answered Question: " + totalAnsweredQuestion.ToString();
    }
}
