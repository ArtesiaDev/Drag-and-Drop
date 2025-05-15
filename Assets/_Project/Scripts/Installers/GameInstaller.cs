using _Project.Scripts.Configs;
using _Project.Scripts.Core.Models;
using _Project.Scripts.Logic;
using _Project.Scripts.View;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Installers
{
    public class GameInstaller: MonoInstaller
    {
        [SerializeField] private RackView _rackView;
        [SerializeField] private RackConfig _rackConfig;

        public override void InstallBindings()
        {
            Container.Bind<RackView>().FromInstance(_rackView).AsSingle();
            Container.BindInterfacesAndSelfTo<RackPresenter>().AsSingle();
            Container.Bind<RackModel>().AsSingle().WithArguments(_rackConfig);
            Container.BindInterfacesTo<ShelfElementDragging>().AsSingle();
            Container.Bind<ShelfElementPresenter>().AsSingle();
        }
    }
}