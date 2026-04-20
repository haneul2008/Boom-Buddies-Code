using System.Collections.Generic;
using System.Linq;
using HNLib.Dependencies;
using UnityEngine;

namespace Code.Managers
{
    [DefaultExecutionOrder(-20)]
    public class CreateOnceManager : MonoBehaviour
    {
        public static CreateOnceManager Instance { get; private set; }
        private List<MonoBehaviour> _createOnceManagers = new List<MonoBehaviour>();

        private void Awake()
        {
            _createOnceManagers = GetComponentsInChildren<MonoBehaviour>().ToList();
            Injector injector = FindAnyObjectByType<Injector>();

            if (Instance != null && Instance != this)
            {
                foreach (MonoBehaviour manager in _createOnceManagers)
                    injector.AddExcludeProvider(manager);

                Destroy(gameObject);
                return;
            }

            Instance = this;

            injector.OnInjectEnd += HandleInjectEnd;
            
            DontDestroyOnLoad(gameObject);
        }

        private void HandleInjectEnd()
        {
            Injector injector = FindAnyObjectByType<Injector>();
            injector.OnInjectEnd -= HandleInjectEnd;
            
            _createOnceManagers.OfType<IOnceManager>().OrderByDescending(manager => manager.Priority)
                .ToList().ForEach(manager => manager.Initialize());
        }

        public T GetManager<T>() where T : MonoBehaviour
        {
            return _createOnceManagers.FirstOrDefault(manager => manager is T) as T;
        }
    }
}