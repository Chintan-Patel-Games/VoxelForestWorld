using Colyseus;
using UnityEngine;
using UnityEngine.SceneManagement;
using VoxelWorld.Core;
using VoxelWorld.Core.Events;
using VoxelWorld.Networking;
using VoxelWorld.UI.Interface;

namespace VoxelWorld.UI.PauseUI
{
    public class PauseUIController : IUIController
    {
        private PauseUIView view;

        public PauseUIController(PauseUIView view)
        {
            this.view = view;
            this.view.SetController(this);
            Hide();
        }

        public void ResumeGame()
        {
            GlobalSoundService.Instance.SoundService.PlaySFX(Sound.SoundType.UI_BUTTON_CLICK);
            EventService.Instance.OnGamePause.InvokeEvent(false);
        }

        public void ShowOptionsUI()
        {
            GlobalSoundService.Instance.SoundService.PlaySFX(Sound.SoundType.UI_BUTTON_CLICK);
            UIService.Instance.ShowOptionsUI();
        }

        public async void ShowMainMenuUI()
        {
            Hide();
            Time.timeScale = 1f;
            GlobalSoundService.Instance.SoundService.PlaySFX(Sound.SoundType.UI_BUTTON_CLICK);
            if (ColyseusManager.Instance.GetRoom() != null)
                await ColyseusManager.Instance.GetRoom().Leave();
            UIService.Instance.ShowMainMenuUI();
            SceneManager.LoadScene("Main Menu");
        }

        public void Show() => view.EnableView();
        public void Hide() => view.DisableView();
    }
}