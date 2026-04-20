using System;
using Code.Combat;
using Code.Core;
using Code.Entities;
using Code.FSM;
using Code.Managers;
using Code.Stats;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Code.Units
{
    public enum UnitType
    {
        Ground,
        Air
    }

    public class Unit : Entity, ISpawnable, IPlaceable, IDamageable
    {
        public UnityEvent OnFinalDead;
        
        [field: SerializeField] public StatSO CooldownStat { get; private set; }
        [field: SerializeField] public UnitType UnitType { get; private set; }
        [field: SerializeField] public SpawnableDataSO SpawnableData { get; private set; }

        public Collider Collider { get; private set; }
        public bool IsPlaced { get; private set; }
        public int UpgradeIndex { get; protected set; }

        [SerializeField] private StateSO[] states;
        [SerializeField] private StateSO initState;
        [SerializeField] private NavMeshAgent agent;

        public Entity target;
        public Entity prevTarget;
        public float lastAttackTime = -999f;

        protected IPlaceMediator _unitManager;
        protected EntityStateMachine _stateMachine;
        protected EntityHealth _health;

        public virtual void Initialize()
        {
            _unitManager = CreateOnceManager.Instance.GetManager<UnitManager>();
            _health = GetCompo<EntityHealth>();
            Collider = GetComponent<Collider>();
            
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

            GetCompo<NavMovement>().enabled = false;
            agent.enabled = false;
        }

        public void Spawn()
        {
            _stateMachine = new EntityStateMachine(this, states);
        }

        private void Update()
        {
            _stateMachine?.UpdateStateMachine();
        }

        public void ChangeState(string stateName, bool isForce = false) => _stateMachine.ChangeState(stateName, isForce);

        [ContextMenu("Show Current State")]
        public void ShowCurrentState()
        {
            print(_stateMachine.CurrentState.ToString());
        }
        
        public void StartPlace()
        {
        }

        public void CompletePlace(Vector3 pos)
        {
            agent.enabled = true;
            GetCompo<NavMovement>().enabled = true;
            
            IsPlaced = true;
            
            _stateMachine?.ChangeState(initState.stateName);
        }

        public bool TryPlace(Vector3Int pos)
        {
            return _unitManager.TryPlace(pos, this);
        }

        public virtual void ApplyDamage(float damage, Vector3 hitPoint, Vector3 normal, Entity dealer)
        {
            _health.TakeDamage(damage);
        }

        public virtual void HandleDead()
        {
            IsDead = true;
            Collider.enabled = false;
            ChangeState("DEAD");
            
            _stateMachine.CanStatChangeable = false;
        }
    }
}