using UnityEngine;

public class PlayerSettings : MonoBehaviour 
{
    [Header("Базовые настройки игрока")]
    [SerializeField] private float BASEmoveSpeed = 5f;
    [SerializeField] private float BASEattackPower = 1f;
    [SerializeField] private float BASEattackSpeed = 0.6f;
    // Баффы за уровни
    [Header("Настройки прокачки")]
    [SerializeField] private float BUFFmoveSpead = 0.1f;
    [SerializeField] private float BUFFattackPower = 0.5f;
    [SerializeField] private float BUFFattackSpeed = 0.05f;

    [Header("Текущий уровень")]
    [SerializeField] private int currentLevel = 1;
    public float MoveSpeed => BASEmoveSpeed;
    public float AttackPower => BASEattackPower;
    public float AttackSpeed => BASEattackSpeed;
}
