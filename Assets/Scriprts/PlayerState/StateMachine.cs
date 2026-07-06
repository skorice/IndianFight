using UnityEngine;

namespace PlayerState
{
    public class StateMachine
    {
        public IPlayerState CurrentState { get; private set; }

        public void ChangeState(IPlayerState newState)
        {
            if (newState == null)
            {
                return;
            }

            CurrentState?.ExitState();
            CurrentState = newState;
            CurrentState?.EnterState();
        }

        public void Tick() => CurrentState?.UpdateState();
    }
}