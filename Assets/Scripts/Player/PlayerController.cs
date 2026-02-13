using UnityEngine;
using VoxelWorld.Core.InputSystem;

namespace VoxelWorld.Player
{
    public class PlayerController
    {
        private PlayerModel model;
        private PlayerView view;

        // Footsteps
        private bool wasMoving = false;
        private bool wasSprinting = false;

        private bool Grounded;

        public void TickUpdate()
        {
            // Client Rendering Only
            view.ApplySimulation(model.Position);
            view.ApplyRotation(model.RotationY);
        }

        private void HandleFootsteps(bool isMoving, bool isSprinting)
        {
            // Must be grounded to play footsteps OR If player is not moving -> stop footsteps
            if (!isMoving || !Grounded)
            {
                GlobalSoundService.Instance.SoundService.StopFootsteps();
                wasMoving = false;
                return;
            }

            // If player JUST started moving -> start footsteps
            if (isMoving && !wasMoving)
            {
                GlobalSoundService.Instance.SoundService.StartFootsteps(isSprinting);
            }

            // If player changed from walk -> sprint or sprint -> walk
            if (wasSprinting != isSprinting && wasMoving)
            {
                GlobalSoundService.Instance.SoundService.UpdateFootstepsMode(isSprinting);
            }

            wasMoving = isMoving;
            wasSprinting = isSprinting;
        }
    }
}