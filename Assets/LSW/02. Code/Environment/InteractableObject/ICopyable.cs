
namespace LSW._02._Code.Environment.InteractableObject
{
    public interface ICopyable
    {
        public bool IsCoping { get; set; }

        public void Copy();
        public void Paste();
    }
}