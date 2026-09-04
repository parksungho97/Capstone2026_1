using Fusion;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class VoiceClipSender : NetworkBehaviour
{
    [SerializeField] private VoiceClipManager voiceClipManager;

    [SerializeField] private const string BASE_URL = "https://finisher-cavalry-narrow.ngrok-free.dev/";
    private const string UPLOAD_URL = BASE_URL + "upload.php";
    private const string CLEANUP_URL = BASE_URL + "cleanup.php";
    private const int SAMPLE_RATE = 16000;

    private bool _isUploading = false;

    public override void Spawned()
    {
        base.Spawned();
        CleanupServerFiles();
    }

    public override void Despawned(NetworkRunner runner, bool hasStateAuthority)
    {
        CleanupServerFiles();
    }

    private void OnDestroy()
    {
        CleanupServerFiles();
    }

    public void SendVoice(float[] data)
    {
        if (_isUploading) return;
        _isUploading = true;

        float[] downsampled = Downsample(data, 44100, SAMPLE_RATE);
        byte[] wavFileBytes = ConvertToWavFileBytes(downsampled, SAMPLE_RATE);

        StartCoroutine(UploadVoiceToWebInterface(wavFileBytes));
    }

    private IEnumerator UploadVoiceToWebInterface(byte[] voiceData)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", voiceData, "voice.wav", "audio/wav");

        using (UnityWebRequest www = UnityWebRequest.Post(UPLOAD_URL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Upload Failed: {www.error}");
                _isUploading = false;
                yield break;
            }

            string uploadedAudioUrl = www.downloadHandler.text.Trim();
            RPC_SendAudioUrlToHost(uploadedAudioUrl);
        }

        _isUploading = false;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
    private void RPC_SendAudioUrlToHost(string audioUrl)
    {
        RPC_BroadcastAudioUrl(audioUrl);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    private void RPC_BroadcastAudioUrl(string audioUrl)
    {
        if (VoiceClipManager.Instance.Count >= VoiceClipManager.Instance.MaxClips) return;
        StartCoroutine(DownloadAndPlayAudioCoroutine(audioUrl));
    }

    private IEnumerator DownloadAndPlayAudioCoroutine(string audioUrl)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.WAV))
        {
            ((DownloadHandlerAudioClip)www.downloadHandler).streamAudio = true;
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip downloadedClip = DownloadHandlerAudioClip.GetContent(www);
                voiceClipManager.AddClip(downloadedClip);
            }
        }
    }

    public void CleanupServerFiles()
    {
        if (Object != null && Object.HasStateAuthority)
        {
            RPC_RequestCleanup();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
    private void RPC_RequestCleanup()
    {
        if (Runner != null && Runner.IsRunning)
        {
            StartCoroutine(CleanupServerFilesCoroutine());
        }
    }

    private IEnumerator CleanupServerFilesCoroutine()
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(CLEANUP_URL, ""))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Cleanup Failed: {www.error}");
            }
            else
            {
                Debug.Log("Server voice files cleaned up successfully.");
            }
        }
    }

    private float[] Downsample(float[] data, int fromRate, int toRate)
    {
        float ratio = (float)fromRate / toRate;
        int newLength = Mathf.FloorToInt(data.Length / ratio);
        float[] result = new float[newLength];
        for (int i = 0; i < newLength; i++)
        {
            int srcIndex = Mathf.FloorToInt(i * ratio);
            result[i] = data[Mathf.Min(srcIndex, data.Length - 1)];
        }
        return result;
    }

    private byte[] ConvertToWavFileBytes(float[] samples, int sampleRate)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                short numChannels = 1;
                short bitsPerSample = 16;
                int byteRate = sampleRate * numChannels * (bitsPerSample / 8);
                short blockAlign = (short)(numChannels * (bitsPerSample / 8));

                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + samples.Length * 2);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
                writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                writer.Write(16);
                writer.Write((short)1);
                writer.Write(numChannels);
                writer.Write(sampleRate);
                writer.Write(byteRate);
                writer.Write(blockAlign);
                writer.Write(bitsPerSample);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                writer.Write(samples.Length * 2);

                for (int i = 0; i < samples.Length; i++)
                {
                    short s = (short)(Mathf.Clamp(samples[i], -1f, 1f) * 32767);
                    writer.Write(s);
                }
            }
            return stream.ToArray();
        }
    }
}