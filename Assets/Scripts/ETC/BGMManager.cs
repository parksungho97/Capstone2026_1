using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Header("BGM")]
    [SerializeField] private AudioClip bgmClip;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.5f;

    [Header("Stop Scene")]
    [SerializeField] private string inGameSceneName = "inGame";

    private AudioSource audioSource;

    private void Awake()
    {
        // 이미 살아 있는 BGMManager가 있으면 새로 만들어진 것은 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 씬이 바뀌어도 유지
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        // AudioSource 기본 설정
        audioSource.clip = bgmClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f; // 2D 사운드
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        HandleSceneBGM(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleSceneBGM(scene.name);
    }

    private void HandleSceneBGM(string sceneName)
    {
        if (sceneName == inGameSceneName)
        {
            StopBGM();
            return;
        }

        PlayBGM();
    }

    public void PlayBGM()
    {
        if (audioSource == null)
            return;

        if (bgmClip == null)
        {
            Debug.LogWarning("[BGMManager] BGM Clip이 연결되지 않았습니다.");
            return;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopBGM()
    {
        if (audioSource == null)
            return;

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);

        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}