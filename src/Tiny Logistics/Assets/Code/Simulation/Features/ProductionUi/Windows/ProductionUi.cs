using System.Globalization;
using System.Text;
using Code.Simulation.Features.ProductionUi.Model;
using Code.Simulation.Features.ProductionUi.Services;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Code.Simulation.Features.ProductionUi.Windows {
    public sealed class ProductionUi : MonoBehaviour {
        public Text ResourcesLabel;
        public Text ProductionLabel;
        public Text UpgradeButtonLabel;
        public Button UpgradeButton;

        private IProductionUiService _service;
        private bool _subscribed;
        private readonly StringBuilder _resourceText = new();

        [Inject]
        public void Construct(IProductionUiService service) {
            Unsubscribe();
            _service = service;

            if (isActiveAndEnabled) {
                Subscribe();
            }
        }

        private void OnEnable() => Subscribe();

        private void OnDisable() => Unsubscribe();

        private void Subscribe() {
            if (_service == null || _subscribed) {
                return;
            }

            _service.onDataChanged += Refresh;
            UpgradeButton.onClick.AddListener(RequestUpgrade);
            _subscribed = true;
            Refresh();
        }

        private void Unsubscribe() {
            if (!_subscribed) {
                return;
            }

            _service.onDataChanged -= Refresh;
            UpgradeButton.onClick.RemoveListener(RequestUpgrade);
            _subscribed = false;
        }

        private void RequestUpgrade() {
            _service.RequestUpgrade();
        }

        private void Refresh() {
            ProductionUiInfo info = _service.GetUiInfo();

            _resourceText.Clear();

            foreach (ResourceUiInfo resource in info.Resources) {
                if (_resourceText.Length > 0) {
                    _resourceText.AppendLine();
                }

                _resourceText.Append(resource.Name).Append(": ").Append(Format(resource.Amount));
            }

            ResourcesLabel.text = _resourceText.ToString();
            
            ProductionLabel.text = info.IsFactoryAlive
                ? $"Truck every {FormatRate(info.CraftDuration)} s\nProduction: {FormatRate(info.TrucksPerSecond)} trucks/s"
                : "Factory destroyed";
            
            UpgradeButtonLabel.text = $"Upgrade factory (×{FormatRate(info.UpgradeSpeedMultiplier)})\n{Format(info.UpgradeCost)} Gold";
            
            UpgradeButton.interactable = info.CanUpgrade;
        }

        private static string Format(long value) => value.ToString("N0", CultureInfo.InvariantCulture);

        private static string FormatRate(float value) => value.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
