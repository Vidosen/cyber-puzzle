using System.Collections.Generic;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Vulnerabilities.Models;
using Utils.Inventory;

namespace Minigames.MatrixBreaching.Vulnerabilities
{
    public class VulnerabiltyInventory : BaseInventory<VulnerabilityModel, string>
    {
        private static int _index = 0;
        
        private readonly VulnerabiltyFactory _factory;
        
        public VulnerabiltyInventory(VulnerabiltyFactory factory)
        {
            _factory = factory;
        }

        public VulnerabilityModel CreateNewModel(List<CellValueType> vulnerabilitySequence, out string modelId)
        {
            modelId = $"{nameof(VulnerabilityModel)}_{_index++}";
            var model = CreateModelById(modelId);
            model.Initialize(vulnerabilitySequence);
            return model;
        }
        public override VulnerabilityModel CreateModelById(string modelId)
        {
            var newModel = _factory.Create(modelId);
            _models.Add(newModel);
            return newModel;
        }
    }
}