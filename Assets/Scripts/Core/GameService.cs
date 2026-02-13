using UnityEngine;
using UnityEngine.SceneManagement;
using VoxelWorld.Core.Events;
using VoxelWorld.Core.PlayerSystem;
using VoxelWorld.Core.Utilities;
using VoxelWorld.Sound;
using VoxelWorld.UI;
using VoxelWorld.WorldGeneration.Chunks;
using VoxelWorld.WorldGeneration.World;

namespace VoxelWorld.Core
{
    public class GameService : GenericMonoSingleton<GameService>
    {
        [Header("References")]
        [SerializeField] private GameObject chunkPrefab;
        [SerializeField] private WorldController worldController;

        [Header("World Settings")]
        [SerializeField] private float loadDelay = 0.02f;
        [SerializeField] private int worldSeed = 12345;

        private Transform player;
        private Vector3 spawnPos;

        // Services
        public WorldService WorldService { get; private set; }
        public TreeService TreeService { get; private set; }

        private bool isGameScene;

        protected override void Awake()
        {
            base.Awake();
            Time.timeScale = 1f;  // safety reset every time gameplay scene loads
            Application.targetFrameRate = 60;

            // Determine scene type
            isGameScene = SceneManager.GetActiveScene().name == "VoxelCraft";

            if (isGameScene)
            {
                InitializeServices();
                worldController.Init(WorldService);
                worldController.SetupFog();
            }
        }

        private void InitializeServices()
        {
            WorldService = new WorldService(chunkPrefab, worldSeed, loadDelay);
            TreeService = new TreeService(worldSeed);
        }

        private void Start()
        {
            if (!isGameScene) return; // MainMenu scene -> skip world generation

            // Force ChunkRunner to initialize so its Update() will run
            _ = ChunkRunner.Instance;

            if (chunkPrefab == null || worldController == null)
            {
                Debug.LogError("GameService: Missing reference!");
                enabled = false;
                return;
            }
        }

        public void RegisterNetworkPlayer(Transform networkPlayer)
        {
            Debug.Log("GameService registering network player.");

            player = networkPlayer;
            //PlayerService.Instance.InitializePlayer(networkPlayer.gameObject);
            PlayerService.Instance.RegisterLocalNetworkPlayer(networkPlayer);
            worldController.player = player;

            EventService.Instance.OnGameInitialized.InvokeEvent(true);
            UIService.Instance.HideLoadingUI();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            GlobalSoundService.Instance.SoundService?.UpdateFootsteps(Time.deltaTime);
            PlayerService.Instance.Render();
        }

        public static ChunkService ChunkService => Instance.WorldService.GetChunkService();
    }
}