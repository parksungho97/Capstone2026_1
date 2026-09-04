using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoSceneController : MonoBehaviour
{
    private bool _requested;

    private void Update()
    {
        if (_requested) return;

        if (Input.anyKeyDown)
        {
            _requested = true;
            SceneFlowManager.Instance.GoToLobby();
        }
    }
}