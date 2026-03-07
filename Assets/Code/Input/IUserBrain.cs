namespace Tulip.Input
{
    public interface IUserBrain
    {
        public bool WantsToPause { get; }
        public bool WantsToCancel { get; }
        public int? TabSwitchDelta { get; }
    }
}
