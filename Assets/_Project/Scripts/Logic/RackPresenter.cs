using System;
using _Project.Scripts.Core.Models;
using _Project.Scripts.Services.AudioManagement;
using _Project.Scripts.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Zenject;

namespace _Project.Scripts.Logic
{
    public class RackPresenter : IInitializable, IDisposable, IWinLogic
    {
        private RackView _view;
        private RackModel _rackModel;
        private AudioSystem _audioSystem;

        [Inject]
        private void Construct(RackView rackView, RackModel rackModel, AudioSystem audioSystem)
        {
            _view = rackView;
            _rackModel = rackModel;
            _audioSystem = audioSystem;
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

        public void GameWin()
        {
            if(IsGameWin())
                _audioSystem.PlayOneShotSound(AudioClip.Final);
        }

        private async void RunTimer()
        {
            for (var i = 0; i < _rackModel.SessionTime; i++)
            {
                _view.DrawTimer(i);
                if (!await WaitForSeconds(1))
                    return;
            }
            GameOver();
        }

        private async void MoveElements()
        {
            if (!await WaitForSeconds(0.5f))
                return;
            
            foreach (var element in _rackModel.ElementViews)
            {
                var model = _rackModel.ElementModels[element.StartPosition.Row, element.StartPosition.Column];
                var moveArea = _rackModel.ElementAreas[model.MovePosition.Row, model.MovePosition.Column];
                element.transform.DOMove(moveArea.transform.position, _rackModel.StartMoveDuration);
                model.SetCurrentPosition(moveArea.Position);
            }
            _rackModel.ElementsIMoved = true;
        }

        private void GameOver()
        {
            _view.SwitchGameOverScreen(true, _rackModel.FinishEndGameLayoutAlfa);
            foreach (var element in _rackModel.ElementViews)
            {
                var model = _rackModel.ElementModels[element.StartPosition.Row, element.StartPosition.Column];
                var moveArea = _rackModel.ElementAreas[model.StartPosition.Row, model.StartPosition.Column];
                element.transform.DOMove(moveArea.transform.position, _rackModel.StartMoveDuration);
                model.SetCurrentPosition(moveArea.Position);
            }
        }

        private async UniTask<bool> WaitForSeconds(float delay)
        {
            try
            {
                await UniTask.WaitForSeconds(delay, cancellationToken: _rackModel.CancellationToken.Token);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool IsGameWin()
        {
            for (var y = 0; y < _rackModel.ElementModels.GetLength(0); y++)
            for (var x = 0; x < _rackModel.ElementModels.GetLength(1); x++)
                if (!_rackModel.ElementModels[y, x].IsRightPosition)
                    return false;
            return true;
        }
    }
}