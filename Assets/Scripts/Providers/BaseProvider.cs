using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Providers
{
    public abstract class MonoProvider<TMono> : MonoBehaviour, IProvider<TMono> where TMono : MonoBehaviour
    {
        [SerializeField] private RectTransform _holder;
        [SerializeField] private TMono _prefab;

        private List<TMono> _objects = new List<TMono>();
        public RectTransform Holder => _holder;
        
        public TMono CreateNew()
        {
            var obj = Instantiate(_prefab, _holder);
            _objects.Add(obj);
            
            OnCreated(obj);
            return obj;
        }
        
        //Optional
        protected virtual void OnCreated(TMono obj)
        { }

        public IEnumerable<TMono> GetCreatedObjects()
        {
            return _objects.AsEnumerable();
        }

        public void Remove(TMono obj)
        {
            _objects.Remove(obj);
            Destroy(obj);
        }

        public void Clear()
        {
            for (int i = _objects.Count - 1; i >= 0; i--)
            {
                var obj = _objects[i];
                _objects.RemoveAt(i);
                Destroy(obj);
            }
        }
    }

    public interface IProvider<TObject>
    {
        public TObject CreateNew();
        public IEnumerable<TObject> GetCreatedObjects();
        public void Remove(TObject obj);
        public void Clear();
    }
}
