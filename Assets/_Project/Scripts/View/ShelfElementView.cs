using _Project.Scripts.GameEntity;
using UnityEngine;

namespace _Project.Scripts.View
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ShelfElementView: MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _collider;
        [field: SerializeField] public Coordinates StartPosition { get; private set; }
    }
}