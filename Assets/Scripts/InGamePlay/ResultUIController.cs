using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class ResultUIController : MonoBehaviour
    {
        [SerializeField] private GameObject mVictoryImage = null;
        [SerializeField] private GameObject mDefeatImage = null;
        [SerializeField] private float mReturnDelay = 5.0f;

        private void Start()
        {
            if (mVictoryImage != null)
                mVictoryImage.SetActive(false);

            if (mDefeatImage != null)
                mDefeatImage.SetActive(false);

            StartCoroutine(CoShowResultAndReturn());
        }

        private IEnumerator CoShowResultAndReturn()
        {
            yield return null;
            yield return null;
            yield return null;

            if (RoomManager.Instance == null)
                yield break;

            if (NetworkManager.Singleton == null)
                yield break;

            if (RoomManager.Instance.TryGetMyRoomId(out int roomId) == false)
            {
                Debug.LogError("[ResultUI] failed to get my roomId");
                yield break;
            }

            ulong localPlayerId = NetworkManager.Singleton.LocalClientId;

            if (RoomManager.Instance.TryGetPlayerTeam(localPlayerId, roomId, out Team myTeam) == false)
            {
                Debug.LogError($"[ResultUI] failed to get my team. clientId={localPlayerId}, roomId={roomId}");
                yield break;
            }

            GameWinner winnerTeam = RoomManager.Instance.WinnerTeam;

            bool isVictory =
                (winnerTeam == GameWinner.Red && myTeam == Team.Red) ||
                (winnerTeam == GameWinner.Blue && myTeam == Team.Blue);

            if (isVictory)
            {
                if (mVictoryImage != null)
                {
                    mVictoryImage.SetActive(true);
                    mVictoryImage.transform.SetAsLastSibling();
                }

                if (mDefeatImage != null)
                    mDefeatImage.SetActive(false);
            }
            else
            {
                if (mVictoryImage != null)
                    mVictoryImage.SetActive(false);

                if (mDefeatImage != null)
                {
                    mDefeatImage.SetActive(true);
                    mDefeatImage.transform.SetAsLastSibling();
                }
            }

            yield return new WaitForSeconds(mReturnDelay);

            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(
                    SceneNames.Room,
                    LoadSceneMode.Single
                );
            }
        }
    }
}