using UnityEngine;

namespace VoxelWorld.Player
{
    public class PlayerView : MonoBehaviour
    {
        [Header("Camera Target")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        private CharacterController characterController;

        private void Awake() => characterController = GetComponent<CharacterController>();

        public void ApplySimulation(Vector3 serverPosition)
        {
            Vector3 delta = serverPosition - transform.position;
            characterController.Move(delta);
        }

        public void ApplyRotation(float yaw)
        {
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        public Transform CameraTarget => CinemachineCameraTarget.transform;
    }
}