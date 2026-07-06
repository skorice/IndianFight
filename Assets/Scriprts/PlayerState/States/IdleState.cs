using UnityEngine;

namespace PlayerState
{
    public class IdleState : IPlayerState
    {
        private PlayerController _player;
        private StateMachine _stateMachine;

        public IdleState(PlayerController player, StateMachine stateMachine)
        {
            _player = player;
            _stateMachine = stateMachine;
        }

        public void EnterState()
        {
            Debug.Log("Entered Idle State");
        }

        public void UpdateState()
        {
            // Проверка входа в боевую зону
            if (_player.IsInCombatZone && _player.HasEnemyInRange)
            {
                _stateMachine.ChangeState(_player.IdleFightState);
                return;
            }

            // Проверка движения
            if (_player.Input.MovementInput.magnitude > 0.1f)
            {
                _stateMachine.ChangeState(_player.MoveState);
                return;
            }
        }

        public void ExitState() { }
    }
}