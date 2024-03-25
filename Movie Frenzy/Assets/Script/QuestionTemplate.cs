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

public class QuestionTemplate : MonoBehaviour
{
    public TMP_Text optionAText;
    public TMP_Text optionBText;
    public TMP_Text optionCText;
    public TMP_Text optionDText;
    public RawImage rawImage;
    public VideoPlayer videoPlayer;

    private FirebaseFirestore db;
    private IDictionary<string, object> cachedData;

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
        // Cek apakah data sudah ada di cache lokal
        if (cachedData != null)
        {
            DisplayData(cachedData);
        }
        else
        {
            // Panggil Firestore untuk mendapatkan data
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
                    Debug.LogError("Document 'Fantasy' does not exist in collection 'Question'!");
                }
            });
        }
    }

    private void DisplayData(IDictionary<string, object> data)
    {
        // Ambil daftar field yang tersedia dalam dokumen "Action"
        List<string> fieldNames = data.Keys.ToList();

        // Pilih secara acak satu field dari daftar fieldNames
        string randomFieldName = fieldNames[UnityEngine.Random.Range(0, fieldNames.Count)];

        // Ambil nilai dari field yang dipilih secara acak
        IDictionary<string, object> randomFieldData = (IDictionary<string, object>)data[randomFieldName];
        string question = randomFieldData.ContainsKey("Question") ? randomFieldData["Question"].ToString() : "";
        string optionA = randomFieldData.ContainsKey("OptionA") ? randomFieldData["OptionA"].ToString() : "";
        string optionB = randomFieldData.ContainsKey("OptionB") ? randomFieldData["OptionB"].ToString() : "";
        string optionC = randomFieldData.ContainsKey("OptionC") ? randomFieldData["OptionC"].ToString() : "";
        string optionD = randomFieldData.ContainsKey("OptionD") ? randomFieldData["OptionD"].ToString() : "";

        // Tampilkan teks pada TMP_Text
        optionAText.text = optionA;
        optionBText.text = optionB;
        optionCText.text = optionC;
        optionDText.text = optionD;

        // Jika terdapat field "Question", Anda dapat memuat video jika itu URL
        if (!string.IsNullOrEmpty(question))
        {
            // Cek jika question adalah URL video
            if (Uri.TryCreate(question, UriKind.Absolute, out Uri videoUri) && (videoUri.Scheme == Uri.UriSchemeHttp || videoUri.Scheme == Uri.UriSchemeHttps))
            {
                // Muat video dari URL menggunakan VideoPlayer
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
        // Buat temporary file untuk menyimpan video
        string tempFilePath = Application.persistentDataPath + "/" + Guid.NewGuid().ToString() + ".mp4";

        // Download video dari URL menggunakan UnityWebRequest
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Kirim permintaan unduhan
            yield return request.SendWebRequest();

            // Cek jika terjadi kesalahan saat mengunduh video
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Failed to load video: " + request.error);
                yield break; // Keluar dari coroutine jika terjadi kesalahan
            }

            // Simpan data video yang diunduh ke file temporary
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

}
