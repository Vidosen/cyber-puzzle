using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Utils.Inventory
{
    public abstract class BaseInventory<TModel, TIndex> : IEnumerable<TModel>
        where TModel : IModel<TIndex>
        where TIndex : IComparable<TIndex>
    {
        protected ReactiveCollection<TModel> _models = new ReactiveCollection<TModel>();
        
        public IObservable<CollectionAddEvent<TModel>> ModelAdded => _models.ObserveAdd();
        public IObservable<CollectionRemoveEvent<TModel>> ModelRemoved => _models.ObserveRemove();
        public IObservable<int> CountChanged => _models.ObserveCountChanged();
        public int Count => _models.Count;
        
        public abstract TModel CreateModelById(TIndex modelId);
        
        public bool Contains(TModel model)
        {
            return _models.Contains(model);
        }

        public virtual bool Contains(TIndex modelId)
        {
            return _models.Any(model => model.Id.Equals(modelId));
        }

        public virtual TModel GetModelById(TIndex modelId)
        {
            return _models.FirstOrDefault(model => model.Id.Equals(modelId));
        }

        public virtual void Remove(TModel model)
        {
            _models.Remove(model);
        }
        
        public virtual void Remove(TIndex modelId)
        {
            var model = GetModelById(modelId);
            _models.Remove(model);
        }

        public IEnumerator<TModel> GetEnumerator()
        {
            return _models.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}