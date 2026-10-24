using System;
using Code.Simulation.Features.ProductionUi.Model;

namespace Code.Simulation.Features.ProductionUi.Services {
    public interface IProductionUiService {
        event Action onDataChanged;

        ProductionUiInfo GetUiInfo();

        void RequestUpgrade();
    }
}
