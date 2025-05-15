using _Project.Scripts.Configs;
using _Project.Scripts.Core.Models;
using _Project.Scripts.Services.AudioManagement;
using Scripts.Services.AssetManagement;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Installers
{
    public class ProjectInstaller: MonoInstaller
    {
        [SerializeField] private GameObject _audioSystem;
        [SerializeField] private AudioConfig _audioConfig;

        public override void InstallBindings()
        {
           // Container.BindInterfacesTo<AssetProvider>().AsSingle();
            Container.Bind<AudioModel>().AsSingle().WithArguments(_audioConfig);
            Container.Bind<AudioSystem>()
                .FromComponentInNewPrefab(_audioSystem)
                .AsSingle()
                .NonLazy();
        }
    }
}