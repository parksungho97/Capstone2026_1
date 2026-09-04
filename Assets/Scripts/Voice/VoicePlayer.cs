using Fusion;
using System.Collections;
using UnityEngine;

public class VoicePlayer : NetworkBehaviour
{
    [SerializeField] private AudioClip[] footStepClips;

    private AudioSource audioSource;
    private int index;
    private float footStepTimer;
    private bool playingFootSteps;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Debug.Assert(audioSource);
    }

    private void Update()
    {
        if (!playingFootSteps || footStepClips.Length == 0)
            return;

        footStepTimer -= Time.deltaTime;
        if (footStepTimer <= 0f)
            PlayFootStep();
    }

    public bool IsPlaying => audioSource.isPlaying;

    public void StartFootSteps()
    {
        if (playingFootSteps) 
            return;
        playingFootSteps = true;
        footStepTimer = 0f;
    }

    public void StopFootSteps()
    {
        playingFootSteps = false;
    }

    public void PlayRandom()
    {
        int i = VoiceClipManager.Instance.GetRandomIndex();
        if (i < 0) return;

        AudioClip clip = VoiceClipManager.Instance.GetClipAt(i);
        if (clip == null) return;

        audioSource.PlayOneShot(clip);
    }

    public void PlayRandomOnAll()
    {
        if (!Object.HasStateAuthority) return;

        int i = VoiceClipManager.Instance.GetRandomIndex();
        if (i < 0) return;

        RPC_PlayRandom(i);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayRandom(int clipIndex)
    {
        AudioClip clip = VoiceClipManager.Instance.GetClipAt(clipIndex);
        if (clip == null) return;
        audioSource.PlayOneShot(clip);
    }

    public void PlayAttackClip(int id)
    {
        AudioClip clip = VoiceClipManager.Instance.GetAttackClipAt(id);
        if (clip == null) return;
        audioSource.PlayOneShot(clip);
    }

    public void DisableAfterPlaying()
    {
        StopFootSteps();
        StartCoroutine(DisableAfterPlayingRoutine());
    }

    private IEnumerator DisableAfterPlayingRoutine()
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        audioSource.enabled = false;
    }

    private void PlayFootStep()
    {
        if (footStepClips.Length == 0)
            return;

        AudioClip clip = footStepClips[index];
        audioSource.PlayOneShot(clip);
        footStepTimer = clip.length;
        index = (index + 1) % footStepClips.Length;
    }
}
