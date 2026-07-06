namespace PlayerState
{
    public interface IPlayerState
    {
        void EnterState();
        void UpdateState();
        void ExitState();
    }
}