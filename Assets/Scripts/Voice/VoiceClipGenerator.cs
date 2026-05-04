using System.Collections.Generic;
using UnityEngine;

public class VoiceClipGenerator : MonoBehaviour
{
    [SerializeField] private VoiceClipSender voiceClipSender;
    private const int SAMPLE_RATE = 44100;
    private const float THRESHOLD = 0.02f;      // 데시벨 임계값
    private const float SILENCE_TIMEOUT = 0.5f; // 무음 지속 시간

    private AudioClip _micClip;
    private List<float> _buffer = new();
    private bool _isRecording = false;
    private float _silenceTimer = 0f;
    private int _lastSamplePos = 0;

    private void Start()
    {
        Debug.Assert(voiceClipSender);
        _micClip = Microphone.Start(null, true, 60, SAMPLE_RATE); // 60초 루프 녹음
    }

    private void Update()
    {
        int currentPos = Microphone.GetPosition(null);
        if (currentPos == _lastSamplePos) 
            return;

        // 새로 들어온 샘플 읽기
        int sampleCount = currentPos - _lastSamplePos;
        if (sampleCount < 0) sampleCount += _micClip.samples;

        float[] samples = new float[sampleCount];
        _micClip.GetData(samples, _lastSamplePos);
        _lastSamplePos = currentPos;

        // 최대 진폭 체크
        float maxVal = 0f;
        foreach (var s in samples) maxVal = Mathf.Max(maxVal, Mathf.Abs(s));

        if (maxVal > THRESHOLD)
        {
            _isRecording = true;
            _silenceTimer = 0f;
            _buffer.AddRange(samples);
        }
        else if (_isRecording)
        {
            _silenceTimer += Time.deltaTime;
            _buffer.AddRange(samples);

            if (_silenceTimer >= SILENCE_TIMEOUT)
            {
                SaveClip();
                _isRecording = false;
                _silenceTimer = 0f;
            }
        }
    }

    private void SaveClip()
    {
        if (_buffer.Count == 0) return;

        voiceClipSender.SendVoice(_buffer.ToArray());
        _buffer.Clear();
    }

    private void OnDestroy()
    {
        Microphone.End(null);
    }
}
