using UnityEngine;

public class RoomTeamDashboardUI : MonoBehaviour
{
    [SerializeField] private Transform redTeamListParent;
    [SerializeField] private Transform blueTeamListParent;
    [SerializeField] private GameObject playerNameItemPrefab;

    public PlayerNameItemUI AddRedPlayer(string playerName)
    {
        GameObject item = Instantiate(playerNameItemPrefab, redTeamListParent);
        PlayerNameItemUI itemUI = item.GetComponent<PlayerNameItemUI>();

        Debug.Assert(itemUI);
        itemUI.SetPlayerName(playerName);

        return itemUI;
    }

    public PlayerNameItemUI AddBluePlayer(string playerName)
    {
        GameObject item = Instantiate(playerNameItemPrefab, blueTeamListParent);
        PlayerNameItemUI itemUI = item.GetComponent<PlayerNameItemUI>();

        Debug.Assert(itemUI);
        itemUI.SetPlayerName(playerName);

        return itemUI;
    }

    public void ClearRedTeam()
    {
        for (int i = redTeamListParent.childCount - 1; i >= 0; i--)
        {
            Destroy(redTeamListParent.GetChild(i).gameObject);
        }
    }

    public void ClearBlueTeam()
    {
        for (int i = blueTeamListParent.childCount - 1; i >= 0; i--)
        {
            Destroy(blueTeamListParent.GetChild(i).gameObject);
        }
    }

    public void ClearAll()
    {
        ClearRedTeam();
        ClearBlueTeam();
    }
}