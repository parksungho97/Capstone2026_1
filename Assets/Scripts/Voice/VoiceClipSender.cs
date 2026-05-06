using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class VoiceClipSender : NetworkBehaviour
{
    [SerializeField] private VoiceClipManager voiceClipManager;

    private const int CHUNK_SIZE = 100;
    private const int SAMPLE_RATE = 16000; // 44100 → 16000

    private List<byte> _receiveBuffer = new();

    public void Initialize(VoiceClipManager manager)
    {
        voiceClipManager = manager;
    }

    public void SendVoice(float[] data)
    {
        // 샘플레이트 다운샘플 (44100 → 16000)
        float[] downsampled = Downsample(data, 44100, SAMPLE_RATE);

        // float[] → byte[] 압축
        byte[] compressed = CompressToBytes(downsampled);

        int totalChunks = Mathf.CeilToInt((float)compressed.Length / CHUNK_SIZE);

        for (int i = 0; i < totalChunks; i++)
        {
            int start = i * CHUNK_SIZE;
            int size = Mathf.Min(CHUNK_SIZE, compressed.Length - start);

            byte[] chunk = new byte[size];
            System.Array.Copy(compressed, start, chunk, 0, size);

            RPC_SendVoiceChunk(chunk, i == totalChunks - 1);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SendVoiceChunk(byte[] chunk, bool isLast)
    {
        _receiveBuffer.AddRange(chunk);

        if (!isLast) return;

        // byte[] → float[] 압축 해제
        float[] data = DecompressFromBytes(_receiveBuffer.ToArray());
        _receiveBuffer.Clear();

        AudioClip clip = AudioClip.Create("Voice", data.Length, 1, SAMPLE_RATE, false);
        clip.SetData(data, 0);

        voiceClipManager.AddClip(clip);
    }

    // ── 다운샘플 ────────────────────────────────────────────────────
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

    // ── 압축 ────────────────────────────────────────────────────────
    private byte[] CompressToBytes(float[] data)
    {
        byte[] bytes = new byte[data.Length * 2];
        for (int i = 0; i < data.Length; i++)
        {
            short s = (short)(Mathf.Clamp(data[i], -1f, 1f) * 32767);
            bytes[i * 2] = (byte)(s & 0xFF);
            bytes[i * 2 + 1] = (byte)((s >> 8) & 0xFF);
        }
        return bytes;
    }

    // ── 압축 해제 ────────────────────────────────────────────────────
    private float[] DecompressFromBytes(byte[] bytes)
    {
        float[] data = new float[bytes.Length / 2];
        for (int i = 0; i < data.Length; i++)
        {
            short s = (short)(bytes[i * 2] | (bytes[i * 2 + 1] << 8));
            data[i] = s / 32767f;
        }
        return data;
    }
}