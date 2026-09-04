using System;
using TMPro;
using UnityEngine;

public class CreateRoomPopupUI : MonoBehaviour
{
    [SerializeField] private GameObject popupObject;
    [SerializeField] private TMP_InputField roomNameInputField;

    public Action<string> OnCreateRoomRequested;

    public void OpenPopup()
    {
        popupObject.SetActive(true);
        roomNameInputField.text = "";
        roomNameInputField.ActivateInputField();
    }

    public void ClosePopup()
    {
        popupObject.SetActive(false);
    }

    public void OnClickCreateRoom()
    {
        string roomName = roomNameInputField.text.Trim();

        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("방 이름이 비어 있습니다.");
            return;
        }

        Debug.Log("방 생성 요청: " + roomName);

        // 입력한 방 이름을 바깥으로 전달
        OnCreateRoomRequested?.Invoke(roomName);

        // 팝업 닫기
        popupObject.SetActive(false);
    }
}