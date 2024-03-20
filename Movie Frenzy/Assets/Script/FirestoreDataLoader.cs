using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;

public class FirestoreDataLoader : MonoBehaviour
{
    public TMP_Text optionAText;
    public TMP_Text optionBText;
    public TMP_Text optionCText;
    public TMP_Text optionDText;
    public RawImage rawImage;

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
                int randomIndex = Random.Range(0, totalDocuments);

                // Ambil dokumen secara acak
                DocumentSnapshot randomDoc = snapshot.Documents.ElementAt(randomIndex);

                // Ambil nilai dari field Image
                string imageUrl = randomDoc.GetValue<string>("Question");

                // Muat gambar dari URL menggunakan UnityWebRequest
                StartCoroutine(LoadImage(imageUrl));

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

    IEnumerator LoadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Failed to load image: " + request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            rawImage.texture = texture;
        }
    }
}
