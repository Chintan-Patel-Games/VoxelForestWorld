using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Core.InputSystem;
using VoxelWorld.Networking;

namespace VoxelWorld.Networking
{
    public class NetworkInputSender : MonoBehaviour
    {
        public ColyseusManager manager;

        private float sendRate = 0.05f;
        private float timer;

        void Update()
        {
            if (manager == null) return;

            timer += Time.deltaTime;
            if (timer < sendRate) return;

            timer = 0;

            manager.SendInput(new Dictionary<string, object>
            {
                { "moveX", InputService.Instance.Move.x },
                { "moveY", InputService.Instance.Move.y },
                { "lookX", InputService.Instance.Look.x },
                { "jump", InputService.Instance.Jump },
                { "sprint", InputService.Instance.Sprint }
            });
        }
    }
}