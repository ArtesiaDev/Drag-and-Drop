using System;
using System.Linq;
using _Project.Scripts.Core.Models;
using _Project.Scripts.Services.AudioManagement;
using _Project.Scripts.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;
using AudioClip = _Project.Scripts.Services.AudioManagement.AudioClip;

namespace _Project.Scripts.Logic
{
    public class ShelfElementPresenter
    {
        private RackModel _rackModel;
        private AudioSystem _audioSystem;
        private IWinLogic _winLogic;

        private GameObject _draggedElement;
        private Vector3 _dragOffset;
        private Tween _dragScaleTween;
        private Tween _shakeTween;
        private readonly Collider2D[] _overlapResults = new Collider2D[5];

        [Inject]
        private void Construct(AudioSystem audioSystem, RackModel rackModel, IWinLogic winLogic)
        {
            _rackModel = rackModel;
            _audioSystem = audioSystem;
            _winLogic = winLogic;
        }

        public void StartDragElement(Vector3 offset, GameObject element)
        {
            _draggedElement = element;
            _dragOffset = offset;

            _audioSystem.PlayOneShotSound(AudioClip.Get);

            _dragScaleTween?.Kill();
            _dragScaleTween = _draggedElement.transform.DOScale(1.1f, 0.1f)
                .OnComplete(() => { _dragScaleTween = _draggedElement.transform.DOScale(1f, 0.1f); });
        }

        public void Dragging(Vector3 touchWorldPos)
        {
            if (_draggedElement != null)
                _draggedElement.transform.position = touchWorldPos + _dragOffset;
        }

        public async void FinishDrag()
        {
            if (_draggedElement == null)
                return;
            
            var collider = _draggedElement.GetComponent<Collider2D>();
            collider.enabled = false;
            var hitCount = Physics2D.OverlapPointNonAlloc(_draggedElement.transform.position, _overlapResults);
            collider.enabled = true;

            switch (hitCount)
            {
                case 0:
                case 1: MoveDraggableBack(); break;

                case 2: await BusyZoneLogic(); break;
                default: throw new ArgumentOutOfRangeException();
            }

            _shakeTween?.Kill();
            _dragScaleTween?.Kill();
            _draggedElement.transform.localScale = Vector3.one;
            _draggedElement = null;
            _dragOffset = default;
        }

        private void MoveDraggableBack()
        {
            var view = _draggedElement.GetComponent<ShelfElementView>();
            var model = _rackModel.ElementModels[view.StartPosition.Row, view.StartPosition.Column];
            var moveArea = _rackModel.ElementAreas[model.CurrentPosition.Row, model.CurrentPosition.Column];
            _draggedElement.transform.DOMove(moveArea.transform.position, _rackModel.MoveElementDuration);
            model.SetCurrentPosition(moveArea.Position);
        }

        private async UniTask BusyZoneLogic()
        {
            ShelfElementView otherView = default;
            ElementArea moveArea = default;

            foreach (var collider in _overlapResults.Where(collider => collider != null))
            {
                if (collider.gameObject.TryGetComponent<ShelfElementView>(out var view))
                    otherView = view;
                if (collider.gameObject.TryGetComponent<ElementArea>(out var area))
                    moveArea = area;
            }

            if (otherView == null || moveArea == null)
                throw new NullReferenceException();

            var otherModel = _rackModel.ElementModels[otherView.StartPosition.Row, otherView.StartPosition.Column];
            if (otherModel.IsRightPosition)
            {
                _audioSystem.PlayOneShotSound(AudioClip.Wrong);
                await PlayErrorShake(_draggedElement.transform);
                MoveDraggableBack();
            }
            else SwapElements(moveArea, otherView, otherModel);
        }

        private async UniTask PlayErrorShake(Transform target)
        {
            _shakeTween?.Kill();
            var sequence = DOTween.Sequence();
            sequence.Append(target.DOLocalRotate(new Vector3(0, 0, -15), 0.05f).SetEase(Ease.OutQuad));
            sequence.Append(target.DOLocalRotate(new Vector3(0, 0, 15), 0.1f).SetEase(Ease.OutQuad));
            sequence.Append(target.DOLocalRotate(Vector3.zero, 0.05f).SetEase(Ease.OutQuad));

            _shakeTween = sequence;
            await sequence.AsyncWaitForCompletion();
        }

        private void SwapElements(ElementArea thisArea, ShelfElementView otherView, ShelfElementModel otherModel)
        {
            var view = _draggedElement.GetComponent<ShelfElementView>();
            var model = _rackModel.ElementModels[view.StartPosition.Row, view.StartPosition.Column];
            _draggedElement.transform.DOMove(thisArea.transform.position, _rackModel.PutElementDuration);

            var otherArea = _rackModel.ElementAreas[model.CurrentPosition.Row, model.CurrentPosition.Column];
            otherView.gameObject.transform.DOMove(otherArea.transform.position, _rackModel.MoveElementDuration);

            model.SetCurrentPosition(thisArea.Position);
            otherModel.SetCurrentPosition(otherArea.Position);

            if (model.IsRightPosition || otherModel.IsRightPosition)
            {
                _audioSystem.PlayOneShotSound(AudioClip.Right);
                _winLogic.GameWin();
            }
        }
    }
}