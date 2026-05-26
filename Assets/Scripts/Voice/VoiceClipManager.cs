using System.Collections.Generic;
using UnityEngine;

public class VoiceClipManager : MonoBehaviour
{
    public static VoiceClipManager Instance { get; private set; }

    private readonly List<AudioClip> clips = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[VoiceClipManager] 중복된 매니저가 감지되어 제거되었습니다! 오브젝트: {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddClip(AudioClip clip)
    {
        if (clip == null) return;

        clips.Add(clip);
    }

    public int GetRandomIndex()
    {
        if (clips.Count == 0) return -1;
        return Random.Range(0, clips.Count);
    }

    public AudioClip GetClipAt(int index)
    {
        if (index < 0 || index >= clips.Count) return null;
        return clips[index];
    }

    public AudioClip GetRandomClipOrNull()
    {
        int index = GetRandomIndex();
        return index >= 0 ? GetClipAt(index) : null;
    }
}