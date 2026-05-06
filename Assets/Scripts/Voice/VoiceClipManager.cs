using System.Collections.Generic;
using UnityEngine;

public class VoiceClipManager : MonoBehaviour
{
    public void AddClip(AudioClip clip)
    {
        clips.Add(clip);

        Debug.Log($"AddClip: {clips.Count}");
    }

    public AudioClip GetRandomClipOrNull()
    {
        Debug.Log($"GetRandomClipOrNull: {clips.Count}");
        if (clips.Count == 0) 
            return null;

        int index = Random.Range(0, clips.Count);
        return clips[index];
    }

    private List<AudioClip> clips = new();
}
