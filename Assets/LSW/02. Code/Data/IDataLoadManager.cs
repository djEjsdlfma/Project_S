using System;

namespace LSW._02._Code.Data
{
    public interface IDataLoadManager
    {
        public void LoadData();
        public event Action<string> OnLoadError;
        
        float Progress { get; }
        bool IsDone { get; }
    }
}