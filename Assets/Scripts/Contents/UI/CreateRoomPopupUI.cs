using TMPro;
using UnityEngine;

public class CreateRoomPopupUI : MonoBehaviour
{
    [SerializeField] private GameObject popupObject;
    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private RoomListUIManager roomListUIManager;

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

        if (roomListUIManager == null)
        {
            Debug.LogError("RoomListUIManager가 연결되지 않았습니다.");
            return;
        }

        Debug.Log("방 생성: " + roomName);

        // 대시보드에 방 추가
        roomListUIManager.AddRoom(roomName, 1, 4);

        // 팝업 닫기
        popupObject.SetActive(false);
    }
}