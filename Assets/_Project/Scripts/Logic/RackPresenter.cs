using System;
using _Project.Scripts.Core.Models;
using _Project.Scripts.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Zenject;

namespace _Project.Scripts.Logic
{
    public class RackPresenter : IInitializable, IDisposable
    {
        private RackView _view;
        private RackModel _rackModel;

        [Inject]
        private void Construct(RackView rackView, RackModel rackModel)
        {
            _view = rackView;
            _rackModel = rackModel;
        }

        public void RetryGame()
        {
            _view.SwitchGameOverScreen(false, _rackModel.StartEndGameLayoutAlfa);
            RunTimer();
            MoveElements();
        }

        public void Initialize()
        {
            _rackModel.InitializeElementViews(_view.GetElements());
            _rackModel.InitializeElementAreas(_view.GetElementAreas());
            RunTimer();
            MoveElements();
        }

        public void Dispose() =>
            _rackModel.CancellationToken?.Cancel();

        private async void RunTimer()
        {
            for (var i = 0; i < _rackModel.SessionTime; i++)
            {
                _view.DrawTimer(i);
                try
                {
                    await UniTask.WaitForSeconds(1, cancellationToken: _rackModel.CancellationToken.Token);
                }
                catch
                {
                    return;
                }
            }

            GameOver();
        }

        private void MoveElements()
        {
            foreach (var element in _rackModel.ElementViews)
            {
                var model = _rackModel.ElementModels[element.StartPosition.Row, element.StartPosition.Column];
                var moveArea = _rackModel.ElementAreas[model.MovePosition.Row, model.MovePosition.Column];
                element.transform.DOMove(moveArea.transform.position, _rackModel.StartMoveDuration);
            }
        }

        private void GameOver()
        {
            _view.SwitchGameOverScreen(true, _rackModel.FinishEndGameLayoutAlfa);
        }
    }
}