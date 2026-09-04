using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [SerializeField] private float defaultFadeDuration = 0.6f;

    private Image _image;
    private bool _isFading;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureOverlay();
        ForceClear();
    }

    private void Start()
    {
        ForceClear();
        StartCoroutine(ForceClearNextFrame()); // <- 중요
    }

    private IEnumerator ForceClearNextFrame()
    {
        yield return null;
        ForceClear();
    }

    private void EnsureOverlay()
    {
        var canvasGO = new GameObject("FaderCanvas");
        canvasGO.transform.SetParent(transform, false);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        canvasGO.AddComponent<GraphicRaycaster>();

        var imgGO = new GameObject("FaderImage");
        imgGO.transform.SetParent(canvasGO.transform, false);

        _image = imgGO.AddComponent<Image>();
        _image.color = Color.black;

        var rt = _image.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private void ForceClear()
    {
        if (_image == null) return;
        SetAlpha(0f);
        _image.raycastTarget = false;
        _isFading = false;
    }

    public Coroutine FadeOut(float? duration = null) =>
        StartCoroutine(FadeRoutine(0f, 1f, duration ?? defaultFadeDuration));

    public Coroutine FadeIn(float? duration = null) =>
        StartCoroutine(FadeRoutine(1f, 0f, duration ?? defaultFadeDuration));

    private IEnumerator FadeRoutine(float from, float to, float duration)
    {
        if (_isFading) yield break;
        _isFading = true;

        _image.raycastTarget = true;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Lerp(from, to, Mathf.Clamp01(t / duration)));
            yield return null;
        }

        SetAlpha(to);
        _image.raycastTarget = false; // <- 항상 해제 (검정 고정 방지)
        _isFading = false;
    }

    private void SetAlpha(float a)
    {
        var c = _image.color;
        c.a = a;
        _image.color = c;
    }
}