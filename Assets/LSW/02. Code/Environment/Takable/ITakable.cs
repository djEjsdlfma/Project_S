namespace LSW._02._Code.Environment.Takable
{
    public interface ITakable
    {
        public void Take();
        public bool IsDisableCapture();
        public bool CanBeTaken();
    }
}