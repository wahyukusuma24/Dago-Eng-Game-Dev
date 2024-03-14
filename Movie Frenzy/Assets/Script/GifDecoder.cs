using System;
using System.Collections.Generic;
using UnityEngine;

public class GifDecoder
{
    private byte[] gifData;
    private int currentPosition;
    private int lastDisposalMethod;
    private bool isDecoded;

    private List<Texture2D> frames;
    private List<float> delays;

    public GifDecoder()
    {
        frames = new List<Texture2D>();
        delays = new List<float>();
    }

    public bool Read(byte[] data)
    {
        gifData = data;
        currentPosition = 0;
        isDecoded = false;

        if (gifData.Length < 13 || gifData[0] != 'G' || gifData[1] != 'I' || gifData[2] != 'F' || gifData[3] != '8')
        {
            Debug.LogError("Not a valid GIF file!");
            return false;
        }

        Decode();
        return isDecoded;
    }

    private void Decode()
    {
        frames.Clear();
        delays.Clear();

        ReadLogicalScreenDescriptor();

        while (currentPosition < gifData.Length)
        {
            byte blockType = gifData[currentPosition++];

            switch (blockType)
            {
                case 0x21:
                    ReadExtensionBlock();
                    break;
                case 0x2C:
                    ReadImageDescriptor();
                    break;
                case 0x3B:
                    return; // End of GIF
            }
        }

        isDecoded = true;
    }

    private void ReadLogicalScreenDescriptor()
    {
        // Read logical screen descriptor data (skipped for simplicity)
        currentPosition += 7;
    }

    private void ReadImageDescriptor()
    {
        // Read image descriptor data (skipped for simplicity)

        // Read local color table (skipped for simplicity)

        // Read image data
        ReadImageData();
    }

    private void ReadImageData()
    {
        while (true)
        {
            byte blockType = gifData[currentPosition++];

            if (blockType == 0x3B) // End of GIF
            {
                break;
            }

            if (blockType == 0x2C) // Image Descriptor
            {
                // Read image descriptor data (skipped for simplicity)

                // Read local color table (skipped for simplicity)

                // Read image data
                ReadFrameData();
            }
            else if (blockType == 0x21) // Extension Block
            {
                ReadExtensionBlock();
            }
        }
    }

    private void ReadExtensionBlock()
    {
        // Read extension block data (skipped for simplicity)
        // You may need to implement logic to handle different extension block types
        // For simplicity, I'm skipping the actual implementation here
        byte extensionLabel = gifData[currentPosition++];

        // Read extension block data
        if (extensionLabel == 0xF9)
        {
            // Graphic Control Extension
            // Skipped for simplicity, you may implement this if needed
            currentPosition += 6; // Skip the data block
        }
        else if (extensionLabel == 0xFE)
        {
            // Comment Extension
            // Skip comment block
            SkipDataBlocks();
        }
        else if (extensionLabel == 0x01)
        {
            // Plain Text Extension
            // Skipped for simplicity, you may implement this if needed
            currentPosition += 13; // Skip the data block
        }
        else if (extensionLabel == 0xFF)
        {
            // Application Extension (Netscape Extension)
            // Skipped for simplicity, you may implement this if needed
            currentPosition += 14; // Skip the data block
        }
        else
        {
            // Unknown extension block
            Debug.LogWarning("Unknown extension block: " + extensionLabel);
            SkipDataBlocks();
        }
    }

    private void SkipDataBlocks()
    {
        // Read the length of the data block
        byte blockSize;
        do
        {
            blockSize = gifData[currentPosition++];
            currentPosition += blockSize;
        }
        while (blockSize != 0); // Continue reading until block size is 0
    }

    private void ReadFrameData()
    {
        // Read frame data (skipped for simplicity)
        // Assuming each frame is read and stored in Texture2D
        // You may need to implement more complex logic to properly decode and store frames
        Texture2D frame = new Texture2D(100, 100); // Adjust size as needed
        frame.LoadImage(gifData); // Example: Load image data directly into texture
        frames.Add(frame);

        delays.Add(0); // Placeholder delay for simplicity
    }

    public int GetFrameCount()
    {
        return frames.Count;
    }

    public int GetFrameWidth()
    {
        if (frames.Count > 0)
        {
            return frames[0].width;
        }
        return 0;
    }

    public int GetFrameHeight()
    {
        if (frames.Count > 0)
        {
            return frames[0].height;
        }
        return 0;
    }

    public Color32[] GetCurrentFrame()
    {
        if (frames.Count > 0)
        {
            return frames[0].GetPixels32();
        }
        return null;
    }

    public float GetFrameDelay()
    {
        if (delays.Count > 0)
        {
            return delays[0];
        }
        return 0;
    }

    public void MoveNextFrame()
    {
        if (frames.Count > 0)
        {
            frames.RemoveAt(0);
            delays.RemoveAt(0);
        }
    }
}
