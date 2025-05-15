using _Project.Scripts.GameEntity;

namespace _Project.Scripts.Core.Models
{
    public class ShelfElementModel
    {
        public ShelfElementModel(ShelfElement config)
        {
            StartPosition = config.StartPosition;
            MovePosition = config.MovePosition;
            SetCurrentPosition(StartPosition);
        }

        public Coordinates StartPosition { get; }
        public Coordinates MovePosition { get; }
        public Coordinates CurrentPosition { get; private set; }

        public bool IsRightPosition => CurrentPosition.Row == StartPosition.Row;

        public void SetCurrentPosition(Coordinates newPosition) =>
            CurrentPosition = newPosition;
    }
}