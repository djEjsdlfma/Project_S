using Moon._01.Script.Datas;
using UnityEngine;

public class LogUI : MonoBehaviour
{
    [SerializeField] private GameObject _noneInfo;
    [SerializeField] private GameObject _existInfo;

    public void CheckData(bool isExist)
    {
        if(isExist)
        {
            _noneInfo.SetActive(false);
            _existInfo.SetActive(true);
        }
        else
        {
            _noneInfo.SetActive(true);
            _existInfo.SetActive(false);
        }
    }
}
