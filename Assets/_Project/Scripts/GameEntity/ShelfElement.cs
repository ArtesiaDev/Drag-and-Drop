using System;
using UnityEngine;

namespace _Project.Scripts.GameEntity
{
    [Serializable]
    public struct ShelfElement
    {
        [field: SerializeField] public Coordinates StartPosition { get; private set; }
        [field: SerializeField] public Coordinates MovePosition { get; private set; }
    }
}