using System.Collections.Generic;
using _Project.Scripts.GameEntity;
using UnityEngine;

namespace _Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "RackConfig", menuName = "Configs/RackConfig", order = 2)]
    public class RackConfig : ScriptableObject
    {
        [field: SerializeField] public int SessionTime { get; private set; }
        [field: SerializeField] public int StartMoveDuration { get; private set; }
        [field: SerializeField, Range(0,1)] public float StartEndGameLayoutAlfa { get; private set; }
        [field: SerializeField, Range(0,1)] public float FinishEndGameLayoutAlfa { get; private set; }
        [field: SerializeField] public List<ShelfElement> Raw1 { get; private set; }
        [field: SerializeField] public List<ShelfElement> Raw2 { get; private set; }
        [field: SerializeField] public List<ShelfElement> Raw3 { get; private set; }
    }
}