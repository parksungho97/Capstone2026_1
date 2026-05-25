using UnityEngine;

// Reads networked gauge values from jjhCapturePointController each frame and
// drives the corresponding CaptureGaugeUI. No ActivaterUIController helper is
// needed because the display logic is simple enough to live here directly.
public class CapturePointUIController : MonoBehaviour
{
    [Header("Capture Points")]
    [SerializeField] private CapturePointController[] mControllers;

    [Header("UI")]
    [SerializeField] private CaptureGaugeUI[] mGaugeUIs;

    private void Start()
    {
        Debug.Assert(mControllers.Length == mGaugeUIs.Length,
            "jjhCapturePointUIController: controller and UI arrays must be the same length.");

        for (int i = 0; i < mControllers.Length; i++)
        {
            Debug.Assert(mControllers[i] != null, $"jjhCapturePointUIController: mControllers[{i}] is null");
            Debug.Assert(mGaugeUIs[i] != null, $"jjhCapturePointUIController: mGaugeUIs[{i}] is null");
        }
    }

    private void Update()
    {
        if (!mReady)
        {
            mElapsed += Time.deltaTime;
            if (mElapsed >= 2f)
                mReady = true;
            return;
        }

        for (int i = 0; i < mControllers.Length; i++)
        {
            if (mControllers[i] == null) continue;
            if (!mControllers[i].Object.IsValid || mControllers[i].CapturePoint == null) continue;
            UpdateGauge(mControllers[i], mGaugeUIs[i]);
        }
    }

    private bool mReady = false;
    private float mElapsed = 0f;

    private static void UpdateGauge(
        CapturePointController controller,
        CaptureGaugeUI gaugeUI)
    {
        ECaptureState captureState = controller.CapturePoint.GetCaptureState();

        if (captureState == ECaptureState.None)
        {
            // ── Default state (Conqueror active) ──────────────────────────────
            // Show whichever team currently has the higher gauge value.
            float redGauge = controller.RedGauge;
            float blueGauge = controller.BlueGauge;

            if (redGauge >= blueGauge)
                gaugeUI.SetGauge(redGauge, 100f, EGuageColor.Red);
            else
                gaugeUI.SetGauge(blueGauge, 100f, EGuageColor.Blue);
        }
        else
        {
            // ── Captured state (Rebel active) ─────────────────────────────────
            // The captured team's bar starts at 100 and shrinks by the rebel gauge.
            // e.g. rebel gauge = 33 → display 67 out of 100 for the captured team.
            EGuageColor capturedColor = captureState == ECaptureState.Red ? EGuageColor.Red : EGuageColor.Blue;
            gaugeUI.SetGauge(100f - controller.RebelGauge, 100f, capturedColor);
        }
    }
}
