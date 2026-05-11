using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.So;
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
    
    [SerializeField] private CloseCafeButton closeCafeBtn;
    
    public event Action onEndChat;
    
    private WaitForSecondsRealtime _interactDelayCoroutine;

    private string _currentKey;
    private DialogueDataCore _dialogueDataCore;
    private GameStatueCore _gameStatueCore;
    private bool wasChatNpc;
    private GameObject nowBubble;

    private ChoiceBubbleData[] choiceData = new ChoiceBubbleData[3];
    
    private List<GameObject> _allDialogueUI = new List<GameObject>();
    private Guest _currentGuest;
    private string _currentGuestSheetName;
    private Dictionary<Guest, SavedDialogueData> _savedDialogue = new Dictionary<Guest, SavedDialogueData>();
    private Coroutine interactDelayRoutine;
    private ChoiceBubble _currentChoiceUI;
    private bool _isChoiceActive;
    private int _currentChoiceSeq;
    
    private bool wasEndChat = false;
    public bool CanInteract { get; private set; }
    
    private void Awake()
    {
        _interactDelayCoroutine = new WaitForSecondsRealtime(1.25f);
        
        _dialogueDataCore = CoreHandler.Instance.GetCore<DialogueDataCore>();
        _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();

        if(closeCafeBtn != null)
            onEndChat += closeCafeBtn.EnableInteractable;
        
        if (_savedDialogue == null)
            _savedDialogue = new Dictionary<Guest, SavedDialogueData>();
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
        interactDelayRoutine = null;
        EnableInteract();
    }

    public void SpawnMessage()
    {
        if (wasEndChat)
            return;

        if (string.IsNullOrEmpty(_currentGuestSheetName) || _currentGuest == Guest.None)
            return;

        if (!_dialogueDataCore.GetDialogueDataByKey(_currentGuestSheetName, _currentKey, out DialogueEntry data))
            return;

        DisableInteract();

        if (data.nextKey == "END")
        {
            if (data.sincerity != 0)
                _gameStatueCore.ChangeSincerityAmount(_currentGuestSheetName, data.sincerity);

            ShowPlayerText(data.content, wasChatNpc);
            wasEndChat = true;
            onEndChat?.Invoke();
        }
        else if (data.type == DialogueType.Select)
        {
            ChoiceBubble choice = Instantiate(PlayerChoice, _contaner);
            _allDialogueUI.Add(choice.gameObject);
            
            _currentChoiceUI = choice;
            _isChoiceActive = true;
            _currentChoiceSeq = data.seq;

            FindChoice(data.seq, choice);
            choice.AddEvent(Choose);
        }
        else if (data.speaker == SpeakerType.NPC)
        {
            ShowNPCText(data.content, wasChatNpc, data.speaker.ToString());
            _currentKey = data.nextKey;

            interactDelayRoutine = StartCoroutine(DelayInteract());
        }
        else
        {
            if (data.sincerity != 0)
                _gameStatueCore.ChangeSincerityAmount(_currentGuestSheetName, data.sincerity);

            ShowPlayerText(data.content, wasChatNpc);
            _currentKey = data.nextKey;

            interactDelayRoutine = StartCoroutine(DelayInteract());
        }

        StartCoroutine(RefreshLayout(_contaner));
        StartCoroutine(ScrollToBottom());
    }

    private void ShowNPCText(string log, bool wasNPC, string speakerName = "NPC")
    {
        bool isFirst = !wasNPC;

        if (isFirst)
        {
            wasChatNpc = true;

            BubbleText text = Instantiate(NPCFirstText, _contaner);
            _allDialogueUI.Add(text.gameObject);
            text.InitBubble(log, 1f, speakerName);

            nowBubble = text.gameObject;
        }
        else
        {
            BubbleText text = Instantiate(NPCText, _contaner);
            _allDialogueUI.Add(text.gameObject);
            text.InitBubble(log, 1f);

            nowBubble = text.gameObject;
        }

        AddHistory(SpeakerType.NPC, log, isFirst);

        StartCoroutine(ScrollToBottom());
        ShowBubbleDelay();
    }

    private void ShowPlayerText(string log, bool wasNPC = true)
    {
        bool isFirst = wasNPC;

        wasChatNpc = false;

        BubbleText prefab = isFirst ? PlayerFirstText : PlayerText;

        BubbleText text = Instantiate(prefab, _contaner);
        _allDialogueUI.Add(text.gameObject);

        text.InitBubble(log, 1f);

        AddHistory(SpeakerType.PLAYER, log, isFirst);

        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void ShowBubbleDelay()
    {
        if(nowBubble != null)
            nowBubble.SetActive(false);
        GameObject loading = Instantiate(NPCChatting, _contaner);
        _allDialogueUI.Add(loading.gameObject);
        StartCoroutine(DelayChat(loading));
    }

    private IEnumerator DelayChat(GameObject loadingObject)
    {
        yield return new WaitForSeconds(1f);
        if (loadingObject != null)
        {
            _allDialogueUI.Remove(loadingObject);
            Destroy(loadingObject);
        }
        if(nowBubble != null)
            nowBubble.SetActive(true);
        StartCoroutine(ScrollToBottom());
    }

    private void FindChoice(int seqNum, ChoiceBubble choice)
    {
        if(!_dialogueDataCore.GetAllDialogueEntry(_currentGuestSheetName, out var allData))
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
        _gameStatueCore.ChangeSincerityAmount(
            _currentGuestSheetName,
            choiceData[num].ChoiceSincerity
        );

        string resultText = choiceData[num].ChoiceText;

        ShowPlayerText(resultText, wasChatNpc);

        _currentKey = choiceData[num].NextKey;

        _allDialogueUI.Remove(target);
        Destroy(target);

        _currentChoiceUI = null;
        _isChoiceActive = false;

        StartCoroutine(NextStepAfterChoice());
    }
    
    private IEnumerator NextStepAfterChoice()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnMessage();
    }

    public void ChangeGuestDialogue(Guest guest)
    {
        
        if (_currentGuest == guest)
            return;

        if (!_dialogueDataCore.GetSheetNameByGuest(guest, out var sheetName))
            return;
        
        if (interactDelayRoutine != null)
        {
            StopCoroutine(interactDelayRoutine);
            interactDelayRoutine = null;
        }
        
        if (_currentGuest != Guest.None)
        {
            _savedDialogue[_currentGuest] = new SavedDialogueData
            {
                LastDialogueKey = _currentKey,
                WasChatNpc = wasChatNpc,
                History = _savedDialogue.ContainsKey(_currentGuest)
                    ? _savedDialogue[_currentGuest].History
                    : new List<DialogueHistoryData>(),
                
                HasActiveChoice = _isChoiceActive,
                ChoiceSeq = _currentChoiceSeq
            };
        }
        
        foreach (var ui in _allDialogueUI)
        {
            if (ui != null)
                Destroy(ui);
        }
        _allDialogueUI.Clear();

        _currentGuest = guest;
        _currentGuestSheetName = sheetName;

        SavedDialogueData saveData = default;
        bool hasSave = _savedDialogue.TryGetValue(_currentGuest, out saveData);
        
        if (hasSave && !string.IsNullOrEmpty(saveData.LastDialogueKey))
        {
            _currentKey = saveData.LastDialogueKey;
            wasChatNpc = saveData.WasChatNpc;
        }
        else
        {
            if (_dialogueDataCore.GetFirstDialogueByDay(
                    _currentGuestSheetName,
                    _gameStatueCore.CurrentDay,
                    out var data))
            {
                _currentKey = data.key;
                wasChatNpc = false;
            }
        }

        wasEndChat = false;
        
        if (hasSave && saveData.History != null)
        {
            RebuildHistory(saveData);
        }

        StartCoroutine(ScrollToBottom());
    }
    
    private void AddHistory(SpeakerType speaker, string content, bool isFirst)
    {
        if (!_savedDialogue.ContainsKey(_currentGuest))
        {
            _savedDialogue[_currentGuest] = new SavedDialogueData
            {
                LastDialogueKey = _currentKey,
                WasChatNpc = wasChatNpc,
                History = new List<DialogueHistoryData>()
            };
        }

        var data = _savedDialogue[_currentGuest];

        data.History ??= new List<DialogueHistoryData>();

        data.History.Add(new DialogueHistoryData
        {
            Speaker = speaker,
            Content = content,
            IsFirst = isFirst,
            IsChoice = false
        });

        _savedDialogue[_currentGuest] = data;
    }
    
    private void RebuildHistory(SavedDialogueData data)
    {
        if (data.History == null)
            return;

        foreach (var h in data.History)
        {
            if (h.Speaker == SpeakerType.NPC)
            {
                BubbleText prefab = h.IsFirst ? NPCFirstText : NPCText;

                BubbleText text = Instantiate(prefab, _contaner);
                text.InitBubble(h.Content, 1f);
                _allDialogueUI.Add(text.gameObject);
            }
            else
            {
                BubbleText prefab = h.IsFirst ? PlayerFirstText : PlayerText;

                BubbleText text = Instantiate(prefab, _contaner);
                text.InitBubble(h.Content, 1f);
                _allDialogueUI.Add(text.gameObject);
            }
        }
        
        if (data.HasActiveChoice)
        {
            StartCoroutine(DelayRestoreChoice(data.ChoiceSeq));
        }
    }
    
    private IEnumerator DelayRestoreChoice(int seq)
    {
        yield return null;

        ChoiceBubble choice = Instantiate(PlayerChoice, _contaner);
        _allDialogueUI.Add(choice.gameObject);

        FindChoice(seq, choice);
        choice.AddEvent(Choose);

        _currentChoiceUI = choice;
        _isChoiceActive = true;
        
        StartCoroutine(RefreshLayout(_contaner));
        StartCoroutine(ScrollToBottom());
    }

    public void EnableInteract()
    {
        CanInteract = true;
    }

    public void DisableInteract()
    {
        CanInteract = false;
    }
    
    IEnumerator RefreshLayout(RectTransform rect)
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }


    private void OnDestroy()
    {
        if(closeCafeBtn != null)
            onEndChat -= closeCafeBtn.EnableInteractable;
    }
}

public struct SavedDialogueData
{
    public string LastDialogueKey;
    public bool WasChatNpc;
    public List<DialogueHistoryData> History;
    
    public bool HasActiveChoice;
    public int ChoiceSeq;
}

public struct DialogueHistoryData
{
    public SpeakerType Speaker;
    public string Content;

    public bool IsFirst;
    public bool IsChoice;
            
    public bool IsChoiceGroup;
    public int ChoiceSeq;
}

public struct ChoiceBubbleData
{
    public string ChoiceText;
    public string NextKey;
    public int ChoiceSincerity;
}

public enum DialogueEventType
{
    NPC,
    PLAYER,
    CHOICE
}
