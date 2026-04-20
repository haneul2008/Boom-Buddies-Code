using Code.Combat;
using Code.Core;
using Code.Entities;
using Code.EventSystems;
using Code.Extension;
using Code.FSM;
using Code.Managers;
using Code.Stats;
using Code.Upgrade;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Code.Towers
{
    public abstract class Tower : Entity, ISpawnable, IPlaceable, IDamageable, IUpgradeable
    {
        public UnityEvent OnFinalDeadEvent;

        [field: SerializeField] public int Priority { get; private set; }
        [field: SerializeField] public bool IsAutoRotate { get; set; } = true;
        [field: SerializeField] public bool CanDetectWallOut { get; set; } = true;

        public SpawnableDataSO SpawnableData => towerData;
        public UpgradeDataListSO MyUpgrade => SpawnableData.upgrades;
        public bool IsRotating { get; private set; }
        public bool IsPlaced { get; private set; }
        public int UpgradeLevel { get; set; }
        public Vector3Int[,] Bound { get; private set; }
        public int Size => Mathf.CeilToInt(transform.localScale.x);
        
        [HideInInspector] public float lastAttackTime = -999f;
        public Entity target;

        [SerializeField] protected Transform rotationTrm;
        [SerializeField] protected MeshFilter meshFilter;
        [SerializeField] private TowerDataSO towerData;
        [SerializeField] private GameEventChannelSO systemChannel;
        [SerializeField] private StateSO[] states;
        [SerializeField] private StateSO initState;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float rotationThreshold = 5f;

        protected IPlaceMediator _towerManager;
        protected EntityHealth _health;
        protected EntityStateMachine _stateMachine;
        protected Collider _collider;

        public virtual void Initialize(TowerDataSO towerData, bool isCombatStage)
        {
            this.towerData = towerData;
            _towerManager = CreateOnceManager.Instance.GetManager<TowerManager>();
            _health = GetCompo<EntityHealth>();
            _collider = GetComponent<Collider>();
            
            if (isCombatStage == false)
            {
                BoxCollider boxCollider = _collider as BoxCollider;
                
                if(boxCollider is null) return;
                
                Vector3 size = boxCollider.size;
                Vector3 center = boxCollider.center;

                boxCollider.center = new Vector3(0, center.y / 2, 0);
                boxCollider.size = new Vector3(1, size.y / 2, 1);
            }
        }

        protected virtual void Update()
        {
            AutoRotate();
            
            _stateMachine?.UpdateStateMachine();
        }

        protected virtual void FixedUpdate() => _stateMachine?.FixedUpdateStateMachine();

        private void AutoRotate()
        {
            if (IsAutoRotate && target != null)
            {
                Quaternion towerRot = rotationTrm.rotation;
                Quaternion targetRot = Quaternion.LookRotation(GetDirection());

                IsRotating = Quaternion.Angle(rotationTrm.rotation, targetRot) > rotationThreshold;
                
                if(IsRotating == false) return;
                
                rotationTrm.rotation = Quaternion.Lerp(towerRot, targetRot, rotationSpeed * Time.deltaTime);
            }
        }

        private Vector3 GetDirection()
        {
            Vector3 targetPos = target.transform.position;
            Vector3 dir = targetPos - transform.position;
            return dir.RemoveY();
        }
        
        public abstract void Spawn();
        
        public virtual void StartPlace()
        {
        }

        public virtual void CompletePlace(Vector3 pos)
        {
            Bound = CalculateBound(pos.ToVector3Int());
            IsPlaced = true;

            _stateMachine = new EntityStateMachine(this, states);
            ChangeState(initState.stateName);
        }

        public void Look(Vector3 dir) => rotationTrm.rotation = Quaternion.LookRotation(dir);

        public void ChangeState(string stateName) => _stateMachine.ChangeState(stateName);

        public Vector3Int[,] CalculateBound(Vector3Int pos)
        {
            Vector3Int center = pos;

            Bound = new Vector3Int[Size, Size];

            Vector3Int startPos = Size == 1 ? center : new Vector3Int(center.x - Size, center.y, center.z + Size);

            if (Size % 2 == 0)
            {
                startPos.x += 1;
                startPos.z -= 1;
            }

            return FillBound(startPos);
        }

        private Vector3Int[,] FillBound(Vector3Int startPos)
        {
            Vector3Int[,] res = new Vector3Int[Size, Size];

            for (int i = 0; i < Size; ++i)
            {
                for (int j = 0; j < Size; ++j)
                {
                    Vector3Int adder = new Vector3Int(i * 2, 0, -(j * 2));
                    Vector3Int targetPos = startPos + adder;
                    res[i, j] = targetPos;
                }
            }

            return res;
        }

        public bool TryPlace(Vector3Int pos)
        {
            return _towerManager.TryPlace(pos, this);
        }
        
        public virtual void Upgrade(UpgradeDataSO upgradeData)
        {
            EntityStatCompo entityStatCompo = GetCompo<EntityStatCompo>(true);
            
            foreach (UpgradeStat upgradeStat in upgradeData.upgrades)
            {
                StatSO targetStat = entityStatCompo.GetStat(upgradeStat.targetStat);
                targetStat.AddModifier(upgradeData, upgradeStat.modifyValue);
            }
            
            SetUpgradeVisual(upgradeData);
        }

        protected virtual void SetUpgradeVisual(UpgradeDataSO upgradeData)
        {
            meshFilter.mesh = upgradeData.targetMesh;
        }

        public virtual void ApplyDamage(float damage, Vector3 hitPoint, Vector3 normal, Entity dealer)
        {
            _health.TakeDamage(damage);
        }

        public virtual void HandleDead()
        {
            systemChannel.RaiseEvent(SystemEvents.BakeMapEvent);
            IsDead = true;
            _collider.enabled = false;
            IsAutoRotate = false;
            ChangeState("DEAD");
            
            _stateMachine.CanStatChangeable = false;
        }

        [ContextMenu("Show Current State")]
        public void ShowCurrentState()
        {
            print(_stateMachine.CurrentState.ToString());
        }

        private void OnDrawGizmos()
        {
            if(target == null) return;
            Gizmos.color = Color.red;
            Vector3 dir = (target.transform.position - transform.position).normalized.RemoveY();
            Gizmos.DrawRay(transform.position + new Vector3(0, 0.5f, 0), dir * 15f);
        }
    }
}