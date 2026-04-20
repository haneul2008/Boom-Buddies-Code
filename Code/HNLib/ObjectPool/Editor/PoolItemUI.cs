using System;
using HNLib.ObjectPool.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace HNLib.ObjectPool.Editor
{
    public class PoolItemUI
    {
        private Label _nameLabel;
        private Button _deleteBtn;
        private VisualElement _rootElement;

        public event Action<PoolItemUI> OnDeleteEvent;
        public event Action<PoolItemUI> OnSelectEvent;

        public string Name
        {
            get => _nameLabel.text;
            set => _nameLabel.text = value;
        }

        public PoolItemSO poolItem;

        public bool IsActive
        {
            get => _rootElement.ClassListContains("active");
            set => _rootElement.EnableInClassList("active", value);
        }

        public PoolItemUI(VisualElement root, PoolItemSO item)
        {
            poolItem = item;
            _rootElement = root.Q<VisualElement>("PoolItem");
            Debug.Assert(_rootElement != null, "Root element is null");
            _nameLabel = _rootElement.Q<Label>("ItemName");
            _deleteBtn = _rootElement.Q<Button>("DeleteBtn");
            _deleteBtn.RegisterCallback<ClickEvent>(evt =>
            {
                OnDeleteEvent?.Invoke(this);
                evt.StopPropagation();
            });
            
            _rootElement.RegisterCallback<ClickEvent>(evt =>
            {
                OnSelectEvent?.Invoke(this);
                evt.StopPropagation();
            });
        }
    }
}