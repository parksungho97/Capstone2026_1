using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivaterUIController
{
    public ActivaterUIController(CapturePointController capturePointController, CaptureGaugeUI captureGaugeUI)
    {
        this.capturePointController = capturePointController; 
        this.captureGaugeUI = captureGaugeUI;
    }

    public void Update()
    {
        // NetworkBehaviour랑 시간차이 때문에 어쩔 수 없이 null체크
        if (capturePointController == null)
            return;
        // 원래 이전 정보 저장해서 바뀌었을 때만 하면 되긴하는데 그냥 하자
        float showProgress = capturePointController.Activater.GetGreaterProgress();
        ERequestType requestType = capturePointController.Activater.GetGreaterTeam();

        EGuageColor teamType = requestType == ERequestType.Red ? EGuageColor.Red : EGuageColor.Blue;
        captureGaugeUI.SetGauge(showProgress, 100.0f, teamType);
    }

    private CapturePointController capturePointController;
    private CaptureGaugeUI captureGaugeUI;
}
