using System;
using System.Linq;
using Code.Core;
using Code.UI.Spawnable;
using Code.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Units
{
    public class UnitUI : MonoBehaviour
    {
        public int Amount { get; protected set; }
        
        [SerializeField] protected SpawnableListSO unitList;
        [SerializeField] protected Image unitImage;
        [SerializeField] protected TextMeshProUGUI amountText;
        
        protected UnitDataSO _unitData;
        protected SpawnableUI _spawnableUI;
        protected ISpawnable _unitPrefab;
        
        public virtual void Initialize(UnitDataSO unitData)
        {
            unitImage.sprite = unitData.spawnableSprite;
            _unitData = unitData;
            Amount = 1;
            SetAmountText(Amount);

            _spawnableUI = FindAnyObjectByType<SpawnableUI>();

            _unitPrefab = unitList.spawnableDataList
                .FirstOrDefault(unit => unit == _unitData)?.prefab.GetComponent<ISpawnable>();
        }

        public virtual void AddAmount() => SetAmountText(++Amount);
        public virtual void DecreaseAmount() => SetAmountText(--Amount);

        public virtual void SetAmountText(int amount) => amountText.text = $"x{amount}";

        public virtual void PopUpInfoUI()
        {
            _spawnableUI.Select(_unitPrefab);
        }
    }
}