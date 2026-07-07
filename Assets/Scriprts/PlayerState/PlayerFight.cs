using UnityEngine;
using System.Collections.Generic;

public class PlayerFight : MonoBehaviour
{
    private PlayerSettings settings;
    [SerializeField] private Transform triggerZone;
    [SerializeField] private Transform attackZone;

    private float cooldown;
    private List<Collider2D> enemiesInZone = new List<Collider2D>();

    public bool IsInCombat => enemiesInZone.Count > 0;

    private void Awake()
    {
        settings = GetComponent<PlayerSettings>();
        if (triggerZone == null) triggerZone = transform.Find("TriggerAttackZone");
        if (attackZone == null) attackZone = transform.Find("RadiusAttack");
    }

    private void Update()
    {
        if (cooldown > 0) cooldown -= Time.deltaTime;

        if (triggerZone != null) triggerZone.position = transform.position;
        if (attackZone != null) attackZone.position = transform.position;

        // Очищаем мёртвых врагов
        enemiesInZone.RemoveAll(e => e == null || !e.gameObject.activeSelf);

        if (IsInCombat && cooldown <= 0)
        {
            Attack();
        }
    }

    private void Attack()
    {
        cooldown = settings.AttackSpeed;

        if (attackZone != null)
        {
            var collider = attackZone.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    attackZone.position,
                    collider.radius
                );

                foreach (var hit in hits)
                {
                    if (hit.CompareTag("Enemy"))
                    {
                        Debug.Log($"Цдар прошел"); //доработать, когда будут мобы
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !enemiesInZone.Contains(other))
        {
            enemiesInZone.Add(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && enemiesInZone.Contains(other))
        {
            enemiesInZone.Remove(other);
        }
    }
    private void OnDrawGizmosSelected()
    {
        var collider = triggerZone.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(triggerZone.position, collider.radius);
        }
        

        collider = attackZone.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackZone.position, collider.radius);
        }
        
    }
}