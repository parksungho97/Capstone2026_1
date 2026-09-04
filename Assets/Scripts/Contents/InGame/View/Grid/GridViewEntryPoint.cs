using UnityEngine;

namespace jjh
{
    public class jjhEntryPoint : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private GameObject player;
        [SerializeField] private CameraController cameraController;

        private void Start()
        {
            cameraController.SetTarget(player.transform);
            
        }

    }
}