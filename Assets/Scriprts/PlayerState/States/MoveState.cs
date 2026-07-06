using UnityEngine;

namespace PlayerState
{
    public class MoveState : IPlayerState
    {
        private PlayerController _player;
        private StateMachine _stateMachine;

        public MoveState(PlayerController player, StateMachine stateMachine)
        {
            _player = player;
            _stateMachine = stateMachine;
        }

        public void EnterState()
        {
            Debug.Log("Состояние движения");
        }

        public void UpdateState()
        {
            Vector2 movement = _player.Input.MovementInput;

            // Проверка боевого состояния
            if (_player.HasEnemyInRange)
            {
                _stateMachine.ChangeState(_player.MoveFightState);
                return;
            }

            // Движение
            if (movement.magnitude > 0.1f)
            {
                _player.Body.linearVelocity = movement * _player.MoveSpeed;

            }
            else
            {
                _stateMachine.ChangeState(_player.IdleState);
                return;
            }
        }

        public void ExitState()
        {
            _player.Body.linearVelocity = Vector2.zero;
        }
    }
}