using System.Collections.Generic;
using System.Linq;
using Moon._01.Script.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Moon._01.Script.Memos
{
    public class MemoCreatedButton : MonoBehaviour
    {
       [SerializeField] private List<Button> buttons = new();
       
       [SerializeField]private GameObject delBlack;
       [SerializeField]private GameObject saveBlack;
       
       private List<Button> _currentButton = new List<Button>();
       public Human CurrentGuest { get; private set; }

       private MemoSystem _memoSystem;
       
       public void DelOpen(bool close) => delBlack.SetActive(!close);
       public void SaveOpen(bool close) => saveBlack.SetActive(!close);
           
       public void ChangeCurrentGuest(Human currentGuest)
       {
           CurrentGuest = currentGuest;
       }

       public void SetMemoSystem(MemoSystem memoSystem)
       {
           _memoSystem = memoSystem;

           CurrentGuest = Human.None;
           int i = 0;
           foreach (var v in _memoSystem.memoDict)
           {
               if(v.Key == Human.None)
                   continue;
               buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = v.Value;
               buttons[i].onClick.AddListener(() => ChangeCurrentGuest(v.Key));
               if (DataManager.Instance.TryGetValue(v.Key + "IsFind", out bool b))
               {
                   if (b)
                   {
                       buttons[i].gameObject.SetActive(true);
                       _currentButton.Add(buttons[i]);
                       if (CurrentGuest == Human.None)
                       {
                           CurrentGuest = v.Key;
                       }
                   }
                   else
                   {
                       buttons[i].gameObject.SetActive(false);
                   }
               }
               else
               {
                   buttons[i].gameObject.SetActive(false);
               }
               i++;
           }
           gameObject.SetActive(false);
       }

       public void AddGuest(Human human)
       {
           int i = 0;
           foreach (var v in _memoSystem.memoDict)
           {
               if (v.Key == human)
               {
                   buttons[i].gameObject.SetActive(true);
                   ChangeCurrentGuestToFirst();
               }

               i++;
           }
       }

       public void ChangeCurrentGuestToFirst()
       {
           foreach (var v in _memoSystem.memoDict)
           {
                if (DataManager.Instance.TryGetValue(v.Key + "IsFind", out bool b))
                {
                     if (b)
                     {
                          CurrentGuest = v.Key;
                          break;
                     }
                }
           }
       }
    }
}