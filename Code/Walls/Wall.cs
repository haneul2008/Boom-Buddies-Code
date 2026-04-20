using System.Collections;
using System.Collections.Generic;
using Code.Combat;
using Code.Core;
using Code.Entities;
using Code.EventSystems;
using Code.Extension;
using Code.Managers;
using Code.Stats;
using Code.Upgrade;
using UnityEngine;

namespace Code.Walls
{
    public class Wall : Entity, ISpawnable, IPlaceable, IDamageable, IUpgradeable
    {
        public SpawnableDataSO SpawnableData => wallData;
        public UpgradeDataListSO MyUpgrade => SpawnableData.upgrades;
        public bool IsPlaced { get; private set; }
        public int UpgradeLevel { get; set; }

        [SerializeField] private MeshFilter visual;
        [SerializeField] private Mesh unConnectMesh, connectMesh;
        [SerializeField] private GameEventChannelSO systemChannel;
        [SerializeField] private GameEventChannelSO wallChannel;
        [SerializeField] private List<MeshFilter> visuals = new List<MeshFilter>();
        [SerializeField] private WallDataSO wallData;

        private IPlaceMediator _wallManager;
        private EntityHealth _health;
        private Dictionary<Vector3Int, Wall> _wallPairs = new Dictionary<Vector3Int, Wall>();

        public void Initialize(bool isCombatStage)
        {
            if (isCombatStage == false)
            {
                BoxCollider boxCollider = GetComponent<BoxCollider>();
                Vector3 size = boxCollider.size;
                Vector3 center = boxCollider.center;

                boxCollider.center = new Vector3(0, center.y / 2, 0);
                boxCollider.size = new Vector3(1, size.y / 2, 1);
            }
        }
        
        public void Spawn()
        {
            _wallManager = CreateOnceManager.Instance.GetManager<WallManager>();
            wallChannel.AddListener<CheckNearWallEvent>(HandleCheckNearWall);
            _health = GetCompo<EntityHealth>();
        }

        private void OnDestroy()
        {
            wallChannel.RemoveListener<CheckNearWallEvent>(HandleCheckNearWall);
        }

        private void HandleCheckNearWall(CheckNearWallEvent evt)
        {
            bool isConnect = false;
            _wallPairs = evt.wallPairs;
            
            foreach (MeshFilter meshFilter in visuals)
                Destroy(meshFilter.gameObject);

            visuals.Clear();

            Vector3[] checkDirs = new Vector3[4]
            {
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1)
            };

            foreach (Vector3 checkDir in checkDirs)
            {
                Vector3Int targetPos = (transform.position + new Vector3(checkDir.x, 0, checkDir.z) * 2).ToVector3Int();
                if (evt.wallPairs.ContainsKey(targetPos))
                {
                    MeshFilter newVisual = Instantiate(visual, transform);
                    Vector3 dir = (targetPos - transform.position).normalized.RemoveY();
                    newVisual.transform.rotation = Quaternion.LookRotation(dir);
                    newVisual.mesh = connectMesh;
                    visuals.Add(newVisual);

                    if (isConnect == false) isConnect = true;
                }
            }

            if (isConnect == false)
            {
                MeshFilter newVisual = Instantiate(visual, transform);
                newVisual.mesh = unConnectMesh;
                visuals.Add(newVisual);
            }
        }

        public void StartPlace()
        {
        }

        public void CompletePlace(Vector3 pos)
        {
            IsPlaced = true;
        }

        public bool TryPlace(Vector3Int pos)
        {
            return _wallManager.TryPlace(pos, this);
        }

        public void ApplyDamage(float damage, Vector3 hitPoint, Vector3 normal, Entity dealer)
        {
            _health.TakeDamage(damage);
        }

        public void HandleDead()
        {
            IsDead = true;
            StartCoroutine(DeadCoroutine());
        }

        private IEnumerator DeadCoroutine()
        {
            yield return new WaitForEndOfFrame();
            
            Destroy(gameObject);
            systemChannel.RaiseEvent(SystemEvents.BakeMapEvent);
            wallChannel.RaiseEvent(WallEvents.CheckNearWallEvent.Initializer(_wallPairs));
        }
        
        public void Upgrade(UpgradeDataSO upgradeData)
        {
            WallUpgradeDataSO wallUpgradeData = upgradeData as WallUpgradeDataSO;

            if (wallUpgradeData is null)
            {
                Debug.LogWarning("upgrade data is not wall upgrade data");
                return;
            }
            
            EntityStatCompo entityStatCompo = GetCompo<EntityStatCompo>(true);
            
            foreach (UpgradeStat upgradeStat in upgradeData.upgrades)
            {
                StatSO targetStat = entityStatCompo.GetStat(upgradeStat.targetStat);
                targetStat.AddModifier(upgradeData, upgradeStat.modifyValue);
            }

            unConnectMesh = wallUpgradeData.targetMesh;
            connectMesh = wallUpgradeData.connectMesh;
            
            HandleCheckNearWall(WallEvents.CheckNearWallEvent.Initializer(_wallPairs));
        }
    }
}