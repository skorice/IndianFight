using UnityEngine;

namespace PlayerState
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 8f;

        [Header("Combat Settings")]
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private float attackRadius = 1.5f;
        [SerializeField] private LayerMask enemyLayer;

        [Header("References")]
        [SerializeField] private Transform attackTriggerZone;

        public Rigidbody2D Body { get; private set; }
        public InputReader Input { get; private set; }

        // Публичные свойства
        public float MoveSpeed => moveSpeed;
        public float AttackCooldown => attackCooldown;
        public float AttackRadius => attackRadius;
        public LayerMask EnemyLayer => enemyLayer;

        public bool IsInCombatZone { get; set; }
        public float AttackCooldownTimer { get; set; }
        public Transform CurrentTarget { get; set; }
        public bool HasEnemyInRange { get; set; }

        // Состояния
        public IdleState IdleState { get; private set; }
        public MoveState MoveState { get; private set; }
        public IdleFightState IdleFightState { get; private set; }
        public MoveFightState MoveFightState { get; private set; }

        private StateMachine _stateMachine;

        private void Awake()
        {
            Body = GetComponent<Rigidbody2D>();
            Input = new InputReader();
            _stateMachine = new StateMachine();

            // Инициализация состояний
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

            UpdateCombatZone();
            CheckEnemiesInRange();

            _stateMachine.Tick();
        }

        private void UpdateCombatZone()
        {
            if (attackTriggerZone == null) return;

            // Триггерная зона всегда следует за игроком
            attackTriggerZone.position = transform.position;
        }

        private void CheckEnemiesInRange()
        {
            if (attackTriggerZone == null) return;

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                attackTriggerZone.position,
                attackRadius,
                enemyLayer
            );

            HasEnemyInRange = hits.Length > 0;

            if (HasEnemyInRange)
            {
                float closestDistance = float.MaxValue;
                foreach (var hit in hits)
                {
                    float distance = Vector2.Distance(transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        CurrentTarget = hit.transform;
                    }
                }
            }
            else
            {
                CurrentTarget = null;
            }
        }

        public void PerformAttack()
        {
            if (AttackCooldownTimer > 0 || CurrentTarget == null) return;

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