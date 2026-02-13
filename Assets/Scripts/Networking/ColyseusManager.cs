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

        // SAFE REMOVE LIST
        private List<string> playersToRemove = new();

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
            foreach (var key in state.players.Keys)
            {
                string id = (string)key;
                var playerState = state.players[id];

                // SPAWN PLAYER IF NOT EXISTS
                if (!spawnedPlayers.ContainsKey(id))
                {
                    GameObject playerObj = Instantiate(playerPrefab);

                    // Set initial transform from server BEFORE first frame
                    playerObj.transform.position = new Vector3(
                        playerState.x,
                        playerState.y,
                        playerState.z
                    );

                    playerObj.transform.rotation = Quaternion.Euler(
                        0f,
                        playerState.rotY,
                        0f
                    );

                    spawnedPlayers[id] = playerObj;
                    networkTransforms[id] = new NetworkTransform();

                    var view = playerObj.GetComponent<NetworkPlayerView>();
                    var sender = playerObj.GetComponent<NetworkInputSender>();

                    // LOCAL PLAYER
                    if (id == room.SessionId)
                    {
                        Debug.Log("Local player spawned");

                        view.IsLocalPlayer = true;
                        playerObj.tag = "LocalPlayer";
                        view.NetworkTransform = networkTransforms[id];

                        if (sender != null)
                        {
                            sender.manager = this;
                            sender.enabled = true;
                        }
                    }
                    // REMOTE PLAYER
                    else
                    {
                        Debug.Log("Remote player spawned");

                        view.IsLocalPlayer = false;
                        view.NetworkTransform = networkTransforms[id];

                        if (sender != null)
                            sender.enabled = false;
                    }

                    Debug.Log("Spawned player: " + id);
                }

                // UPDATE TARGET TRANSFORM
                networkTransforms[id].TargetPosition = new Vector3(
                    playerState.x,
                    playerState.y,
                    playerState.z
                );

                networkTransforms[id].TargetRotationY = playerState.rotY;

                // REGISTER LOCAL PLAYER ONLY ONCE
                if (id == room.SessionId)
                {
                    if (!localPlayerInitialized)
                    {
                        localPlayerInitialized = true;

                        Debug.Log("Registering local network player AFTER first sync");

                        GameService.Instance.RegisterNetworkPlayer(
                            spawnedPlayers[id].transform
                        );
                    }
                }
            }

            // MARK DISCONNECTED PLAYERS
            playersToRemove.Clear();

            foreach (var id in spawnedPlayers.Keys)
            {
                if (!state.players.ContainsKey(id))
                    playersToRemove.Add(id);
            }

            if (playersToRemove.Count > 0)
                StartCoroutine(RemovePlayersSafely());
        }

        // SAFE REMOVE AFTER PATCH FINISH
        private IEnumerator RemovePlayersSafely()
        {
            yield return new WaitForEndOfFrame();

            foreach (var id in playersToRemove)
            {
                if (spawnedPlayers.TryGetValue(id, out var obj))
                {
                    if (obj != null)
                        Destroy(obj);

                    spawnedPlayers.Remove(id);
                    networkTransforms.Remove(id);

                    Debug.Log("Removed player safely: " + id);
                }
            }

            playersToRemove.Clear();
        }

        public void SendInput(Dictionary<string, object> input)
        {
            if (room == null) return;
            room.Send("input", input);
        }

        public Room<ColyseusSchema.State> GetRoom() => room;

        private void OnDestroy()
        {
            if (room != null)
                room.OnStateChange -= OnStateUpdated;
        }
    }
}