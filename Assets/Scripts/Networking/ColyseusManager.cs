using Colyseus;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VoxelWorld.Core;
using VoxelWorld.Core.Utilities;

namespace VoxelWorld.Networking
{
    public class ColyseusManager : GenericMonoSingleton<ColyseusManager>
    {
        [Header("Player Prefab")]
        public GameObject playerPrefab;

        private Client client;
        private Room<ColyseusSchema.State> room;

        private bool localPlayerInitialized = false;

        private Dictionary<string, GameObject> spawnedPlayers = new();
        private Dictionary<string, NetworkTransform> networkTransforms = new();

        async void Start()
        {
            await Connect();
        }

        async Task Connect()
        {
            client = new Client("wss://voxelworld-server.onrender.com");

            room = await client.JoinOrCreate<ColyseusSchema.State>("my_room");

            Debug.Log("Connected to room!");

            room.OnLeave += (code) =>
            {
                Debug.LogError("ROOM DISCONNECTED! CODE: " + code);
            };

            room.OnStateChange += OnStateUpdated;
        }

        private void OnStateUpdated(ColyseusSchema.State state, bool isFirstState)
        {
            if (isFirstState)
                Debug.Log("Initial state received");

            foreach (var key in state.players.Keys)
            {
                string id = (string)key;
                var playerState = state.players[id];

                if (!spawnedPlayers.ContainsKey(id))
                {
                    GameObject playerObj = Instantiate(playerPrefab);
                    spawnedPlayers[id] = playerObj;

                    networkTransforms[id] = new NetworkTransform();

                    var view = playerObj.GetComponent<NetworkPlayerView>();
                    if (view == null)
                    {
                        Debug.LogError("NetworkPlayerView MISSING on Player Prefab!");
                        return;
                    }
                    view.NetworkTransform = networkTransforms[id];

                    var sender = playerObj.GetComponent<NetworkInputSender>();

                    if (id == room.SessionId)
                    {
                        Debug.Log("Local player spawned");

                        view.IsLocalPlayer = true;
                        playerObj.tag = "LocalPlayer";

                        if (sender != null)
                        {
                            sender.manager = this;
                            sender.enabled = true;
                        }
                    }
                    else
                    {
                        Debug.Log("Remote player spawned");

                        view.IsLocalPlayer = false;

                        if (sender != null)
                            sender.enabled = false;
                    }

                    Debug.Log("Spawned player: " + id);
                }

                networkTransforms[id].TargetPosition = new Vector3(
                    playerState.x,
                    playerState.y,
                    playerState.z
                );

                if (id == room.SessionId && !localPlayerInitialized)
                {
                    localPlayerInitialized = true;

                    Debug.Log("Registering local network player AFTER first server sync");

                    GameService.Instance.RegisterNetworkPlayer(
                        spawnedPlayers[id].transform
                    );
                }

                networkTransforms[id].TargetRotationY = playerState.rotY;
            }

            var existingIds = new List<string>(spawnedPlayers.Keys);

            foreach (var id in existingIds)
            {
                if (!state.players.ContainsKey(id))
                    StartCoroutine(RemovePlayerNextFrame(id));
            }
        }

        private IEnumerator RemovePlayerNextFrame(string id)
        {
            yield return null;

            if (spawnedPlayers.TryGetValue(id, out var obj))
            {
                if (obj != null)
                    Destroy(obj);

                spawnedPlayers.Remove(id);
                networkTransforms.Remove(id);
            }
        }

        public void SendInput(Dictionary<string, object> input)
        {
            if (room == null) return;
            room.Send("input", input);
        }

        public Room<ColyseusSchema.State> GetRoom() => room;
    }
}