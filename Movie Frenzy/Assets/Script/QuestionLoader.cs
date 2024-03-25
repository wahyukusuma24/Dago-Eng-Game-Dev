using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Linq;

public class QuestionLoader : MonoBehaviour
{
    public TMP_Text optionAText;
    public TMP_Text optionBText;
    public TMP_Text optionCText;
    public TMP_Text optionDText;
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    public AnswerButtonHandler optionAButtonHandler;
    public AnswerButtonHandler optionBButtonHandler;
    public AnswerButtonHandler optionCButtonHandler;
    public AnswerButtonHandler optionDButtonHandler;
    public AnswerCounter answerCounter;

    private FirebaseFirestore db;
    private IDictionary<string, object> cachedData;
    private List<string> usedQuestions = new List<string>();

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
                LoadDataFromCacheOrFirestore();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    private void LoadDataFromCacheOrFirestore()
    {
        if (cachedData != null)
        {
            DisplayData(cachedData);
        }
        else
        {
            db.Collection("Question").Document("Action").GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to load data from Firestore: " + task.Exception);
                    return;
                }

                DocumentSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                {
                    cachedData = snapshot.ToDictionary();
                    DisplayData(cachedData);
                }
                else
                {
                    Debug.LogError("Document 'Action' does not exist in collection 'Question'!");
                }
            });
        }
    }

    private void DisplayData(IDictionary<string, object> data)
    {
        List<string> fieldNames = data.Keys.ToList();

        List<string> availableQuestions = fieldNames.Except(usedQuestions).ToList();
        if (availableQuestions.Count == 0)
        {
            Debug.LogError("All questions have been displayed. Resetting question list.");
            usedQuestions.Clear();
            availableQuestions = fieldNames;
        }

        availableQuestions = availableQuestions.OrderBy(x => Guid.NewGuid()).ToList();

        string randomFieldName = availableQuestions[0];

        usedQuestions.Add(randomFieldName);

        IDictionary<string, object> randomFieldData = (IDictionary<string, object>)data[randomFieldName];
        string question = randomFieldData.ContainsKey("Question") ? randomFieldData["Question"].ToString() : "";
        string optionA = randomFieldData.ContainsKey("OptionA") ? randomFieldData["OptionA"].ToString() : "";
        string optionB = randomFieldData.ContainsKey("OptionB") ? randomFieldData["OptionB"].ToString() : "";
        string optionC = randomFieldData.ContainsKey("OptionC") ? randomFieldData["OptionC"].ToString() : "";
        string optionD = randomFieldData.ContainsKey("OptionD") ? randomFieldData["OptionD"].ToString() : "";
        string correctAnswer = randomFieldData.ContainsKey("Answer") ? randomFieldData["Answer"].ToString() : "";

        Debug.Log("Id Soal: " + randomFieldName);

        optionAText.text = optionA;
        optionBText.text = optionB;
        optionCText.text = optionC;
        optionDText.text = optionD;

        optionAButtonHandler.CorrectAnswer = correctAnswer;
        optionBButtonHandler.CorrectAnswer = correctAnswer;
        optionCButtonHandler.CorrectAnswer = correctAnswer;
        optionDButtonHandler.CorrectAnswer = correctAnswer;

        if (!string.IsNullOrEmpty(question))
        {
            if (Uri.TryCreate(question, UriKind.Absolute, out Uri videoUri) && (videoUri.Scheme == Uri.UriSchemeHttp || videoUri.Scheme == Uri.UriSchemeHttps))
            {
                StartCoroutine(LoadVideo(question));
            }
            else
            {
                // Jika bukan URL video, mungkin itu hanya teks pertanyaan
                // Anda bisa menangani teks pertanyaan di sini jika perlu
            }
        }
    }

    private IEnumerator LoadVideo(string url)
    {
        string tempFilePath = Application.persistentDataPath + "/" + Guid.NewGuid().ToString() + ".mp4";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Failed to load video: " + request.error);
                yield break;
            }

            System.IO.File.WriteAllBytes(tempFilePath, request.downloadHandler.data);
        }

        videoPlayer.url = tempFilePath;

        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
    }

    public void StartNextQuestion()
    {
        ResetButtonColorForOptionButtons();
        LoadDataFromCacheOrFirestore();

        AnswerButtonHandler[] answerButtonHandlers = FindObjectsOfType<AnswerButtonHandler>();
        foreach (AnswerButtonHandler handler in answerButtonHandlers)
        {
            handler.ResetAnswerState();
        }
    }

    public void ResetButtonColorForOptionButtons()
    {
        optionAButtonHandler.ResetButtonColor();
        optionBButtonHandler.ResetButtonColor();
        optionCButtonHandler.ResetButtonColor();
        optionDButtonHandler.ResetButtonColor();
    }

    public void CallIncrementAnsweredQuestions()
    {
        // Memanggil IncrementAnsweredQuestions dari AnswerCounter
        answerCounter.IncrementAnsweredQuestions();
    }
}
