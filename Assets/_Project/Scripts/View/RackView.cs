using System.Collections.Generic;
using _Project.Scripts.Logic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.View
{
    public class RackView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _time;
        [SerializeField] private Button _retryButton;
        [SerializeField] private GameObject _endGamePanel;
        [SerializeField] private Image _endGameLayout;
        [SerializeField] private List<ShelfElementView> _elements;
        [SerializeField] private List<ElementArea> _elementAreas;

        private RackPresenter _presenter;

        [Inject]
        private void ConstructPresenter(RackPresenter presenter) =>
            _presenter = presenter;

        private void Awake()
        {
            _retryButton.onClick.AddListener(_presenter.RetryGame);
        }

        private void OnDisable()
        {
            _retryButton.onClick.RemoveAllListeners();
        }
        
        public List<ShelfElementView> GetElements() =>
            _elements;
        public List<ElementArea> GetElementAreas() =>
            _elementAreas;

        public void DrawTimer(int time) =>
            _time.text = $"{time} s";

        public void SwitchGameOverScreen(bool predicate, float alpha)
        {
            _endGamePanel.SetActive(predicate);
            if (predicate)
                _endGameLayout.DOFade(alpha, 2f);
            else
            {
                var color = _endGameLayout.color;
                color.a = alpha;
                _endGameLayout.color = color;
            }
        }
    }
}