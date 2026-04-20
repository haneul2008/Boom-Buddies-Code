using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.ETC;
using Code.Managers;
using Code.Save;
using HNLib.Dependencies;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Stages
{
    public class FriendlyMatchUI : MonoBehaviour
    {
        [SerializeField] private SaveIdSO spawnableId;
        [SerializeField] private SaveIdSO gameDataId;
        [SerializeField] private FriendlyMatchElement element;
        [SerializeField] private RectTransform elementTrm;
        [SerializeField] private float rewardMultiplier;

        [Inject] private SaveManager _saveManager;

        private readonly List<FriendlyMatchElement> _elements = new List<FriendlyMatchElement>();
        private VerticalLayoutGroup _layoutGroup;
        private bool _isInit;

        private void Start()
        {
            Active(false);
        }

        public void Active(bool isActive)
        {
            if (isActive && _isInit == false)
            {
                _saveManager.GetAllUserData(false, HandleDataLoaded);
                _isInit = true;
            }

            gameObject.SetActive(isActive);
        }

        public void Refresh()
        {
            for(int i = _elements.Count - 1; i >= 0; --i)
                Destroy(_elements[i].gameObject);
                
            _elements.Clear();
            
            _saveManager.GetAllUserData(false, HandleDataLoaded);
        }

        private void HandleDataLoaded(List<DataCollection> list)
        {
            InitElements(list);
            SetContentTrmSize(_elements.Count);
        }

        private void InitElements(List<DataCollection> list)
        {
            foreach (DataCollection collection in list)
            {
                SaveData spawnableData = GetSaveData(collection, spawnableId);
                SaveData gameData = GetSaveData(collection, gameDataId);

                if (spawnableId == null || gameData == null) continue;

                GameSaveData gameSaveData =
                    JsonConvert.DeserializeObject<GameSaveData>(gameData.data, JsonSetting.JsonSettings);
                string userName = gameSaveData.userName;

                SpawnableSaveData spawnableSaveData =
                    JsonConvert.DeserializeObject<SpawnableSaveData>(spawnableData.data, JsonSetting.JsonSettings);

                int combatScore = 0;

                combatScore += CalculateTowerScore(GetInfos(spawnableSaveData, SpawnableType.Tower));
                combatScore += CalculateTowerScore(GetInfos(spawnableSaveData, SpawnableType.Wall));

                FriendlyMatchElement newElement = Instantiate(element, elementTrm);
                newElement.Initialize(userName, combatScore, Mathf.RoundToInt(combatScore * rewardMultiplier), spawnableSaveData);
                _elements.Add(newElement);
            }
        }

        private SaveData GetSaveData(DataCollection collection, SaveIdSO saveId)
        {
            return collection.dataList.FirstOrDefault(data => data.saveId == saveId.saveID);
        }

        private SpawnableInfoDatas GetInfos(SpawnableSaveData saveData, SpawnableType spawnableType)
        {
            return saveData.infoDatas.FirstOrDefault(data => data.spawnableType == spawnableType);
        }

        private int CalculateTowerScore(SpawnableInfoDatas infoDatas)
        {
            int result = 0;

            foreach (SpawnableInfoData infoData in infoDatas.saveDatas)
            {
                foreach (PolymorphicWrapper wrapper in infoData.instanceDatas)
                {
                    Type type = Type.GetType(wrapper.type);
                    SpawnableInstanceData instanceData =
                        JsonConvert.DeserializeObject(wrapper.json, type, JsonSetting.JsonSettings)
                            as SpawnableInstanceData;

                    if (instanceData is GoldTowerData)
                        instanceData.requiredGold = 0;
                        
                    result += instanceData.requiredGold + instanceData.requiredGold * instanceData.upgrade;
                }
            }

            return result;
        }
        
        private void SetContentTrmSize(int contentCnt)
        {
            _layoutGroup ??= elementTrm.GetComponent<VerticalLayoutGroup>();
            RectTransform friendlyMatchTrm = element.transform as RectTransform;

            float paddings = _layoutGroup.padding.top + _layoutGroup.padding.bottom;
            float spacings = (contentCnt - 1) * _layoutGroup.spacing;
            float y = friendlyMatchTrm.sizeDelta.y * contentCnt + paddings + spacings;
            print(y);
            elementTrm.sizeDelta = new Vector2(elementTrm.sizeDelta.x, y);
        }
    }
}