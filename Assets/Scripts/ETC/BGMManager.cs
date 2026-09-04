using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Serializable]
    public class SceneBGMEntry
    {
        public string sceneName;
        public AudioClip clip;
    }

    [Header("Scene BGM")]
    [SerializeField] private SceneBGMEntry[] sceneBGMs;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.5f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void Start() => HandleSceneBGM(SceneManager.GetActiveScene().name);

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleSceneBGM(scene.name);
    }

    private void HandleSceneBGM(string sceneName)
    {
        AudioClip clip = FindClipForScene(sceneName);
        if (clip == null)
        {
            StopBGM();
            return;
        }
        PlayBGM(clip);
    }

    private AudioClip FindClipForScene(string sceneName)
    {
        if (sceneBGMs == null) return null;
        foreach (var entry in sceneBGMs)
        {
            if (entry.sceneName == sceneName)
                return entry.clip;
        }
        return null;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        // 같은 클립이 이미 재생 중이면 재시작하지 않음
        if (audioSource.clip == clip && audioSource.isPlaying) return;

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopBGM()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
            audioSource.volume = volume;
    }
}
