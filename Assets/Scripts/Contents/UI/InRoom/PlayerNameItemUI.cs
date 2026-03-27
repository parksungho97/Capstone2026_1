using TMPro;
using UnityEngine;

public class PlayerNameItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;

    public void SetPlayerName(string playerName)
    {
        playerNameText.text = playerName;
    }

    public void ReadyEffect()
    {
        // Todo: 뭐 바뀐다던가 아무튼 준비 상태일 때 어떻게 ui가 바뀔건지
        Debug.Log("준비 효과");
    }
}