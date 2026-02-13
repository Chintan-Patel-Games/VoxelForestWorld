using UnityEngine;

namespace VoxelWorld.Simulation.Player
{
    public class PlayerSimulation
    {
        public Vector3 Position;
        public float VerticalVelocity;
        public float RotationY;
        public bool Grounded;

        private float inputX;
        private float inputZ;
        private bool jumpPressed;

        private const float MoveSpeed = 5f;
        private const float Gravity = -20f;
        private const float JumpForce = 6f;
        private const float GroundHeight = 2f;

        public void ApplyInput(float moveX, float moveZ, bool jump)
        {
            inputX = moveX;
            inputZ = moveZ;
            jumpPressed = jump;
        }

        public void ApplyLook(float lookX)
        {
            RotationY += lookX * 100f;
        }

        public void Simulate(float dt)
        {
            // Horizontal Movement
            Vector3 horizontalMove = new Vector3(inputX, 0f, inputZ);
            Position += Quaternion.Euler(0f, RotationY, 0f) *
                        new Vector3(inputX, 0f, inputZ) * MoveSpeed * dt;

            // Jump
            if (jumpPressed && Grounded)
            {
                VerticalVelocity = JumpForce;
                Grounded = false;
            }

            // Gravity
            VerticalVelocity += Gravity * dt;
            Position.y += VerticalVelocity * dt;

            // TEMP Ground Check
            if (Position.y <= GroundHeight)
            {
                Position.y = GroundHeight;
                VerticalVelocity = 0f;
                Grounded = true;
            }

            // Reset Jump after use
            jumpPressed = false;
        }
    }
}