using Unity.Cinemachine;
using UnityEngine;
using VoxelWorld.Core.Utilities;
using VoxelWorld.Player;

namespace VoxelWorld.Core.PlayerSystem
{
    public class PlayerService : GenericMonoSingleton<PlayerService>
    {
        [SerializeField] private CinemachineCamera virtualCamera;

        private PlayerView localPlayerView;

        public void RegisterLocalNetworkPlayer(Transform player)
        {
            var view = player.GetComponent<PlayerView>();

            if (view == null)
            {
                Debug.LogError("PlayerView missing on Local Player!");
                return;
            }

            localPlayerView = view;

            AttachCamera(view);
        }

        private void AttachCamera(PlayerView view)
        {
            Transform followTarget = view.CameraTarget;

            if (followTarget != null)
            {
                virtualCamera.Follow = followTarget;
                virtualCamera.LookAt = followTarget;
            }
            else
            {
                Debug.LogWarning("CameraTarget missing!");
            }
        }

        public void Render()
        {
            if (localPlayerView != null)
                localPlayerView.TickRender();
        }
    }
}