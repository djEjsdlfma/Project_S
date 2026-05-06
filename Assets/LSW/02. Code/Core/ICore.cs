
namespace LSW._02._Code.Core
{
    public interface ICore
    {
        public void Initialize(CoreHandler coreHandler);
        public void LoadScene(SceneType sceneType);
        public void Reset();
    }
}