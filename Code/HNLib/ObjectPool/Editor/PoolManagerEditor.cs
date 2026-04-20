using System;
using System.Collections.Generic;
using System.IO;
using HNLib.ObjectPool.Editor;
using HNLib.ObjectPool.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.HNLib.ObjectPool.Editor
{
    public class PoolManagerEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset = default;
        [SerializeField] private PoolManagerSO poolManager = default;
        [SerializeField] private VisualTreeAsset itemAsset = default;

        private string _rootFolder;
        private Button _createBtn;
        private ScrollView _itemView;

        private List<PoolItemUI> _itemList;
        private PoolItemUI _selectedItem;

        private UnityEditor.Editor _cacheEditor;
        private VisualElement _inspectorView;

        [MenuItem("Tools/PoolManagerEditor")]
        public static void ShowWindow()
        {
            PoolManagerEditor wnd = GetWindow<PoolManagerEditor>();
            wnd.titleContent = new GUIContent("PoolManagerEditor");
            wnd.minSize = new Vector2(600, 400);
        }

        public void CreateGUI()
        {
            InitializeRootFolder();
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            visualTreeAsset.CloneTree(root);

            GetElement(root);
            GeneratePoolingItems();
        }

        private void GeneratePoolingItems()
        {
            _itemList.Clear();
            _itemView.Clear();

            foreach (PoolItemSO item in poolManager.itemList)
            {
                TemplateContainer itemUI = itemAsset.Instantiate();
                PoolItemUI poolItemUI = new PoolItemUI(itemUI, item);
                _itemView.Add(itemUI);
                _itemList.Add(poolItemUI);

                poolItemUI.Name = item.poolingName;

                if(_selectedItem != null && _selectedItem.poolItem == item)
                    HandleSelectEvent(poolItemUI);
            
                poolItemUI.OnSelectEvent += HandleSelectEvent;
                poolItemUI.OnDeleteEvent += HandleDeleteEvent;
            }
        }

        private void HandleDeleteEvent(PoolItemUI item)
        {
            poolManager.itemList.Remove(item.poolItem);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.poolItem));
            EditorUtility.SetDirty(poolManager);
            AssetDatabase.SaveAssets();

            if (item == _selectedItem)
            {
                _selectedItem = null;
            }
        
            GeneratePoolingItems();
        }

        private void HandleSelectEvent(PoolItemUI item)
        {
            if (_selectedItem != null)
                _selectedItem.IsActive = false;
            _selectedItem = item;
            _selectedItem.IsActive = true;
        
            _inspectorView.Clear();
            UnityEditor.Editor.CreateCachedEditor(_selectedItem.poolItem, null, ref _cacheEditor);
            VisualElement inspector = _cacheEditor.CreateInspectorGUI();

            SerializedObject serializedObject = new SerializedObject(_selectedItem.poolItem);
            inspector.Bind(serializedObject);
            inspector.TrackSerializedObjectValue(serializedObject, so =>
            {
                _selectedItem.Name = so.FindProperty("poolingName").stringValue;
            });
            _inspectorView.Add(inspector);
        }

        private void GetElement(VisualElement root)
        {
            _createBtn = root.Q<Button>("CreateBtn");
            _createBtn.clicked += HandleCreateItem;
            _itemView = root.Q<ScrollView>("ItemView");
            _inspectorView = root.Q("InspectorView");
        
            _itemView.Clear();
            _itemList = new List<PoolItemUI>();
        }

        private void HandleCreateItem()
        {
            string itemName = Guid.NewGuid().ToString();
            PoolItemSO newitem = ScriptableObject.CreateInstance<PoolItemSO>();
            newitem.poolingName = itemName;

            if (Directory.Exists($"{_rootFolder}/Items") == false)
            {
                Directory.CreateDirectory($"{_rootFolder}/Items");
            }
        
            AssetDatabase.CreateAsset(newitem, $"{_rootFolder}/Items/{itemName}.asset");
            poolManager.itemList.Add(newitem);
            EditorUtility.SetDirty(poolManager);
            AssetDatabase.SaveAssets();
        
            GeneratePoolingItems();
        }

        private void InitializeRootFolder()
        {
            MonoScript monoScript = MonoScript.FromScriptableObject(this);
            string scriptPath = AssetDatabase.GetAssetPath(monoScript);
            _rootFolder = Path.GetDirectoryName(Path.GetDirectoryName(scriptPath).Replace("\\", "/"));

            if (visualTreeAsset == null)
            {
                string loadPath = $"{_rootFolder}/Editor/PoolManagerEditor.uxml";
                visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(loadPath);
                Debug.Assert(visualTreeAsset != null, $"Load Failed : {loadPath}");
            }

            if (poolManager == null)
            {
                string filePath = $"{_rootFolder}/PoolManager.asset";
                poolManager = AssetDatabase.LoadAssetAtPath<PoolManagerSO>(filePath);
                if (poolManager == null)
                {
                    Debug.LogWarning("Pool manager scriptable object is not exist, create new one");
                    poolManager = ScriptableObject.CreateInstance<PoolManagerSO>();
                    AssetDatabase.CreateAsset(poolManager, filePath);
                }
            }

            if (itemAsset == null)
            {
                string loadPath = $"{_rootFolder}/Editor/PoolItemUI.uxml";
                itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(loadPath);
                Debug.Assert(itemAsset != null, $"Load Failed : {loadPath}");
            }
        }
    }
}