using UnityEngine;

namespace VoxelWorld.Player
{
    [System.Serializable]
    public class PlayerModel
    {
        public Vector3 Position;
        public float VerticalVelocity;
        public float RotationY;
        public bool Grounded;

        //[Tooltip("Rotation speed of the character")]
        //public float RotationSpeed = 1f;

        //[Header("Cinemachine")]
        //[Tooltip("How far in degrees can you move the camera up")]
        //public float TopClamp = 90f;
        //[Tooltip("How far in degrees can you move the camera down")]
        //public float BottomClamp = -90f;
    }
}