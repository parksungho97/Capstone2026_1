using System;
using UnityEngine;
using UnityEngine.UI;

public class CapturePointManager : MonoBehaviour
{
    [Header("Capture Points (Fixed Size: 3)")]
    [SerializeField] private CapturePointController[] capturePointControllers;

    [Header("UI (Fixed Size: 3)")]
    [SerializeField] private ActivaterUIController[] activaterUIControllers;
    [SerializeField] private Image[]                 activateProgressImages;

    public void Initialize()
    {
        for (int i = 0; i < capturePointControllers.Length; i++)
        {
            Debug.Assert(capturePointControllers[i] != null, $"CapturePointManager: capturePointControllers[{i}] is null");
            Debug.Assert(activaterUIControllers[i]   != null, $"CapturePointManager: activaterUIControllers[{i}] is null");
            Debug.Assert(activateProgressImages[i]   != null, $"CapturePointManager: activateProgressImages[{i}] is null");

            activaterUIControllers[i].Initalize(capturePointControllers[i].Activater, activateProgressImages[i]);
        }
    }

    public bool IsAllCaptured(out ECaptureState[] captureStates)
    {
        ECaptureState[] states = new ECaptureState[capturePointControllers.Length];
        captureStates = states;
        int index = 0;
        foreach (CapturePointController controller in capturePointControllers)
        {
            if (controller.CapturePoint.GetCaptureState() == ECaptureState.None)
                return false;
            else
                states[index++] = controller.CapturePoint.GetCaptureState();
        }
        return true;
    }

}
