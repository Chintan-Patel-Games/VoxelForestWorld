using UnityEngine;
using VoxelWorld.Core.Utilities;

namespace VoxelWorld.Sound
{
    public class SceneAudioInstaller : GenericMonoSingleton<SceneAudioInstaller>
    {
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource bgmSource;

        private void Start()
        {
            GlobalSoundService.Instance.RegisterAudioSources(sfxSource, bgmSource);
        }
    }
}