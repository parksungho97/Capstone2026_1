using Fusion;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CapturePointManager : NetworkBehaviour
{
    [Header("Capture Points (Fixed Size: 3)")]
    [SerializeField] private CapturePointController[] capturePointControllers;

    [Header("UI (Fixed Size: 3)")]
    [SerializeField] private CaptureGaugeUI[] captureGaugeUI;

    private ActivaterUIController[] activaterUIControllers;

    private void Start()
    {
        activaterUIControllers = new ActivaterUIController[capturePointControllers.Length];

        for (int i = 0; i < capturePointControllers.Length; i++)
        {
            Debug.Assert(capturePointControllers[i] != null, $"CapturePointManager: capturePointControllers[{i}] is null");
            Debug.Assert(captureGaugeUI[i] != null, $"CapturePointManager: activateProgressImages[{i}] is null");

            activaterUIControllers[i] = new ActivaterUIController(capturePointControllers[i], captureGaugeUI[i]);
        }
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        foreach(ActivaterUIController activaterUIController in activaterUIControllers)
        {
            activaterUIController.Update();
        }
    }

    public bool IsAllCaptured(out ECaptureState[] captureStates)
    {
        ECaptureState[] states = new ECaptureState[capturePointControllers.Length];
        captureStates = states;
        int index = 0;
        foreach (CapturePointController capturePointController in capturePointControllers)
        {
            if (capturePointController.CapturePoint.GetCaptureState() == ECaptureState.None)
                return false;
            else
                states[index++] = capturePointController.CapturePoint.GetCaptureState();
        }
        return true;
    }

}
