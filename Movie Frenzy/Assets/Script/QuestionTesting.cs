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

public class QuestionTesting : MonoBehaviour
{
    public TMP_Text optionAText;
    public TMP_Text optionBText;
    public TMP_Text optionCText;
    public TMP_Text optionDText;
    public RawImage rawImage;
    public VideoPlayer videoPlayer; // Tambahkan VideoPlayer

    private FirebaseFirestore db;

    private void Start()
    {
        // Inisialisasi Firebase Firestore
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;

                // Panggil metode untuk mendapatkan data secara acak dari Firestore
                LoadRandomDataFromFirestore();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

private void LoadRandomDataFromFirestore()
{
    // Panggil Firestore untuk mendapatkan dokumen "Action" dari koleksi "Question"
    db.Collection("Question").Document("Action").GetSnapshotAsync().ContinueWithOnMainThread(task =>
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Failed to load data from Firestore: " + task.Exception);
            return;
        }

        DocumentSnapshot docSnapshot = task.Result;
        if (docSnapshot != null && docSnapshot.Exists)
        {
            Debug.Log("Document 'Action' exists in collection 'Question'!");

            // Ambil data map dari dokumen
            IDictionary<string, object> data = docSnapshot.ToDictionary();
            
            // Cek apakah bidang-bidang yang dibutuhkan ada dalam data
            if (data.ContainsKey("Question") && data.ContainsKey("OptionA") && data.ContainsKey("OptionB") && data.ContainsKey("OptionC") && data.ContainsKey("OptionD"))
            {
                Debug.Log("All required fields are present in the document 'Action'!");

                // Ambil nilai dari bidang-bidang yang dibutuhkan
                string videoUrl = (string)data["Question"];
                string optionA = (string)data["OptionA"];
                string optionB = (string)data["OptionB"];
                string optionC = (string)data["OptionC"];
                string optionD = (string)data["OptionD"];
                
                // Tampilkan nilai-nilai tersebut
                StartCoroutine(LoadVideo(videoUrl));
                optionAText.text = optionA;
                optionBText.text = optionB;
                optionCText.text = optionC;
                optionDText.text = optionD;
            }
            else
            {
                Debug.LogError("Required fields are missing in the document 'Action'!");
            }
        }
        else
        {
            Debug.LogError("Document 'Action' not found in collection 'Question'!");
        }
    });
}

    IEnumerator LoadVideo(string url)
    {
        // Download video dari URL menggunakan UnityWebRequest
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Failed to load video: " + request.error);
            }
            else
            {
                // Simpan data video yang diunduh
                byte[] videoData = request.downloadHandler.data;

                // Buat temporary file untuk menyimpan video
                string tempFilePath = Application.persistentDataPath + "/" + Guid.NewGuid().ToString() + ".mp4";
                System.IO.File.WriteAllBytes(tempFilePath, videoData);

                // Load video dari temporary file menggunakan VideoPlayer
                videoPlayer.url = tempFilePath;
                videoPlayer.Prepare();
            }
        }
    }
}
