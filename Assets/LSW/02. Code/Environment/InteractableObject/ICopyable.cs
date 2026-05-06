
namespace LSW._02._Code.Environment.InteractableObject
{
    public interface ICopyable
    {
        public bool IsCopying { get; set; }

        public void Copy();
        public void Paste();
    }
}