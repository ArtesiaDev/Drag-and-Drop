using _Project.Scripts.GameEntity;
using UnityEngine;

namespace _Project.Scripts.View
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ElementArea : MonoBehaviour
    {
        [field: SerializeField] public Coordinates Position { get; private set; }
    }
}