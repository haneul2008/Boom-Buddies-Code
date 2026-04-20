using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.UI.Shops
{
    public abstract class SpawnableBuyUI<T> : ShopContentUI where T : ISpawnable
    {
        [SerializeField] protected SpawnableListSO spawnableList;
        [SerializeField] protected RectTransform contentTrm;
        [SerializeField] protected SpawnableBuyElement<T> buyElement;
        [SerializeField] protected List<SpawnableDataSO> excludeDatas;

        private void Awake()
        {
            List<SpawnableDataSO> spawnables = spawnableList.spawnableDataList
                .Where(data => excludeDatas.Contains(data) == false).ToList();
            
            SetContentTrmSize(spawnables);
            InitElements(spawnables);
        }

        private void InitElements(List<SpawnableDataSO> spawnables)
        {
            foreach (SpawnableDataSO data in spawnables)
            {
                SpawnableBuyElement<T> newElement = Instantiate(buyElement, contentTrm);
                newElement.Initialize(data);
            }
        }

        private void SetContentTrmSize(List<SpawnableDataSO> spawnables)
        {
            HorizontalLayoutGroup layoutGroup = contentTrm.GetComponent<HorizontalLayoutGroup>();
            RectTransform elementTrm = buyElement.transform as RectTransform;

            float paddings = layoutGroup.padding.left + layoutGroup.padding.right;
            float spacings = (spawnables.Count - 1) * layoutGroup.spacing;
            float x = elementTrm.sizeDelta.x * spawnables.Count + paddings + spacings;
            contentTrm.sizeDelta = new Vector2(x, contentTrm.sizeDelta.y);
        }

        public override void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}