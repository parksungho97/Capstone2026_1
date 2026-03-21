using UnityEngine;

public enum EResultType : byte
{
    Red,
    Blue,
    Draw
}
public class CGameMode : MonoBehaviour
{
    [Header("Capture Points (Fixed Size: 3)")]
    [SerializeField] private CapturePoint[] capturePoints = new CapturePoint[3];
    
    // 현재 게임 결과 확인 가능
    public EResultType DecideGameResult()
    {
        int RedCount = 0;
        int BlueCount = 0;

        foreach (CapturePoint capturePoint in capturePoints)
        {
            ECaptureState eCaptureState = capturePoint.GetCaptureState();
            switch(eCaptureState)
            {
                case ECaptureState.None:
                    break;
                case ECaptureState.Red:
                    RedCount += 1;
                    break;
                case ECaptureState.Blue:
                    BlueCount += 1;
                    break;
            }
        }

        if(RedCount == BlueCount)
            return EResultType.Draw;
        else if(RedCount < BlueCount)
            return EResultType.Blue;
        else
            return EResultType.Red;

        /*
         * NetworkManager.Singleton.SceneManager.LoadScene(
                SceneNames.Result,
                LoadSceneMode.Single
            );
         */
    }

    private void Start()
    {
        foreach(CapturePoint capturePoint in capturePoints)
            Debug.Assert(capturePoint != null);
    }

    private void OnValidate()
    {
        // 배열 크기가 3이 아니면 강제로 3으로 재조정
        if (capturePoints != null && capturePoints.Length != 3)
        {
            Debug.LogWarning("GameMode: CapturePoints 배열 크기는 3으로 고정됩니다.");
            System.Array.Resize(ref capturePoints, 3);
        }
    }
}
