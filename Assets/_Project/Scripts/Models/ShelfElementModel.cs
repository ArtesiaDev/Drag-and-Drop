using _Project.Scripts.GameEntity;

namespace _Project.Scripts.Core.Models
{
    public class ShelfElementModel
    {
        public ShelfElementModel(ShelfElement config)
        {
            StartPosition = config.StartPosition;
            MovePosition = config.MovePosition;
        }

        public Coordinates StartPosition { get; private set; }
        public Coordinates MovePosition { get; private set; }
        
        public bool IsMoved => StartPosition != MovePosition;
    }
}