using System.Collections.Generic;
using UnityEngine;

public class VoiceClipGenerator : MonoBehaviour
{
    [SerializeField] private VoiceClipSender voiceClipSender;
    [SerializeField] private float maxRecordTime = 5f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float percent = 0.8f;

    private const int SAMPLE_RATE = 44100;
    private const float THRESHOLD = 0.02f;
    private const float SILENCE_TIMEOUT = 0.5f;

    private AudioClip _micClip;
    private List<float> _buffer = new();
    private bool _isRecording = false;
    private bool _isCooldown = false;
    private float _silenceTimer = 0f;
    private float _recordTimer = 0f;
    private float _cooldownTimer = 0f;
    private int _lastSamplePos = 0;

    private void Start()
    {
        Debug.Assert(voiceClipSender);
        _micClip = Microphone.Start(null, true, 60, SAMPLE_RATE);
    }

    private void Update()
    {
        // 쿨다운 처리
        if (_isCooldown)
        {
            _cooldownTimer += Time.deltaTime;
            if (_cooldownTimer >= cooldown)
            {
                _isCooldown = false;
                _cooldownTimer = 0f;
            }
            return;
        }

        int currentPos = Microphone.GetPosition(null);
        if (currentPos == _lastSamplePos) return;

        int sampleCount = currentPos - _lastSamplePos;
        if (sampleCount < 0) sampleCount += _micClip.samples;

        float[] samples = new float[sampleCount];
        _micClip.GetData(samples, _lastSamplePos);
        _lastSamplePos = currentPos;

        float maxVal = 0f;
        foreach (var s in samples) maxVal = Mathf.Max(maxVal, Mathf.Abs(s));

        if (_isRecording)
        {
            _recordTimer += Time.deltaTime;
            _buffer.AddRange(samples);

            // 최대 녹음 시간 초과
            if (_recordTimer >= maxRecordTime)
            {
                SaveClip();
                return;
            }

            // 무음 감지
            if (maxVal <= THRESHOLD)
            {
                _silenceTimer += Time.deltaTime;
                if (_silenceTimer >= SILENCE_TIMEOUT)
                {
                    if (_recordTimer >= maxRecordTime * percent)
                        SaveClip();
                    else
                        DiscardClip();
                }
            }
            else
            {
                _silenceTimer = 0f;
            }
        }
        else
        {
            if (maxVal > THRESHOLD)
            {
                _isRecording = true;
                _recordTimer = 0f;
                _silenceTimer = 0f;
                _buffer.AddRange(samples);
            }
        }
    }

    private void SaveClip()
    {
        if (_buffer.Count > 0)
            voiceClipSender.SendVoice(_buffer.ToArray());

        ResetRecording();
    }

    private void DiscardClip()
    {
        ResetRecording();
    }

    private void ResetRecording()
    {
        _buffer.Clear();
        _isRecording = false;
        _silenceTimer = 0f;
        _recordTimer = 0f;
        _isCooldown = true;
        _cooldownTimer = 0f;
    }

    private void OnDestroy()
    {
        Microphone.End(null);
    }
}