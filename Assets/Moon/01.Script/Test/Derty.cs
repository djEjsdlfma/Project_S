using Moon._01.Script.Datas;
using UnityEngine;

namespace Moon._01.Script.Test
{
    public class Derty : MonoBehaviour
    {
        [ContextMenu("Test")]
        private void Test()
        {
            DataManager.Instance.SaveData("LSW","BAD");
            DataManager.Instance.SaveData("LSW_IQ",35);
            DataManager.Instance.SaveData("LSW_Exam",57.2f);
            DataManager.Instance.SaveData("LSW_Male",false);
            DataManager.Instance.SlotSave(1);
            DataManager.Instance.AutoSave();
        }

        [ContextMenu("TestLoad")]
        private void TestLoad()
        {
            DataManager.Instance.LoadSlot(1);

            string LSWS;
            int LSWI;
            float LSWF;
            bool LSWB;
            if(DataManager.Instance.CurrentData.TryGetValue("LSW", out LSWS))
            {
                Debug.Log(LSWS);
            }

            if (DataManager.Instance.CurrentData.TryGetValue("LSW_IQ", out LSWI))
            {
                Debug.Log(LSWI);
            }
            
            if (DataManager.Instance.CurrentData.TryGetValue("LSW_Exam", out LSWF))
            {
                Debug.Log(LSWF);
            }

            if (DataManager.Instance.CurrentData.TryGetValue("LSW_Male", out LSWB))
            {
                Debug.Log(LSWB);
            }
            
            Debug.Log("222222222222222222222222222222222222222");
            
            DataManager.Instance.LoadAutoSave();
            
            if(DataManager.Instance.CurrentData.TryGetValue("LSW", out LSWS))
            {
                Debug.Log(LSWS);
            }

            if (DataManager.Instance.CurrentData.TryGetValue("LSW_IQ", out LSWI))
            {
                Debug.Log(LSWI);
            }
            
            if (DataManager.Instance.CurrentData.TryGetValue("LSW_Exam", out LSWF))
            {
                Debug.Log(LSWF);
            }

            if (DataManager.Instance.CurrentData.TryGetValue("LSW_Male", out LSWB))
            {
                Debug.Log(LSWB);
            }
        }
    }
}