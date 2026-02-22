using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance { get; private set; }

    [SerializeField] private float fadeOutDuration = 0.6f;
    [SerializeField] private float fadeInDuration = 0.35f;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GoToLobby() => LoadWithFade(SceneNames.Lobby);

    public void LoadWithFade(string sceneName)
    {
        StartCoroutine(LoadWithFadeRoutine(sceneName));
    }

    private IEnumerator LoadWithFadeRoutine(string sceneName)
    {
        // 페이더 없으면 그냥 로드
        if (ScreenFader.Instance == null)
        {
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        // FadeOut -> Load -> FadeIn
        yield return ScreenFader.Instance.FadeOut(fadeOutDuration);

        SceneManager.LoadScene(sceneName);

        // 씬 로드가 끝나고 한 프레임 기다렸다가 FadeIn
        yield return null;
        yield return ScreenFader.Instance.FadeIn(fadeInDuration);
    }
}