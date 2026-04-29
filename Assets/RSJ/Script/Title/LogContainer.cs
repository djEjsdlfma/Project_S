using Moon._01.Script.Datas;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LogContainer : MonoBehaviour
{
    [SerializeField] private GameObject[] List1to10;
    [SerializeField] private GameObject[] List11to20;

    [SerializeField] private Button Btn1to10;
    [SerializeField] private Button Btn11to20;

    [SerializeField] private Button BtnSave;
    [SerializeField] private Button BtnRoad;

    private DataManager _data;

    public void Awake()
    {
        _data = DataManager.Instance;
        Btn1to10.interactable = false;

        for (int i = 0; i < List11to20.Length; i++)
        {
            List11to20[i].gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        SetSaveState();
    }

    private void OnDestroy()
    {
        RemoveAll();
    }

    public void ChangeList()
    {
        if (List1to10[1].activeSelf == true)
        {
            for (int i = 0; i < List11to20.Length; i++)
            {
                List11to20[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < List1to10.Length; i++)
            {
                List1to10[i].gameObject.SetActive(false);
            }


            Btn1to10.interactable = true;
            Btn11to20.interactable = false;

        }
        else
        {
            for (int i = 0; i < List11to20.Length; i++)
            {
                List11to20[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < List1to10.Length; i++)
            {
                List1to10[i].gameObject.SetActive(true);
            }

            Btn1to10.interactable = false;
            Btn11to20.interactable = true;
        } 
    }

    public void ChangeState()
    {
        RemoveAll();
        if (BtnRoad.interactable == true)
        {
            SetSaveState();


            BtnSave.interactable = true;
            BtnRoad.interactable = false;
        }
        else
        {
            SetRoadState();

            BtnSave.interactable = false;
            BtnRoad.interactable = true;
        }
    }

    private void SetRoadState()
    {
        for (int i = 0; i < List11to20.Length; i++)
        {
            int index = i; // 클로저 문제 방지
            List11to20[i].gameObject.GetComponent<Button>().onClick
                .AddListener(() =>
                {
                if (_data.SlotDataExist(index))
                    _data.LoadSlot(index);
                });
        }
        for (int i = 0; i < List1to10.Length; i++)
        {
            int index = i; // 클로저 문제 방지
            List1to10[i].gameObject.GetComponent<Button>().onClick
                .AddListener(() =>
                {
                    if (_data.SlotDataExist(index))
                        _data.LoadSlot(index);
                });
        }
    }

    private void SetSaveState()
    {
        for (int i = 0; i < List11to20.Length; i++)
        {
            int index = i; // 클로저 문제 방지
            List11to20[i].gameObject.GetComponent<Button>().onClick
                .AddListener(() =>
                {
                    _data.SlotSave(index);

                    List11to20[index].gameObject.GetComponent<LogUI>()
                        .CheckData(_data.SlotDataExist(index));
                });

            List11to20[index].gameObject.GetComponent<LogUI>()
                .CheckData(_data.SlotDataExist(index));
        }
        for (int i = 0; i < List1to10.Length; i++)
        {
            int index = i; // 클로저 문제 방지
            List1to10[i].gameObject.GetComponent<Button>().onClick
                .AddListener(() =>
                {
                    _data.SlotSave(index);

                    List1to10[index].gameObject.GetComponent<LogUI>()
                        .CheckData(_data.SlotDataExist(index));
                });

            List1to10[index].gameObject.GetComponent<LogUI>()
                .CheckData(_data.SlotDataExist(index));
        }
    }

    public void RemoveAll()
    {
        for (int i = 0; i < List11to20.Length; i++)
        {
            List11to20[i].gameObject.GetComponent<Button>().onClick
                .RemoveAllListeners();
        }
        for (int i = 0; i < List1to10.Length; i++)
        {
            List1to10[i].gameObject.GetComponent<Button>().onClick
                .RemoveAllListeners();
        }
    }
}

