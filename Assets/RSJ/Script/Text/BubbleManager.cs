
using System.Collections;
using System.Linq;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.UI;
using UnityEngine;
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

    public string currentKey = "DAY1_NPC_NORM_001_1";
    public string sheetName = "Sheet1";
    
    private WaitForSecondsRealtime _interactDelayCoroutine;
    
    private DialogueDataCore _dialogueDataCore;
    private bool wasChatNpc;
    private GameObject nowBubble;
    private string choiceText2;
    private string nextKey1;
    private string choiceText1;
    private string nextKey2;
    private bool wasEndChat = false;

    private int _dialogueCount = 0;
    public bool CanInteract { get; private set; }

    private void Awake()
    {
        _interactDelayCoroutine = new WaitForSecondsRealtime(1.25f);
        _dialogueDataCore = CoreHandler.Instance.GetCore<DialogueDataCore>();
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
        
        if (!_dialogueDataCore.GetDialogueDataByKey(sheetName, currentKey, out DialogueData data)) 
            return;
        
        DisableInteract();
        if(data.Speaker == SpeakerType.Npc)
        {
            if(data.Type == DialogueType.Normal || data.Type == DialogueType.Reaction)
            {
                StartCoroutine(DelayInteract());
                ShowNPCText(data.Content, wasChatNpc, data.Speaker.ToString());
                currentKey = data.NextKey;
            }
            else if (data.Type == DialogueType.Select)
            {
                ChoiceBubble choice = Instantiate(PlayerChoice, _contaner);
                StartCoroutine(RefreshLayout(_contaner));
                FindChoice(data.Seq, choice);
                choice.AddEvent(ChoseOne, ChoseTwo);
            }
        }
        else
        {
            StartCoroutine(DelayInteract());
            ShowPlayerText(data.Content, wasChatNpc);
            if (data.NextKey == "END")
            {
                wasEndChat = true;
            }
            else if (data.NextKey != "-")
            {
                currentKey = data.NextKey;
            }
            else if(_dialogueDataCore.GetDialogueKeyByIndex(_dialogueCount, out string key))
            {
                currentKey = key;
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
        _dialogueDataCore.GetAllDialogueData(sheetName, out var allData);
        var choices = allData.Values
            .Where(x => x.Seq == seqNum && x.Type == DialogueType.Select)
            .OrderBy(x => x.ID)
            .ToList();

        if (choices.Count >= 2)
        {
            choiceText1 = choices[0].Content;
            choiceText2 = choices[1].Content;
        
            nextKey1 = choices[0].NextKey;
            nextKey2 = choices[1].NextKey;
            choice.ChoiceInit(choices[0].Content, choices[1].Content);
        }
    }

    private void ChoseOne(GameObject target)
    {
        ShowPlayerText(choiceText1, wasChatNpc);

        currentKey = nextKey1;

        Destroy(target);
        StartCoroutine(NextStepAfterChoice());
    }

    private void ChoseTwo(GameObject target)
    {
        ShowPlayerText(choiceText2, wasChatNpc);

        currentKey = nextKey2;

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
