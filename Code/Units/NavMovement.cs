using Code.Entities;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Units
{
    public class NavMovement : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float stopOffset = 0.05f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private bool isUpdateRotation;
        
        private Unit _unit;
        private float _stopMultiplier = 1f;

        public float StopOffset => stopOffset;

        public bool IsStop => agent.isStopped;
        
        public bool IsArrived => !agent.pathPending &&
                                 agent.remainingDistance < agent.stoppingDistance + stopOffset * _stopMultiplier;
        public float RemainDistance => agent.pathPending ? -1 : agent.remainingDistance;
        
        public void Initialize(Entity entity)
        {
            _unit = entity as Unit;
            agent.speed = moveSpeed;
            agent.updateRotation = isUpdateRotation;
        }

        private void Update()
        {
            SetDistanceMultiplier();
            
            if (agent.hasPath && agent.isStopped == false && agent.path.corners.Length > 0)
            {
                LookAtTarget(agent.steeringTarget);
            }
        }

        private void SetDistanceMultiplier()
        {
            if (_unit.target == null)
            {
                _stopMultiplier = 1f;
                return;
            }

            _stopMultiplier = _unit.target.transform.localScale.x;
        }

        public Quaternion LookAtTarget(Vector3 target, bool isSmooth = true)
        {
            Vector3 direction = target - _unit.transform.position;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            if (isSmooth)
                _unit.transform.rotation = Quaternion.Slerp(_unit.transform.rotation, lookRotation,
                    Time.deltaTime * rotationSpeed);
            else
                _unit.transform.rotation = lookRotation;

            return lookRotation;
        }

        public void SetStop(bool isStop)
        {
            agent.isStopped = isStop;
            agent.velocity = Vector3.zero;
        }
        public void SetVelocity(Vector3 velocity) => agent.velocity = velocity;
        public void SetSpeed(float speed) => agent.speed = speed;
        public void SetDestination(Vector3 destination) => agent.SetDestination(destination);

        [ContextMenu("Print dsds")]
        public void asdasd()
        {
            print(IsArrived);
            print($"{agent.remainingDistance} < {agent.stoppingDistance + stopOffset * _stopMultiplier}");
        }
    }
}