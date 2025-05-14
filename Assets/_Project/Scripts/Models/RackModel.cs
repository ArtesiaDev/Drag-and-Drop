using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Configs;
using _Project.Scripts.GameEntity;
using _Project.Scripts.View;

namespace _Project.Scripts.Core.Models
{
    public class RackModel
    {
        public RackModel(RackConfig config)
        {
            SessionTime = config.SessionTime;
            StartMoveDuration = config.StartMoveDuration;
            StartEndGameLayoutAlfa = config.StartEndGameLayoutAlfa;
            FinishEndGameLayoutAlfa = config.FinishEndGameLayoutAlfa;
            CancellationToken = new CancellationTokenSource();
            InitializeElementModels(config);
        }

        public int SessionTime { get; private set; }
        public int StartMoveDuration { get; private set; }
        public float StartEndGameLayoutAlfa { get; private set; }
        public float FinishEndGameLayoutAlfa { get; private set; }
        public ShelfElementModel[,] ElementModels { get; private set; }
        public ElementArea[,] ElementAreas { get; private set; }
        public List<ShelfElementView> ElementViews { get; private set; }
        
        public CancellationTokenSource CancellationToken { get; private set; }

        public void InitializeElementAreas(List<ElementArea> elementAreas)
        {
            ElementAreas = new ElementArea[3, 4];
            foreach (var area in elementAreas)
                ElementAreas[area.StartPosition.Row, area.StartPosition.Column] = area;
        }
        
        public void InitializeElementViews(List<ShelfElementView> elementViews) =>
            ElementViews = elementViews;
        
        private void InitializeElementModels(RackConfig config)
        {
            ElementModels = new ShelfElementModel[3, 4];
            var rows = new List<List<ShelfElement>> { config.Raw1, config.Raw2, config.Raw3 };
            for (var row = 0; row < 3; row++)
                for (var col = 0; col < 4; col++)
                    ElementModels[row, col] = new ShelfElementModel(rows[row][col]);
        }
        
    }
}