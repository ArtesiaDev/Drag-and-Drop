using _Project.Scripts.Core.Models;
using _Project.Scripts.View;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.Logic
{
    public class ShelfElementDragging : ITickable
    {
        private RackModel _rackModel;
        private ShelfElementPresenter _presenter;
        private Camera _mainCamera;
        private readonly Collider2D[] _overlapResults = new Collider2D[5];

        [Inject]
        private void Construct(RackModel rackModel, ShelfElementPresenter presenter)
        {
            _rackModel = rackModel;
            _presenter = presenter;
            _mainCamera = Camera.main;
        }

        public void Tick()
        {
            if (Input.touchSupported && Input.touchCount > 0 && _rackModel.ElementsIMoved)
                MobileDragging();
            else EditorDragging();
        }

        private void MobileDragging()
        {
            var touch = Input.GetTouch(0);
            var touchWorldPos = _mainCamera.ScreenToWorldPoint(touch.position);
            touchWorldPos.z = 0;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    TryStartDrag(touchWorldPos);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    _presenter.Dragging(touchWorldPos);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    _presenter.FinishDrag();
                    break;
            }
        }

        private void EditorDragging()
        {
            var mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            if (Input.GetMouseButtonDown(0))
                TryStartDrag(mouseWorldPos);

            else if (Input.GetMouseButton(0))
                _presenter.Dragging(mouseWorldPos);

            else if (Input.GetMouseButtonUp(0))
                _presenter.FinishDrag();
        }

        private void TryStartDrag(Vector3 position)
        {
            var hitCount = Physics2D.OverlapPointNonAlloc(position, _overlapResults);
            var topSortingOrder = int.MinValue;
            GameObject topMostElement = null;

            for (var i = 0; i < hitCount; i++)
            {
                var hit = _overlapResults[i];
                if (!hit.gameObject.TryGetComponent(out ShelfElementView view))
                    continue;
                
                var model = _rackModel.ElementModels[view.StartPosition.Row, view.StartPosition.Column];
                
                if (model.IsRightPosition)
                    continue;

                var renderer = hit.gameObject.GetComponent<Image>();
                var order = renderer != null ? renderer.layoutPriority : 0;
                    
                if (order <= topSortingOrder)
                    continue;
                
                topSortingOrder = order;
                topMostElement = hit.gameObject;
            }

            if (topMostElement == null)
                return;

            var offset = topMostElement.transform.position - position;
            _presenter.StartDragElement(offset, topMostElement);
        }
    }
}