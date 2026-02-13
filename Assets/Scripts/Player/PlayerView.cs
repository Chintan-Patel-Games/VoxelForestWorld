using UnityEngine;

namespace VoxelWorld.Player
{
    public class PlayerView : MonoBehaviour
    {
        [Header("Camera Target")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        private CharacterController characterController;
        private PlayerController controller;

        private void Awake() => characterController = GetComponent<CharacterController>();

        public void SetController(PlayerController controller) => this.controller = controller;

        public void ApplySimulation(Vector3 serverPosition)
        {
            Vector3 delta = serverPosition - transform.position;
            characterController.Move(delta);
        }

        public void ApplyRotation(float yaw)
        {
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        public void TickRender() => controller?.TickUpdate();

        public Transform CameraTarget => CinemachineCameraTarget.transform;
    }
}