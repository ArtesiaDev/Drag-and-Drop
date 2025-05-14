using System;
using UnityEngine;

namespace _Project.Scripts.GameEntity
{
    [Serializable]
    public struct Coordinates : IEquatable<Coordinates>
    {
        public Coordinates(int row, int column)
        {
            Row = row;
            Column = column;
        }

        [field: Tooltip("Index of the row")]
        [field: SerializeField]
        public int Row { get; private set; }

        [field: Tooltip("MIndex of the column")]
        [field: SerializeField]
        public int Column { get; private set; }

        public static bool operator ==(Coordinates a, Coordinates b) =>
            a.Row == b.Row && a.Column == b.Column;

        public static bool operator !=(Coordinates a, Coordinates b) =>
            !(a == b);

        public bool Equals(Coordinates other) =>
            Row == other.Row && Column == other.Column;

        public override bool Equals(object obj) =>
            obj is Coordinates other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(Row, Column);

        public override string ToString() =>
            $"({Row}, {Column})";
    }
}