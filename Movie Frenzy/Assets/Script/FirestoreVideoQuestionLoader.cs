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

public class FirestoreVideoQuestionLoader : MonoBehaviour
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
        // Panggil Firestore untuk mendapatkan seluruh dokumen dari koleksi "Question"
        db.Collection("Question").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to load data from Firestore: " + task.Exception);
                return;
            }

            QuerySnapshot snapshot = task.Result;
            if (snapshot != null && snapshot.Documents.Count() > 0)
            {
                // Dapatkan jumlah total dokumen
                int totalDocuments = snapshot.Documents.Count();

                // Pilih nomor acak untuk memilih dokumen secara acak
                int randomIndex = UnityEngine.Random.Range(0, totalDocuments);

                // Ambil dokumen secara acak
                DocumentSnapshot randomDoc = snapshot.Documents.ElementAt(randomIndex);

                // Ambil nilai dari field Video
                string videoUrl = randomDoc.GetValue<string>("Question");

                // Muat video dari URL menggunakan VideoPlayer
                StartCoroutine(LoadVideo(videoUrl));

                // Ambil nilai dari field OptionA, OptionB, OptionC, dan OptionD
                string optionA = randomDoc.GetValue<string>("OptionA");
                string optionB = randomDoc.GetValue<string>("OptionB");
                string optionC = randomDoc.GetValue<string>("OptionC");
                string optionD = randomDoc.GetValue<string>("OptionD");

                // Tampilkan teks pada TMP_Text
                optionAText.text = optionA;
                optionBText.text = optionB;
                optionCText.text = optionC;
                optionDText.text = optionD;
            }
            else
            {
                Debug.LogError("No documents found in collection 'Question'!");
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
