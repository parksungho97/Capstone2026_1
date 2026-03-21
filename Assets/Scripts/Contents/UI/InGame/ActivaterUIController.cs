using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivaterUIController : MonoBehaviour
{
    public void Initalize(Activater activater, Image image)
    {
        this.activater = activater; 
        this.image = image;
    }

    private void Update()
    {
        // NetworkBehaviour랑 시간차이 때문에 어쩔 수 없이 null체크
        if (activater == null)
            return;
        // 원래 이전 정보 저장해서 바뀌었을 때만 하면 되긴하는데 그냥 하자
        float showProgress = activater.GetGreaterProgress();
        image.fillAmount = (float)showProgress / 100.0f;
    }

    private Activater activater;
    private Image image;
}
