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
        // 원래 이전 정보 저장해서 바뀌었을 때만 하면 되긴하는데 그냥 하자
        float showProgress = activater.GetGreaterProgress();
        image.fillAmount = (float)showProgress / 100.0f;
    }

    private Activater activater;
    private Image image;
}
