using System;
using System.Collections.Generic;
using Minigames.MatrixBreaching.Config;
using Minigames.MatrixBreaching.Vulnerabilities.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace Minigames.MatrixBreaching.Vulnerabilities.Views
{
    public class VulnerabilitiesPresenter : MonoBehaviour
    {
        [SerializeField] private Transform _holder;
        private VulnerabiltyInventory _vulnerabiltyInventory;

        private Dictionary<VulnerabilityModel, VulnerabilityView> _vulnerabilityViews =
            new Dictionary<VulnerabilityModel, VulnerabilityView>();

        private DiContainer _container;
        private MatrixBreachingViewConfig _viewConfig;

        [Inject]
        private void Construct(VulnerabiltyInventory vulnerabiltyInventory, DiContainer container, MatrixBreachingViewConfig viewConfig)
        {
            _viewConfig = viewConfig;
            _container = container;
            _vulnerabiltyInventory = vulnerabiltyInventory;
        }

        private void Start()
        {
            foreach (var vulnerabilityModel in _vulnerabiltyInventory)
            {
                InitVulnerabilityView(vulnerabilityModel);
            }

            _vulnerabiltyInventory.ModelAdded
                .Subscribe(addEvent => InitVulnerabilityView(addEvent.Value))
                .AddTo(this);
            _vulnerabiltyInventory.ModelRemoved
                .Subscribe(removeEvent => DisposeVulnerabilityView(removeEvent.Value))
                .AddTo(this);
        }

        private async void DisposeVulnerabilityView(VulnerabilityModel vulnerabilityModel)
        {
            if (_vulnerabilityViews.TryGetValue(vulnerabilityModel, out var view))
            {
                await view.HideAnimation();
                Destroy(view.gameObject);
                _vulnerabilityViews.Remove(vulnerabilityModel);
            }
        }

        private void InitVulnerabilityView(VulnerabilityModel vulnerabilityModel)
        {
            _vulnerabilityViews[vulnerabilityModel] =
                _container.InstantiatePrefabForComponent<VulnerabilityView>(_viewConfig.VulnerabilityViewTemplate, _holder);
            _vulnerabilityViews[vulnerabilityModel].Initialize(vulnerabilityModel);
        }
    }
}