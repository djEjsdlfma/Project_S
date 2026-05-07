using System.Collections;
using System.Linq;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.So;
using LSW._02._Code.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BubbleManager : MonoBehaviour, ITabletUI
{
    //ó??
    [SerializeField] private BubbleText NPCFirstText;
    [SerializeField] private BubbleText PlayerFirstText;

    //???
    [SerializeField] private BubbleText NPCText;
    [SerializeField] private BubbleText PlayerText;
    
    [SerializeField] private ChoiceBubble PlayerChoice;

    //???? ??? ??
    [SerializeField] private GameObject NPCChatting;

    [SerializeField] private RectTransform _contaner;
    [SerializeField] private ScrollRect scrollRect;
    
    public string sheetName = "Sheet1";
    
    private WaitForSecondsRealtime _interactDelayCoroutine;

    private string _currentKey;
    private DialogueDataCore _dialogueDataCore;
    private PlayerStatCore _playerStatCore;
    private bool wasChatNpc;
    private GameObject nowBubble;

    private ChoiceBubbleData[] choiceData = new ChoiceBubbleData[3];
    
    private bool wasEndChat = false;

    private int _dialogueCount = 0;
    public bool CanInteract { get; private set; }
    
    private void Awake()
    {
        _interactDelayCoroutine = new WaitForSecondsRealtime(1.25f);
        
        _dialogueDataCore = CoreHandler.Instance.GetCore<DialogueDataCore>();
        _playerStatCore = CoreHandler.Instance.GetCore<PlayerStatCore>();

        if (!_dialogueDataCore.GetFirstDialogueByDay(sheetName, _playerStatCore.CurrentDay, out DialogueEntry entry))
        {
            Debug.Log($"No Day Dialogue in Sheet, Day : {_playerStatCore.CurrentDay}, Sheet : {sheetName}");
            return;
        }

        _currentKey = entry.key;
    }

    private void Update()
    {
        if(!CanInteract)
            return;
        
        if(Keyboard.current.dKey.wasPressedThisFrame)
        {
            SpawnMessage();
        }
    }

    private IEnumerator DelayInteract()
    {
        yield return _interactDelayCoroutine;
        EnableInteract();
    }

    public void SpawnMessage()
    {
        if(wasEndChat)
            return;
        
        if (!_dialogueDataCore.GetDialogueDataByKey(sheetName, _currentKey, out DialogueEntry data)) 
            return;
        
        DisableInteract();
        
        if (data.type == DialogueType.Select)
        {
            ChoiceBubble choice = Instantiate(PlayerChoice, _contaner);
            StartCoroutine(RefreshLayout(_contaner));
            FindChoice(data.seq, choice);
            choice.AddEvent(Choose);
        }
        else if(data.speaker == SpeakerType.NPC)
        {
            StartCoroutine(DelayInteract());
            ShowNPCText(data.content, wasChatNpc, data.speaker.ToString());
            _currentKey = data.nextKey;
        }
        else
        {
            StartCoroutine(DelayInteract());
            
            if (data.sincerity != 0)
                _playerStatCore.ChangeSincerityAmount(sheetName, data.sincerity);

            ShowPlayerText(data.content, wasChatNpc);
            if (data.nextKey == "END")
            {
                wasEndChat = true;
            }
            else
            {
                _currentKey = data.nextKey;
            }
        }
        
        _dialogueCount++;
        StartCoroutine(ScrollToBottom());
    }

    IEnumerator RefreshLayout(RectTransform rect)
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    private void ShowNPCText(string log, bool wasNPC, string speakerName = "NPC")
    {
        if (wasNPC == false)
        {
            wasChatNpc = true;
            BubbleText text = Instantiate(NPCFirstText, _contaner);
            text.InitBubble(log, 1f, speakerName);
            nowBubble = text.gameObject;
        }
        else
        {
            BubbleText text = Instantiate(NPCText, _contaner);
            text.InitBubble(log, 1f);
            nowBubble = text.gameObject;
        }

        StartCoroutine(ScrollToBottom());
        ShowBubbleDelay();
    }

    private void ShowPlayerText(string log, bool wasNPC = true)
    {
        if (wasNPC == true)
        {
            wasChatNpc = false;
            BubbleText text = Instantiate(PlayerFirstText, _contaner);
            text.InitBubble(log, 1f, "Player");
        }
        else
        {
            BubbleText text = Instantiate(PlayerText, _contaner);
            text.InitBubble(log, 1f);
        }

        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void ShowBubbleDelay()
    {
        nowBubble.SetActive(false);
        GameObject loading = Instantiate(NPCChatting, _contaner);
        StartCoroutine(DelayChat(loading));
    }

    private IEnumerator DelayChat(GameObject loadingObject)
    {
        yield return new WaitForSeconds(1f);
        if(loadingObject != null) 
            Destroy(loadingObject);
        nowBubble.SetActive(true);
        StartCoroutine(ScrollToBottom());
    }

    private void FindChoice(int seqNum, ChoiceBubble choice)
    {
        if(!_dialogueDataCore.GetAllDialogueEntry(sheetName, out var allData))
            return;
    
        var choices = allData.Values
            .Where(x => x.seq == seqNum && x.type == DialogueType.Select)
            .OrderBy(x => x.id)
            .ToList();

        if (choices.Count == 3)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                choiceData[i].ChoiceText = choices[i].content;
                choiceData[i].NextKey = choices[i].nextKey;
                choiceData[i].ChoiceSincerity = choices[i].sincerity;
            }
        
            choice.ChoiceInit(choices.Select(x => x.content).ToArray());
        }
    }

    private void Choose(GameObject target, int num)
    {
        _playerStatCore.ChangeSincerityAmount(sheetName, choiceData[num].ChoiceSincerity); 

        ShowPlayerText(choiceData[num].ChoiceText, wasChatNpc);

        _currentKey = choiceData[num].NextKey;

        Destroy(target);
        StartCoroutine(NextStepAfterChoice());
    }
    
    private IEnumerator NextStepAfterChoice()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnMessage();
    }

    public void EnableInteract()
    {
        CanInteract = true;
    }

    public void DisableInteract()
    {
        CanInteract = false;
    }
}


public struct ChoiceBubbleData
{
    public string ChoiceText;
    public string NextKey;
    public int ChoiceSincerity;
}
