using Moon._01.Script.Datas;
using UnityEngine;
using UnityEngine.UI;

public class LogContainer : MonoBehaviour
{
    [SerializeField] private GameObject[] List1to10;
    [SerializeField] private GameObject[] List11to20;

    [SerializeField] private Button Btn1to10;
    [SerializeField] private Button Btn11to20;

    private DataManager _data;

    public void Awake()
    {
        _data = GetComponent<DataManager>();
        Btn1to10.interactable = false;

        for (int i = 0; i < List11to20.Length; i++)
        {
            List11to20[i].gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        for (int i = 0; i < List11to20.Length; i++)
        {
    /*        List11to20[i].gameObject.GetComponent<Button>().onClick
                .AddListener(_data.SlotSave(i));*/
        }
        for (int i = 0; i < List1to10.Length; i++)
        {
            List1to10[i].gameObject.SetActive(false);
        }
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
            Btn11to20. interactable = false;
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
}

