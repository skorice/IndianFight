using UnityEngine;

namespace PlayerState
{
    public class IdleFightState : IPlayerState
    {
        private PlayerController _player;
        private StateMachine _stateMachine;

        public IdleFightState(PlayerController player, StateMachine stateMachine)
        {
            _player = player;
            _stateMachine = stateMachine;
        }

        public void EnterState()
        {
            Debug.Log("Состояние боя, стоя на месте");
            _player.PerformAttack();
        }

        public void UpdateState()
        {
            // Проверка выхода из боевого состояния
            if (!_player.HasEnemyInRange)
            {
                _stateMachine.ChangeState(_player.IdleState);
                return;
            }

            // Проверка движения
            if (_player.Input.MovementInput.magnitude > 0.1f)
            {
                _stateMachine.ChangeState(_player.MoveFightState);
                return;
            }

            if (_player.AttackCooldownTimer <= 0)
            {
                _player.PerformAttack();
            }
        }

        public void ExitState() { }
    }
}