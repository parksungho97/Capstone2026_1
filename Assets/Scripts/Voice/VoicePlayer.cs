using Fusion;
using UnityEngine;

public class VoicePlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] footStepClips;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Debug.Assert(audioSource);
    }

    public bool IsPlaying => audioSource.isPlaying;

    public void PlayRandom()
    {
        int index = VoiceClipManager.Instance.GetRandomIndex();
        
        if (index < 0) 
            return;

        AudioClip clip = VoiceClipManager.Instance.GetClipAt(index);
        if (clip == null) 
            return;

        audioSource.PlayOneShot(clip);
    }

    public void PlayFootStep()
    {
        if (audioSource.isPlaying)
            return;

        if (footStepClips.Length == 0)
            return;
        
        audioSource.PlayOneShot(footStepClips[index]);
        index = (index + 1) % footStepClips.Length;
    }

    private int index = 0;
}
