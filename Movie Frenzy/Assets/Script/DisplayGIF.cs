// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using Firebase.Storage;
// using System.Threading.Tasks;


// public class DisplayGIF : MonoBehaviour
// {
//   public RawImage gifDisplay;
//     public string firebaseStoragePath = "gs://movie-frenzy-fd185.appspot.com/Napoleon-6.png";

//     private async void Start()
//     {
//         await DownloadAndDisplayImage();
//     }

//     private async Task DownloadAndDisplayImage()
//     {
//         FirebaseStorage storage = FirebaseStorage.DefaultInstance;
//         StorageReference reference = storage.GetReferenceFromUrl(firebaseStoragePath);

//         byte[] imageBytes;
//         try
//         {
//             imageBytes = await reference.GetBytesAsync(10 * 1024 * 1024); 
//         }
//         catch (Firebase.Storage.StorageException e)
//         {
//             Debug.LogError("Error downloading image: " + e.Message);
//             return;
//         }

        // Texture2D texture = new Texture2D(800, 450);
        // texture.LoadImage(imageBytes);
        // Debug.Log("Image Bytes Length: " + imageBytes.Length);

//         gifDisplay.texture = texture;
//     }
// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
using System.Threading.Tasks;

public class DisplayGIF : MonoBehaviour
{
    public RawImage gifDisplay;
    public string firebaseStoragePath = "gs://movie-frenzy-fd185.appspot.com/Napoleon-3.gif";

    private async void Start()
    {
        await DownloadAndDisplayGif();
    }

    private async Task DownloadAndDisplayGif()
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference reference = storage.GetReferenceFromUrl(firebaseStoragePath);

        byte[] gifBytes;
        try
        {
            gifBytes = await reference.GetBytesAsync(10 * 1024 * 1024); 
        }
        catch (Firebase.Storage.StorageException e)
        {
            Debug.LogError("Error downloading GIF: " + e.Message);
            Debug.LogException(e); // Log exception stack trace
            return;
        }

        // Create an instance of GifDecoder
        GifDecoder gifDecoder = new GifDecoder();
        
        // Read GIF bytes using GifDecoder
        if (!gifDecoder.Read(gifBytes))
        {
            Debug.LogError("Failed to read GIF!");
            return;
        }

        // Initialize GIF animation
        StartCoroutine(AnimateGif(gifDecoder));
    }

    private IEnumerator AnimateGif(GifDecoder gifDecoder)
    {
        // Check if frame dimensions are valid
        int frameWidth = gifDecoder.GetFrameWidth();
        int frameHeight = gifDecoder.GetFrameHeight();
        if (frameWidth <= 0 || frameHeight <= 0)
        {
            Debug.LogError("Invalid frame dimensions");
            yield break; // Exit coroutine if dimensions are invalid
        }

        // Loop through each frame in the GIF animation
        while (true)
        {
            Texture2D texture = new Texture2D(frameWidth, frameHeight);
            texture.SetPixels32(gifDecoder.GetCurrentFrame());
            texture.Apply();
            gifDisplay.texture = texture;

            yield return new WaitForSeconds(gifDecoder.GetFrameDelay() / 1000f);
            gifDecoder.MoveNextFrame();
        }
    }
}
