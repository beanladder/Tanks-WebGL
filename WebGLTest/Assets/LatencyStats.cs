using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LatencyStats : MonoBehaviourPunCallbacks
{
    public Text displayText; // Reference to the UI Text component
    private Queue<float> fpsBuffer = new Queue<float>();
    private int bufferSize = 30; // Number of frames to average
    private float deltaTime = 0.0f;

    void Update()
    {
        if (photonView.IsMine)
        {
            // Calculate frame time
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            // Calculate FPS
            float fps = 1.0f / deltaTime;

            // Add to buffer
            fpsBuffer.Enqueue(fps);

            // Remove oldest if buffer is full
            if (fpsBuffer.Count > bufferSize)
            {
                fpsBuffer.Dequeue();
            }

            // Calculate average FPS
            float averageFps = 0;
            foreach (float fpsValue in fpsBuffer)
            {
                averageFps += fpsValue;
            }
            averageFps /= fpsBuffer.Count;

            // Get Ping
            int ping = PhotonNetwork.GetPing();

            // Packet Loss (not directly available in Photon, we can approximate)
            float packetLoss = CalculatePacketLoss();

            // Update UI Text
            displayText.text = string.Format("Avg FPS: {0:0.}\nPing: {1} ms\nPacket Loss: {2:0.0}%", averageFps, ping, packetLoss);
        }
    }

    float CalculatePacketLoss()
    {
        // Dummy implementation for packet loss calculation, replace with your own logic
        // Photon does not directly provide packet loss metrics
        // This example returns a dummy value
        return Random.Range(0f, 5f);
    }
}
