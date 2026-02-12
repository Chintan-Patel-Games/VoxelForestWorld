using Colyseus;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VoxelWorld.Core;
using VoxelWorld.Player;

namespace VoxelWorld.Networking
{
    public class ColyseusManager : MonoBehaviour
    {
        public GameObject playerPrefab;

        private Client client;
        private Room<ColyseusSchema.State> room;

        private Dictionary<string, GameObject> spawnedPlayers = new();

        async void Start()
        {
            await Connect();
        }

        async Task Connect()
        {
            client = new Client("ws://localhost:2567");

            room = await client.JoinOrCreate<ColyseusSchema.State>("my_room");

            Debug.Log("Connected to room!");

            room.OnStateChange += (state, isFirstState) =>
            {
                if (isFirstState)
                {
                    Debug.Log("Initial state received");
                }

                // Spawn / Update players
                foreach (var key in state.players.Keys)
                {
                    string id = (string)key;
                    var playerState = state.players[id];

                    if (!spawnedPlayers.ContainsKey(id))
                    {
                        GameObject playerObj = Instantiate(playerPrefab);
                        spawnedPlayers[id] = playerObj;

                        if (id == room.SessionId)
                        {
                            Debug.Log("Spawned LOCAL player: " + id);

                            StartCoroutine(FixSpawnHeight(playerState.x, playerState.z));

                            // Register with GameService
                            //GameService.Instance.RegisterNetworkPlayer(playerObj.transform);

                            StartCoroutine(InitializePlayerAfterSpawn(playerObj));
                        }
                        else
                        {
                            Debug.Log("Spawned REMOTE player: " + id);

                            // Disable movement logic on remote player
                            var view = playerObj.GetComponent<PlayerView>();
                            if (view != null)
                            {
                                view.enabled = false; // disables Update loop if any
                            }

                            // Disable CharacterController to avoid physics interference
                            var cc = playerObj.GetComponent<CharacterController>();
                            if (cc != null)
                            {
                                cc.enabled = false;
                            }
                        }
                    }

                    var obj = spawnedPlayers[id];
                    if (id != room.SessionId)
                    {
                        // Only update REMOTE players from network
                        obj.transform.position = new Vector3(
                            playerState.x,
                            playerState.y,
                            playerState.z
                        );

                        obj.transform.rotation = Quaternion.Euler(
                            0,
                            playerState.rotY,
                            0
                        );
                    }
                }

                // Removal logic (safe copy of keys)
                var existingIds = new List<string>(spawnedPlayers.Keys);

                foreach (var id in existingIds)
                {
                    if (!state.players.ContainsKey(id))
                    {
                        Destroy(spawnedPlayers[id]);
                        spawnedPlayers.Remove(id);
                        Debug.Log("Removed player: " + id);
                    }
                }
                Debug.Log("OnStateChange triggered. Player count: " + state.players.Count);
            };
        }

        void Update()
        {
            if (room == null) return;

            if (!spawnedPlayers.ContainsKey(room.SessionId)) return;

            GameObject localPlayer = spawnedPlayers[room.SessionId];

            // Send actual position of local player
            Vector3 pos = localPlayer.transform.position;
            float rotY = localPlayer.transform.eulerAngles.y;

            room.Send("move", new
            {
                x = pos.x,
                y = pos.y,
                z = pos.z,
                rotY = rotY
            });
        }

        private System.Collections.IEnumerator FixSpawnHeight(float x, float z)
        {
            // Wait a short moment for chunks to generate
            yield return new WaitForSeconds(0.2f);

            int surfaceY = GameService.Instance.WorldService
                .GetSurfaceHeight(new Vector3(x, 0, z));

            Debug.Log($"SurfaceY at {x},{z} = {surfaceY}");

            float correctedY = surfaceY + 4f;

            Debug.Log($"Sending corrected Y: {correctedY}");

            room.Send("move", new
            {
                x = x,
                y = correctedY,
                z = z
            });
        }

        private IEnumerator InitializePlayerAfterSpawn(GameObject playerObj)
        {
            // Wait until Y is corrected
            yield return new WaitForSeconds(0.3f);

            GameService.Instance.RegisterNetworkPlayer(playerObj.transform);
        }
    }
}