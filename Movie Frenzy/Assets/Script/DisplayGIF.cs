using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
using System.Threading.Tasks;

public class DisplayGIF : MonoBehaviour
{
  public RawImage imageDisplay;
    public string firebaseStoragePath = "gs://movie-frenzy-fd185.appspot.com/spiderman-1.png";

    private async void Start()
    {
        await DownloadAndDisplayImage();
    }

    private async Task DownloadAndDisplayImage()
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference reference = storage.GetReferenceFromUrl(firebaseStoragePath);

        // Download the image data
        byte[] imageBytes;
        try
        {
            imageBytes = await reference.GetBytesAsync(10 * 1024 * 1024); // Max size 10MB
        }
        catch (Firebase.Storage.StorageException e)
        {
            Debug.LogError("Error downloading image: " + e.Message);
            return;
        }

        // Create a Texture2D from the downloaded bytes
        Texture2D texture = new Texture2D(800, 450);
        texture.LoadImage(imageBytes);
        Debug.Log("Image Bytes Length: " + imageBytes.Length);

        // Assign the texture to the RawImage component
        imageDisplay.texture = texture;
    }
}
