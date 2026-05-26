using Moon._01.Script.Datas;
using Unity.XR.GoogleVr;
using UnityEngine;
using UnityEngine.UI;

public class PhotoUI : MonoBehaviour
{
    [SerializeField] private Button[] _folders;
    [SerializeField] private GameObject _text;
    [SerializeField] private GameObject _photo;

    private void Start()
    {
        for(int i = 0; i < _folders.Length; i++)
        {
            _folders[i].gameObject.SetActive(false);
        }
    }

    public void ActiveFolder()
    {
        if (DataManager.Instance.CurrentData.TryGetValue("Day", out int day))
        {
            if (day - 2 >= 5) return;

            for (int i = 0; i <= day - 2; i++)
            {
                _folders[i].gameObject.SetActive(true);
            }
        }
    }

    public void ShowMyPhoto()
    {
        for (int i = 0; i < _folders.Length; i++)
        {
            _folders[i].gameObject.SetActive(false);
        }

        _text.SetActive(true);
        _photo.SetActive(true);
    }
}
