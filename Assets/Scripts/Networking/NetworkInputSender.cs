using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using VoxelWorld.Core.InputSystem;
using VoxelWorld.Networking;

namespace VoxelWorld.Networking
{
    public class NetworkInputSender : MonoBehaviour
    {
        public ColyseusManager manager;
        private bool jumpQueued = false;

        private void Awake() => manager = ColyseusManager.Instance;

        private void Update()
        {
            if (manager == null) return;
            if (InputService.Instance == null) return;

            // Queue jump when pressed
            if (InputService.Instance.JumpPressed)
            {
                jumpQueued = true;
                InputService.Instance.JumpPressed = false;
            }

            manager.SendInput(new Dictionary<string, object>
            {
                { "moveX", InputService.Instance.Move.x },
                { "moveY", InputService.Instance.Move.y },
                { "lookX", InputService.Instance.Look.x },
                { "jump", jumpQueued },
                { "sprint", InputService.Instance.Sprint }
            });

            jumpQueued = false; // Reset jump after sending input
        }
    }
}