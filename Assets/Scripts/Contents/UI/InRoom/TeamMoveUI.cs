using UnityEngine;

public class TeamMoveUI : MonoBehaviour
{
    [SerializeField] private Transform redTeamParent;
    [SerializeField] private Transform blueTeamParent;
    [SerializeField] private Transform myPlayerNameUI;

    public void MoveToRed()
    {
        myPlayerNameUI.SetParent(redTeamParent, false);
    }

    public void MoveToBlue()
    {
        myPlayerNameUI.SetParent(blueTeamParent, false);
    }
}