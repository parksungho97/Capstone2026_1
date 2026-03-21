using TMPro;
using UnityEngine;

public class PlayerNameItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;

    public void SetPlayerName(string playerName)
    {
        playerNameText.text = playerName;
    }
}