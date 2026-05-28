using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepClips;

    [Header("Footstep Guard")]
    [SerializeField] private float minInterval = 0.25f;
    [SerializeField] private float minMoveSpeed = 0.1f;

    private int index = 0;
    private float lastFootstepTime = -999f;
    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    public void PlayFootstep()
    {
        float currentSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;

        if (currentSpeed < minMoveSpeed)
            return;

        if (Time.time - lastFootstepTime < minInterval)
            return;

        if (audioSource == null || footstepClips == null || footstepClips.Length == 0)
            return;

        lastFootstepTime = Time.time;

        audioSource.PlayOneShot(footstepClips[index]);
        index = (index + 1) % footstepClips.Length;
    }
}