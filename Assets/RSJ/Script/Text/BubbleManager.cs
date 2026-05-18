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
    [SerializeField] private GameObject ChoiceObject;

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

    private readonly List<ChoiceBubbleData> _currentChoiceData = new List<ChoiceBubbleData>();
    
    private List<GameObject> _allDialogueUI = new List<GameObject>();
    private Guest _currentGuest;
    private string _currentGuestSheetName;
    private Dictionary<Guest, SavedDialogueData> _savedDialogue = new Dictionary<Guest, SavedDialogueData>();
    private bool _isChoiceActive;
    private int _currentChoiceSeq;
    
    private bool wasEndChat = false;
    public bool CanInteract { get; private set; }
    
    private void Awake()
    {
        _interactDelayCoroutine = new WaitForSecondsRealtime(0.2f);
        
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
        EnableInteract();
    }

    public void SpawnMessage()
    {
        if (!CanInteract || wasEndChat || _isChoiceActive) 
            return;

        if (string.IsNullOrEmpty(_currentGuestSheetName) || _currentGuest == Guest.None)
            return;

        if (!_dialogueDataCore.GetDialogueDataByKey(_currentGuestSheetName, _currentKey, out DialogueEntry data))
            return;

        DisableInteract();

        if (data.sincerity != 0)
            _gameStatueCore.ChangeSincerityAmount(_currentGuestSheetName, data.sincerity);
        
        bool isEnding = (data.nextKey == "END");

        if (data.type == DialogueType.Select)
        {
            _isChoiceActive = true;
            _currentChoiceSeq = data.seq;
        
            ChoiceBubble choice = Instantiate(PlayerChoice, _contaner);
            _allDialogueUI.Add(choice.gameObject);
            FindChoice(data.seq, choice);
            choice.AddEvent(Choose);
        }
        else if (data.speaker == SpeakerType.NPC)
        {
            ShowNPCText(data.content, wasChatNpc, data.speaker.ToString(), isEnding);
            _currentKey = data.nextKey;
        }
        else
        {
            ShowPlayerText(data.content, wasChatNpc);
            _currentKey = data.nextKey;

            if (isEnding)
            {
                HandleEndChat();
            }
            else
            {
                StartCoroutine(DelayInteract());
            }
        }

        StartCoroutine(UpdateUILayout());
    }

    private void ShowNPCText(string log, bool wasNPC, string speakerName, bool isEnding)
    {
        Guest recordingGuest = _currentGuest;
        bool isFirst = !wasNPC;

        BubbleText prefab = isFirst ? NPCFirstText : NPCText;
        BubbleText text = Instantiate(prefab, _contaner);
        _allDialogueUI.Add(text.gameObject);

        if (isFirst)
        {
            wasChatNpc = true;
            text.InitBubble(log, 1f, speakerName);
        }
        else
        {
            text.InitBubble(log, 1f);
        }

        nowBubble = text.gameObject;
        AddHistory(recordingGuest, SpeakerType.NPC, log, isFirst);
        
        ShowBubbleDelay(nowBubble, isEnding);
    }

    private void ShowPlayerText(string log, bool wasNPC = true)
    {
        Guest recordingGuest = _currentGuest; // 현재 시점의 게스트 기록
        bool isFirst = wasNPC;

        wasChatNpc = false;

        BubbleText prefab = isFirst ? PlayerFirstText : PlayerText;

        BubbleText text = Instantiate(prefab, _contaner);
        _allDialogueUI.Add(text.gameObject);

        text.InitBubble(log, 1f);
        text.InitScale();

        AddHistory(recordingGuest, SpeakerType.PLAYER, log, isFirst);
    }

    public void ShowBubbleDelay(GameObject targetBubble, bool isEnding)
    {
        if(targetBubble != null)
            targetBubble.SetActive(false);
    
        GameObject loading = Instantiate(NPCChatting, _contaner);
        _allDialogueUI.Add(loading.gameObject);

        StartCoroutine(DelayChat(loading, targetBubble, isEnding));
    }
    
    private IEnumerator DelayChat(GameObject loadingObject, GameObject targetBubble, bool isEnding)
    {
        yield return new WaitForSeconds(1f);

        if (loadingObject != null)
        {
            _allDialogueUI.Remove(loadingObject);
            Destroy(loadingObject);
        }

        if(targetBubble != null)
            targetBubble.SetActive(true);

        if (isEnding)
        {
            HandleEndChat();
        }
        else
        {
            StartCoroutine(DelayInteract());
        }
    
        StartCoroutine(UpdateUILayout());
    }
    
    private void HandleEndChat()
    {
        wasEndChat = true;
        DisableInteract(); // 확실하게 잠금
        onEndChat?.Invoke();
    }

    private void FindChoice(int seqNum, ChoiceBubble choice)
    {
        if(!_dialogueDataCore.GetAllDialogueEntry(_currentGuestSheetName, out var allData))
            return;

        ChoiceObject.SetActive(true);

        var choices = allData.Values
            .Where(x => x.seq == seqNum && x.type == DialogueType.Select)
            .OrderBy(x => x.id)
            .ToList();

        _currentChoiceData.Clear(); 

        foreach (var c in choices)
        {
            _currentChoiceData.Add(new ChoiceBubbleData
            {
                ChoiceText = c.content,
                NextKey = c.nextKey,
                ChoiceSincerity = c.sincerity
            });
        }
    
        choice.ChoiceInit(choices.Select(x => x.content).ToArray());
    }

    private void Choose(GameObject target, int num)
    {
        if (target == null) return;
        if (num < 0 || num >= _currentChoiceData.Count) return;

        var selectedChoice = _currentChoiceData[num];
        _gameStatueCore.ChangeSincerityAmount(_currentGuestSheetName, selectedChoice.ChoiceSincerity);
        
        ShowPlayerText(selectedChoice.ChoiceText, wasChatNpc);
    
        _currentKey = selectedChoice.NextKey;

        _allDialogueUI.Remove(target);
        Destroy(target);
        
        _isChoiceActive = false;
    
        // [수정 핵심] 사용이 끝난 선택지 데이터 초기화
        _currentChoiceData.Clear();
        ChoiceObject.SetActive(false);
        StartCoroutine(NextStepAfterChoice());
    }
    
    private IEnumerator NextStepAfterChoice()
    {
        yield return new WaitForSeconds(0.5f);
        EnableInteract(); 
        SpawnMessage();
    }

    public void ChangeGuestDialogue(Guest guest)
    {
        if (_currentGuest == guest) return;
        if (!_dialogueDataCore.GetSheetNameByGuest(guest, out var sheetName)) return;
        
        
        if (_currentGuest != Guest.None)
        {
            if (!_savedDialogue.ContainsKey(_currentGuest))
            {
                _savedDialogue[_currentGuest] = new SavedDialogueData { History = new List<DialogueHistoryData>() };
            }

            var currentData = _savedDialogue[_currentGuest];
            currentData.LastDialogueKey = _currentKey;
            currentData.WasChatNpc = wasChatNpc;
            currentData.HasActiveChoice = _isChoiceActive;
            currentData.ChoiceSeq = _currentChoiceSeq;
        
            _savedDialogue[_currentGuest] = currentData;
        }

        _isChoiceActive = false;
        _currentChoiceData.Clear(); 
        StopAllCoroutines();
        DisableInteract();
    
        foreach (var ui in _allDialogueUI)
        {
            if (ui != null) Destroy(ui);
        }
        _allDialogueUI.Clear();
        
        _currentChoiceData.Clear(); 

        _currentGuest = guest;
        _currentGuestSheetName = sheetName;
        wasEndChat = false; 

        SavedDialogueData saveData = default;
        bool hasSave = _savedDialogue.TryGetValue(_currentGuest, out saveData);
    
        if (hasSave && !string.IsNullOrEmpty(saveData.LastDialogueKey))
        {
            _currentKey = saveData.LastDialogueKey;
            wasChatNpc = saveData.WasChatNpc;
        }
        else
        {
            if (_dialogueDataCore.GetFirstDialogueByDay(_currentGuestSheetName, _gameStatueCore.CurrentDay, out var data))
            {
                _currentKey = data.key;
                wasChatNpc = false;
            }
        }

        if (hasSave && saveData.History != null)
        {
            RebuildHistory(saveData);
        }

        StartCoroutine(UpdateUILayout());
        EnableInteract();
    }
    
    private void AddHistory(Guest targetGuest, SpeakerType speaker, string content, bool isFirst)
    {
        if (targetGuest == Guest.None) return;

        if (!_savedDialogue.ContainsKey(targetGuest))
        {
            _savedDialogue[targetGuest] = new SavedDialogueData
            {
                History = new List<DialogueHistoryData>()
            };
        }

        var data = _savedDialogue[targetGuest];
        data.History ??= new List<DialogueHistoryData>();

        data.History.Add(new DialogueHistoryData
        {
            Speaker = speaker,
            Content = content,
            IsFirst = isFirst
        });

        _savedDialogue[targetGuest] = data;
    }
    
    private void RebuildHistory(SavedDialogueData data)
    {
        if (data.History == null) return;

        foreach (var h in data.History)
        {
            BubbleText prefab = null;
            if (h.Speaker == SpeakerType.NPC)
                prefab = h.IsFirst ? NPCFirstText : NPCText;
            else
                prefab = h.IsFirst ? PlayerFirstText : PlayerText;

            if (prefab != null)
            {
                BubbleText text = Instantiate(prefab, _contaner);
                text.InitBubble(h.Content, 1f);
                _allDialogueUI.Add(text.gameObject);
            }
        }
    
        if (data.HasActiveChoice)
        {
            _isChoiceActive = true; 
            StartCoroutine(DelayRestoreChoice(data.ChoiceSeq));
        }
    }
    
    private IEnumerator DelayRestoreChoice(int seq)
    {
        yield return new WaitForEndOfFrame();
        
        if (_contaner.GetComponentInChildren<ChoiceBubble>() != null) 
            yield break;

        ChoiceBubble choice = Instantiate(PlayerChoice, _contaner);
        _allDialogueUI.Add(choice.gameObject);

        FindChoice(seq, choice);
        choice.AddEvent(Choose);
        
        StartCoroutine(UpdateUILayout());
    }

    public void EnableInteract()
    {
        CanInteract = true;
    }

    public void DisableInteract()
    {
        CanInteract = false;
    }
    
    private IEnumerator UpdateUILayout()
    {
        yield return new WaitUntil(() => _contaner != null);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_contaner);
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
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
