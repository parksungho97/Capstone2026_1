using System.Collections;
using UnityEngine;

public class InvincibilityBlinker : MonoBehaviour
{
    [SerializeField] private GameObject[] targets;
    [SerializeField] private float blinkInterval = 0.1f;

    private Coroutine blinkCoroutine;

    public void StartBlink(float duration)
    {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkCoroutine(duration));
    }

    private IEnumerator BlinkCoroutine(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            SetVisible(false);
            yield return new WaitForSeconds(blinkInterval);
            SetVisible(true);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval * 2f;
        }

        SetVisible(true);
        blinkCoroutine = null;
    }

    private void SetVisible(bool bVisible)
    {
        if (targets.Length <= 0)
            return;

        foreach (var target in targets)
            target.SetActive(bVisible);
    }
}
