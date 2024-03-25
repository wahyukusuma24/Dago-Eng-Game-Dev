using UnityEngine;
using TMPro;

public class AnswerButton : MonoBehaviour
{
    private AnswerButtonHandler answerButtonHandler;

    private void Start()
    {
        answerButtonHandler = GetComponent<AnswerButtonHandler>();
    }

    public void OnClick()
    {
        string optionText = GetComponentInChildren<TMP_Text>().text;
        answerButtonHandler.CheckAnswer(optionText);
    }
}
