using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Towers.Impl
{
    public class MissileTowerVisual : MonoBehaviour
    {
        public List<Transform> FireTrmList { get; private set; } = new List<Transform>();
        
        [SerializeField] private Transform fireStreamTrm;

        private List<MissileTowerBarrelVisual> _barrelList;
        
        private void Awake()
        {
            _barrelList = fireStreamTrm.GetComponentsInChildren<MissileTowerBarrelVisual>().ToList();
            
            SetFireTrms();
        }

        public void ChangeVisual(GameObject barrelParent)
        {
            foreach (MissileTowerBarrelVisual barrel in _barrelList)
            {
                Destroy(barrel.gameObject);
            }

            GameObject spawnedBarrel = Instantiate(barrelParent, fireStreamTrm);
            _barrelList = spawnedBarrel.GetComponentsInChildren<MissileTowerBarrelVisual>().ToList();

            SetFireTrms();
        }

        private void SetFireTrms()
        {
            FireTrmList.Clear();
            
            foreach (MissileTowerBarrelVisual barrelVisual in _barrelList)
            {
                FireTrmList.Add(barrelVisual.FireTrm);
            }
        }
    }
}