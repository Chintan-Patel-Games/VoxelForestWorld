using UnityEngine;
using VoxelWorld.Core.InputSystem;
using VoxelWorld.Networking;

namespace VoxelWorld.Networking
{
    public class NetworkPlayerView : MonoBehaviour
    {
        public NetworkTransform NetworkTransform;
        public bool IsLocalPlayer;

        [Header("Camera Target")]
        public Transform CameraTarget;

        private float pitch;
        private float smoothing = 12f;

        void Update()
        {
            if (NetworkTransform == null)
                return;

            transform.position = Vector3.Lerp(
                transform.position,
                NetworkTransform.TargetPosition,
                15f * Time.deltaTime
            );

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, NetworkTransform.TargetRotationY, 0),
                15f * Time.deltaTime
            );

            // ONLY LOCAL PLAYER LOOKS
            if (!IsLocalPlayer)
                return;

            var input = InputService.Instance;

            Vector2 look = input.Look;

            pitch += look.y * 2f;
            pitch = Mathf.Clamp(pitch, -80f, 80f);

            if (CameraTarget != null)
                CameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }
}