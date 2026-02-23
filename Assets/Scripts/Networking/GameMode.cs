using UnityEngine;

public enum EResultType : byte
{
    Red,
    Blue,
    Draw
}
public class GameMode : MonoBehaviour
{
    [Header("Capture Points (Fixed Size: 3)")]
    [SerializeField] private Network.CapturePoint[] capturePoints = new Network.CapturePoint[3];
    
    public EResultType DecideGameResult()
    {
        int RedCount = 0;
        int BlueCount = 0;

        foreach (Network.CapturePoint capturePoint in capturePoints)
        {
            Network.ECaptureState eCaptureState = capturePoint.GetCaptureState();
            switch(eCaptureState)
            {
                case Network.ECaptureState.None:
                    break;
                case Network.ECaptureState.Red:
                    RedCount += 1;
                    break;
                case Network.ECaptureState.Blue:
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
    }
    private void Awake()
    {
        foreach(Network.CapturePoint capturePoint in capturePoints)
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
