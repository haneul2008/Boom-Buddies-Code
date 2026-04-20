using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        public UnityEvent<Entity> OnPlaceEvent;
        
        public bool IsDead { get; set; }
        protected Dictionary<Type, IEntityComponent> _components;

        protected virtual void Awake()
        {
            _components = new Dictionary<Type, IEntityComponent>();
            AddComponents();
            InitializeComponents();
            AfterInit();
        }

        protected virtual void AfterInit()
        {
            _components.Values.OfType<IAfterInit>().ToList().ForEach(item => item.AfterInit());
        }

        protected virtual void AddComponents()
        {
            GetComponentsInChildren<IEntityComponent>(true).ToList()
                .ForEach(component => _components.Add(component.GetType(), component));
        }
        
        protected virtual void InitializeComponents()
        {
            _components.Values.ToList().ForEach(component => component.Initialize(this));
        }

        public T GetCompo<T>(bool isDerived = false) where T : IEntityComponent
        {
            if (_components.TryGetValue(typeof(T), out IEntityComponent component))
                return (T)component;
            
            if(isDerived == false) return default(T);
            
            Type findType = _components.Keys.FirstOrDefault(type => type.IsSubclassOf(typeof(T)));
            if(findType != null) 
                return (T)_components[findType];
            
            return default(T);
        }

        public IEntityComponent GetCompo(Type type)
            => _components.GetValueOrDefault(type);
    }
}