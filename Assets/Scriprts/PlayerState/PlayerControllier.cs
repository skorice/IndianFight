using UnityEngine;
using System.Collections.Generic;

namespace PlayerState
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 8f;

        [Header("Combat Settings")]
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private float attackRadius = 3f;

        [Header("References")]
        [SerializeField] private Transform attackTriggerZone;

        // Компоненты
        public Rigidbody2D Body { get; private set; }
        public InputReader Input { get; private set; }

        // Публичные свойства
        public float MoveSpeed => moveSpeed;
        public float AttackCooldown => attackCooldown;
        public float AttackRadius => attackRadius;

        // Состояния игрока
        public float AttackCooldownTimer { get; set; }
        public Transform CurrentTarget { get; set; }
        public bool HasEnemyInRange { get; set; }

        // Состояния
        public IdleState IdleState { get; private set; }
        public MoveState MoveState { get; private set; }
        public IdleFightState IdleFightState { get; private set; }
        public MoveFightState MoveFightState { get; private set; }

        private StateMachine _stateMachine;
        private List<Transform> _enemiesInZone = new List<Transform>();

        private void Awake()
        {
            Body = GetComponent<Rigidbody2D>();
            Input = new InputReader();
            _stateMachine = new StateMachine();

            IdleState = new IdleState(this, _stateMachine);
            MoveState = new MoveState(this, _stateMachine);
            IdleFightState = new IdleFightState(this, _stateMachine);
            MoveFightState = new MoveFightState(this, _stateMachine);

            if (attackTriggerZone == null)
            {
                attackTriggerZone = transform.Find("AttackTriggerZone");
                if (attackTriggerZone == null)
                {
                    Debug.LogError("AttackTriggerZone not found!");
                }
            }

            if (attackTriggerZone != null)
            {
                var collider = attackTriggerZone.GetComponent<CircleCollider2D>();
                if (collider == null)
                {
                    collider = attackTriggerZone.gameObject.AddComponent<CircleCollider2D>();
                }
                collider.isTrigger = true;
                collider.radius = attackRadius;
            }
        }

        private void Start()
        {
            _stateMachine.ChangeState(IdleState);
        }

        private void Update()
        {
            Input.Read();

            if (AttackCooldownTimer > 0)
                AttackCooldownTimer -= Time.deltaTime;

            if (attackTriggerZone != null)
                attackTriggerZone.position = transform.position;

            _stateMachine.Tick();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy") && !_enemiesInZone.Contains(other.transform))
            {
                _enemiesInZone.Add(other.transform);
                UpdateEnemyState();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                _enemiesInZone.Remove(other.transform);
                UpdateEnemyState();
            }
        }

        private void UpdateEnemyState()
        {
            _enemiesInZone.RemoveAll(t => t == null || !t.gameObject.activeSelf);

            HasEnemyInRange = _enemiesInZone.Count > 0;

            if (HasEnemyInRange)
            {
                Transform closest = null;
                float closestDistance = float.MaxValue;

                foreach (var enemy in _enemiesInZone)
                {
                    float distance = Vector2.Distance(transform.position, enemy.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = enemy;
                    }
                }
                CurrentTarget = closest;
            }
            else
            {
                CurrentTarget = null;
            }
        }

        public void PerformAttack()
        {
            if (AttackCooldownTimer > 0 || CurrentTarget == null)
                return;

            AttackCooldownTimer = attackCooldown;
        }

        private void OnDrawGizmosSelected()
        {
            if (attackTriggerZone != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(attackTriggerZone.position, attackRadius);
            }
        }
    }
}