namespace LSW._02._Code.UI
{
    public interface ITabletUI
    {
        public bool CanInteract { get; }
        public void EnableInteract();
        public void DisableInteract();
    }
}