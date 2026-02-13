using UnityEngine;
using VoxelWorld.Core.InputSystem;

namespace VoxelWorld.Player
{
    public class PlayerController
    {
        private PlayerModel model;
        private PlayerView view;
        private Transform cameraTarget;
        private bool isLocalPlayer;

        //// Footsteps
        //private bool wasMoving = false;
        //private bool wasSprinting = false;

        //private bool Grounded;

        //// Camera Rotation
        //private float cinemachinePitch;
        //private float smoothing = 12f;
        //private const float threshold = 0.01f;
        //private Vector2 currentLook;

        public PlayerController(PlayerModel model, PlayerView view, Transform cameraTarget, bool isLocalPlayer)
        {
            this.model = model;
            this.view = view;
            this.cameraTarget = cameraTarget;
            this.isLocalPlayer = isLocalPlayer;
        }

        public void TickUpdate()
        {
            // Client Rendering Only
            view.ApplySimulation(model.Position);
            view.ApplyRotation(model.RotationY);
        }

        //public void TickLateUpdate(Transform playerTransform)
        //{
        //    if (!isLocalPlayer) return;

        //    CameraRotation(playerTransform);
        //}

        //private void HandleFootsteps(bool isMoving, bool isSprinting)
        //{
        //    // Must be grounded to play footsteps OR If player is not moving -> stop footsteps
        //    if (!isMoving || !Grounded)
        //    {
        //        GlobalSoundService.Instance.SoundService.StopFootsteps();
        //        wasMoving = false;
        //        return;
        //    }

        //    // If player JUST started moving -> start footsteps
        //    if (isMoving && !wasMoving)
        //    {
        //        GlobalSoundService.Instance.SoundService.StartFootsteps(isSprinting);
        //    }

        //    // If player changed from walk -> sprint or sprint -> walk
        //    if (wasSprinting != isSprinting && wasMoving)
        //    {
        //        GlobalSoundService.Instance.SoundService.UpdateFootstepsMode(isSprinting);
        //    }

        //    wasMoving = isMoving;
        //    wasSprinting = isSprinting;
        //}

        //private void CameraRotation(Transform playerTransform)
        //{
        //    if (InputService.Instance == null) return;

        //    var input = InputService.Instance;

        //    Vector2 look = input.Look;

        //    // Clamp look input if frame stutter occurs
        //    if (Time.deltaTime > 0.04f) // ~25+ FPS threshold
        //        look *= (0.04f / Time.deltaTime);

        //    // Apply smoothing
        //    look = Vector2.Lerp(currentLook, look, smoothing * Time.deltaTime);
        //    currentLook = look;

        //    if (look.sqrMagnitude >= threshold)
        //    {
        //        float delta = Time.deltaTime;

        //        // vertical rotation (camera pitch)
        //        cinemachinePitch += look.y * model.RotationSpeed * delta;
        //        cinemachinePitch = Mathf.Clamp(cinemachinePitch, model.BottomClamp, model.TopClamp);

        //        cameraTarget.localRotation = Quaternion.Euler(cinemachinePitch, 0f, 0f);

        //        // horizontal rotation (player yaw)
        //        playerTransform.Rotate(Vector3.up * look.x * model.RotationSpeed * delta);
        //    }
        //}
    }
}