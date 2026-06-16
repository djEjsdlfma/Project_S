using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.UI;
using UnityEngine;
using UnityEngine.UI;

public class PhotoUI : MonoBehaviour
{
    [SerializeField] private Button[] _folders;
    [SerializeField] private GameObject _text;
    [SerializeField] private PhotoContainer photo;

    private GameStatueCore _gameStatueCore;
    
    private void Start()
    {
        _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
        
        for(int i = 0; i < _folders.Length; i++)
        {
            _folders[i].gameObject.SetActive(false);
        }
    }

    public void ActiveFolder()
    {
        for (int i = 0; i <= _gameStatueCore.CurrentDay - 2 && i < 6; i++)
        {
            if(i >= _folders.Length || _folders[i] == null)
                continue;
            
            _folders[i].gameObject.SetActive(true);
        }
    }

    public void ShowMyPhoto()
    {
        for (int i = 0; i < _folders.Length; i++)
        {
            _folders[i].gameObject.SetActive(false);
        }
        
        _text.SetActive(photo.PhotoCount == 0);
        photo.gameObject.SetActive(true);
    }

    public void BackSelectFolder()
    {
        ActiveFolder();
        photo.gameObject.SetActive(false);
    }
}
