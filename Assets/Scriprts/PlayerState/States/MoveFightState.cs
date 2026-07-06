using UnityEngine;

namespace PlayerState
{
    public class MoveFightState : IPlayerState
    {
        private PlayerController _player;
        private StateMachine _stateMachine;
        private float _attackTimer;

        public MoveFightState(PlayerController player, StateMachine stateMachine)
        {
            _player = player;
            _stateMachine = stateMachine;
        }

        public void EnterState()
        {
            _attackTimer = 0;
            Debug.Log("Состояние движения и боя");
        }

        public void UpdateState()
        {
            Vector2 movement = _player.Input.MovementInput;

            // Если врагов нет - выходим из боя
            if (!_player.HasEnemyInRange)
            {
                _stateMachine.ChangeState(_player.MoveState);
                return;
            }

            // Движение
            if (movement.magnitude > 0.1f)
            {
                _player.Body.linearVelocity = movement * _player.MoveSpeed;
            }
            else
            {
                // Если остановились - переходим в IdleFightState
                _stateMachine.ChangeState(_player.IdleFightState);
                return;
            }

            // Атака с кулдауном
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= _player.AttackCooldown && _player.CurrentTarget != null)
            {
                // Поворот к цели перед атакой
                Vector2 direction = (_player.CurrentTarget.position - _player.transform.position).normalized;
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                _player.transform.rotation = Quaternion.Euler(0, 0, targetAngle);

                _player.PerformAttack();
                _attackTimer = 0;
            }
        }

        public void ExitState()
        {
            _player.Body.linearVelocity = Vector2.zero;
        }
    }
}