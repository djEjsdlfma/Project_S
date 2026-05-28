using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.So;
using LSW._02._Code.System___Manager;
using LSW._02._Code.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BubbleManager : MonoBehaviour, ITabletUI, ISystemManager
{
    //ó??
    [SerializeField] private BubbleText NPCFirstText;
    [SerializeField] private BubbleText PlayerFirstText;

    //???
    [SerializeField] private BubbleText NPCText;
    [SerializeField] private BubbleText PlayerText;
    
    [SerializeField] private ChoiceBubble PlayerChoice;
    [SerializeField] private GameObject SelectionTrm;

    //???? ??? ??
    [SerializeField] private Chatting NPCChatting;
    [SerializeField] private GameObject Empty;

    [SerializeField] private RectTransform _container;
    [SerializeField] private ScrollRect scrollRect;
    [field:SerializeField] public ChatProfileContainer ChatProfileContainer { get; private set; }
    
    public event Action onSpawnMessage;
    public event Action onEndChat;
    public event Action<Guest, bool> onAlarmStateChanged;
    
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
    
    private GameObject _bottomEmptySpace;
    private Chatting _currentLoading;
    private bool wasEndChat = false;

    private string tempTextData;
    private bool tempAlarmValue;

    public bool CanInteract { get; private set; }
    
    public void Initialize(SystemManager _)
    {
        _interactDelayCoroutine = new WaitForSecondsRealtime(0.2f);
        
        _dialogueDataCore = CoreHandler.Instance.GetCore<DialogueDataCore>();
        
        _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();

        if (_savedDialogue == null)
            _savedDialogue = new Dictionary<Guest, SavedDialogueData>();
        
        DisableInteract();
    }

    private void Start()
    {
        if (ChatProfileContainer != null)
        {
            ChatProfileContainer.InitializeProfiles(this);
        }
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

    public void SpawnMessage(bool isImmediate = false)
    {
        if (!CanInteract || wasEndChat || _isChoiceActive) 
            return;

        if (string.IsNullOrEmpty(_currentGuestSheetName) || _currentGuest == Guest.None)
            return;

        if (!_dialogueDataCore.GetDialogueDataByKey(_currentGuestSheetName, _currentKey, out DialogueEntry data))
        {
            return;
        }

        DisableInteract();

        if (data.sincerity != 0)
            _gameStatueCore.ChangeSincerityAmount(_currentGuestSheetName, data.sincerity);
        
        bool isEnding = (data.nextKey == "END");

        if (data.type == DialogueType.Select)
        {
            _isChoiceActive = true;
            _currentChoiceSeq = data.seq;

            FindChoice(data.seq);
            PlayerChoice.AddEvent(Choose);
        }
        else if (data.speaker == SpeakerType.NPC)
        {
            ShowNPCText(data.content, wasChatNpc, _currentGuestSheetName, isEnding, isImmediate);
            _currentKey = data.nextKey;
            
            if (ChatProfileContainer != null)
                ChatProfileContainer.SetCurrentProfile(data.content, !isImmediate && !isEnding);


            if (!isImmediate && !isEnding)
                onAlarmStateChanged?.Invoke(_currentGuest, true);
        }
        else
        {
            ShowPlayerText(data.content, wasChatNpc);
            _currentKey = data.nextKey;

            if (ChatProfileContainer != null)
                ChatProfileContainer.SetCurrentProfile(data.content, false);

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


    private void ShowNPCText(string log, bool wasNPC, string speakerName, bool isEnding, bool isImmediate = false)
    {
        Guest recordingGuest = _currentGuest;
        bool isFirst = !wasNPC;
        
        if (isFirst && _allDialogueUI.Count > 0)
        {
            ShowEmptySpace();
        }

        BubbleText prefab = isFirst ? NPCFirstText : NPCText;
        BubbleText text = Instantiate(prefab, _container);
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
        
        if (isImmediate)
        {
            nowBubble.SetActive(true);
            if (isEnding)
            {
                HandleEndChat();
            }
            else
            {
                EnableInteract();
            }
        }
        else
        {
            ShowBubbleDelay(nowBubble, speakerName, isEnding);
        }
        onSpawnMessage?.Invoke();
    }

    private void ShowPlayerText(string log, bool wasNPC = true)
    {
        Guest recordingGuest = _currentGuest; // 현재 시점의 게스트 기록
        bool isFirst = wasNPC;

        if (isFirst && _allDialogueUI.Count > 0)
        {
            ShowEmptySpace();
        }
        
        wasChatNpc = false;

        BubbleText prefab = isFirst ? PlayerFirstText : PlayerText;

        BubbleText text = Instantiate(prefab, _container);
        _allDialogueUI.Add(text.gameObject);

        text.InitBubble(log, 1f);
        text.InitScale();

        AddHistory(recordingGuest, SpeakerType.PLAYER, log, isFirst);
        
        UpdateBottomEmptySpace();
        
        onSpawnMessage?.Invoke();
    }
    
    private void ShowEmptySpace()
    {
        if (Empty != null)
        {
            GameObject emptyObj = Instantiate(Empty, _container);
            _allDialogueUI.Add(emptyObj);
        }
    }
    
    private void UpdateBottomEmptySpace()
    {
        if (Empty == null) 
            return;
        
        if (_bottomEmptySpace != null)
        {
            _allDialogueUI.Remove(_bottomEmptySpace);
            Destroy(_bottomEmptySpace);
            _bottomEmptySpace = null;
        }

        _bottomEmptySpace = Instantiate(Empty, _container);
        _allDialogueUI.Add(_bottomEmptySpace);
    }

    public void ShowBubbleDelay(GameObject targetBubble, string chatterName, bool isEnding)
    {
        if(targetBubble != null)
            targetBubble.SetActive(false);
    
        if (_currentLoading != null)
        {
            _allDialogueUI.Remove(_currentLoading.gameObject);
            Destroy(_currentLoading.gameObject);
            _currentLoading = null;
        }

        _currentLoading = Instantiate(NPCChatting, _container);
        _currentLoading.SetName(chatterName);
        _allDialogueUI.Add(_currentLoading.gameObject);
        
        UpdateBottomEmptySpace();

        StartCoroutine(DelayChat(_currentLoading, targetBubble, isEnding));
    }
    
    private IEnumerator DelayChat(Chatting loadingObject, GameObject targetBubble, bool isEnding)
    {
        yield return new WaitForSeconds(0.5f);

        if (loadingObject != null)
        {
            _allDialogueUI.Remove(loadingObject.gameObject);
            Destroy(loadingObject.gameObject);
            if (_currentLoading == loadingObject)
                _currentLoading = null;
        }

        if(targetBubble != null && targetBubble.gameObject != null)
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
        if (_savedDialogue.ContainsKey(_currentGuest))
        {
            var data = _savedDialogue[_currentGuest];
            data.IsCompleted = true;
            _savedDialogue[_currentGuest] = data;
        }

        wasEndChat = true;
        DisableInteract(); // 확실하게 잠금
        onEndChat?.Invoke();
        onAlarmStateChanged?.Invoke(_currentGuest, false);
    }

    private void FindChoice(int seqNum)
    {
        if(!_dialogueDataCore.GetAllDialogueEntry(_currentGuestSheetName, out var allData))
            return;
    
        // 🛠️ 추가: 현재 인게임 진짜 날짜를 기획 시트용 순번(1, 2, 3...)으로 변환
        int currentDay = _gameStatueCore.CurrentDay;
        int order = ((currentDay - 1) / 5) + 1;
    
        // 🛠️ 수정: x.day 조건을 진짜 날짜(currentDay)가 아니라 변환된 순번(order)으로 검사합니다.
        var choices = allData.Values
            .Where(x => x.day == order && x.seq == seqNum && x.type == DialogueType.Select)
            .OrderBy(x => x.id)
            .ToList();

        // 안전장치: 시트 데이터가 꼬여서 선택지를 못 찾아왔을 때 에러 방지
        if (choices.Count == 0)
        {
            Debug.LogError($"[Choice Error] {currentDay}일차(시트순번:{order}), Seq {seqNum}번에 해당하는 선택지 데이터를 시트에서 찾을 수 없습니다!");
            SelectionTrm.SetActive(false);
            _isChoiceActive = false;
            EnableInteract();
            return;
        }

        SelectionTrm.SetActive(true);
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
    
        PlayerChoice.ChoiceInit(choices.Select(x => x.content).ToArray());
    }

    private void Choose(GameObject target, int num)
    {
        if (target == null) 
            return;
        
        if (num < 0 || num >= _currentChoiceData.Count) 
            return;

        var selectedChoice = _currentChoiceData[num];
        _gameStatueCore.ChangeSincerityAmount(_currentGuestSheetName, selectedChoice.ChoiceSincerity);
    
        PlayerChoice.Hide();
        ShowPlayerText(selectedChoice.ChoiceText, wasChatNpc);

        _currentKey = selectedChoice.NextKey;
        _isChoiceActive = false;
        
        SelectionTrm.SetActive(false); 

        _currentChoiceData.Clear();
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
        if (_currentGuest == guest && !string.IsNullOrEmpty(_currentKey)) return;

        // 1. 현재 게스트 데이터 저장
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

        if (guest == Guest.None)
        {
            _currentGuest = Guest.None;
            _currentGuestSheetName = string.Empty;
            _currentKey = string.Empty;
            return;
        }

        if (!_dialogueDataCore.GetSheetNameByGuest(guest, out var sheetName)) return;

        // 2. UI 및 환경 초기화
        _isChoiceActive = false;
        _currentChoiceData.Clear();

        if (_currentLoading != null)
        {
            _allDialogueUI.Remove(_currentLoading.gameObject);
            Destroy(_currentLoading.gameObject);
            _currentLoading = null;
        }

        StopAllCoroutines();

        if (nowBubble != null && nowBubble.gameObject != null && !nowBubble.activeSelf)
        {
            nowBubble.SetActive(true);
        }

        DisableInteract();

        if (SelectionTrm != null)
        {
            SelectionTrm.SetActive(false);
        }

        foreach (var ui in _allDialogueUI)
        {
            if (ui != null) Destroy(ui);
        }

        _allDialogueUI.Clear();
        _bottomEmptySpace = null;

        _currentGuest = guest;
        _currentGuestSheetName = sheetName;
        wasEndChat = false;

        // 3. 대화 키 설정
        SavedDialogueData saveData = default;
        bool hasSave = _savedDialogue.TryGetValue(_currentGuest, out saveData);
        int currentDay = _gameStatueCore.CurrentDay;

        bool isDialogueDay = false;
        switch (guest)
        {
            case Guest.JaeYoonLee: isDialogueDay = (currentDay % 5 == 1); break;
            case Guest.DaEunJung: isDialogueDay = (currentDay % 5 == 2); break;
            case Guest.YulPark: isDialogueDay = (currentDay % 5 == 3); break;
            case Guest.SeoAhYoon: isDialogueDay = (currentDay % 5 == 4); break;
            case Guest.MyeongJinChoi: isDialogueDay = (currentDay % 5 == 0); break;
        }

        if (hasSave && !string.IsNullOrEmpty(saveData.LastDialogueKey))
        {
            _currentKey = saveData.LastDialogueKey;
            wasChatNpc = saveData.WasChatNpc;
        }
        else if (isDialogueDay)
        {
            int order = ((currentDay - 1) / 5) + 1;
            if (_dialogueDataCore.GetFirstDialogueByDay(_currentGuestSheetName, order, out var data))
            {
                _currentKey = data.key;
                wasChatNpc = false;
            }
            else
            {
                Debug.LogError($"[{currentDay}일차] {guest}의 시트 데이터 없음!");
                _currentKey = string.Empty;
                wasChatNpc = false;
                wasEndChat = true;
                return;
            }
        }
        else
        {
            _currentKey = string.Empty;
            wasChatNpc = false;
            wasEndChat = true;
            return;
        }

        // 4. 히스토리 복구 및 UI 업데이트
        if (hasSave && saveData.History != null)
        {
            RebuildHistory(saveData, isDialogueDay, isDialogueDay);
        }

        StartCoroutine(UpdateUILayout());
        EnableInteract();

        SpawnFirstDialogue();

        if (ChatProfileContainer != null)
        {
            onAlarmStateChanged?.Invoke(_currentGuest, false);
        }
    }
    
    public void SpawnFirstDialogue()
    {
        if (_currentGuest == Guest.None || wasEndChat) 
            return;

        while (true)
        {
            if (!_dialogueDataCore.GetDialogueDataByKey(_currentGuestSheetName, _currentKey, out var data))
                break;

            if (data.speaker != SpeakerType.NPC || data.type == DialogueType.Select)
                break;

            SpawnMessage(true);

            if (data.nextKey == "END" || string.IsNullOrEmpty(data.nextKey))
                break;

            _currentKey = data.nextKey;
        }
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
    
    private void RebuildHistory(SavedDialogueData data, bool isDialogueDay, bool updateProfile = true)
    {
        if (data.History == null) return;

        string lastContent = string.Empty;

        foreach (var h in data.History)
        {
            if (h.IsFirst && _allDialogueUI.Count > 0)
            {
                ShowEmptySpace();
            }

            BubbleText prefab = null;
            if (h.Speaker == SpeakerType.NPC)
                prefab = h.IsFirst ? NPCFirstText : NPCText;
            else
                prefab = h.IsFirst ? PlayerFirstText : PlayerText;

            if (prefab != null)
            {
                BubbleText text = Instantiate(prefab, _container);
                text.InitBubble(h.Content, 1f);
                _allDialogueUI.Add(text.gameObject);
                lastContent = h.Content;
            }
        }
        
        if (updateProfile && ChatProfileContainer != null && !string.IsNullOrEmpty(lastContent))
            ChatProfileContainer.SetCurrentProfile(lastContent, !wasEndChat && isDialogueDay);
        
        UpdateBottomEmptySpace();
    
        if (data.HasActiveChoice)
        {
            _isChoiceActive = true; 
            StartCoroutine(DelayRestoreChoice(data.ChoiceSeq));
        }
    }
    
    private IEnumerator DelayRestoreChoice(int seq)
    {
        yield return new WaitForEndOfFrame();
        
        SelectionTrm.SetActive(true);
        FindChoice(seq);
    
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
        yield return null; 
        LayoutRebuilder.ForceRebuildLayoutImmediate(_container);
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void ResetData()
    {
        _savedDialogue.Clear();
        _currentKey = string.Empty;
        _currentGuest = Guest.None;
        _currentGuestSheetName = string.Empty;
        wasChatNpc = false;
        wasEndChat = false;
        _isChoiceActive = false;
        _currentChoiceSeq = 0;
    }
    
    public void Reset()
    { 
        ResetData();
    }
    public void SetCotainer(RectTransform _rect) { _container = _rect; }

    public void SetScrolllRect(ScrollRect _Srect) { scrollRect = _Srect; }

    public string GetLastDialogueContent(Guest guest)
    {
        if (!_dialogueDataCore.GetSheetNameByGuest(guest, out var sheetName))
            return string.Empty;

        int currentDay = _gameStatueCore.CurrentDay;
    
        bool isDialogueDay = false;
        switch (guest)
        {
            case Guest.JaeYoonLee:     isDialogueDay = (currentDay % 5 == 1); break;
            case Guest.DaEunJung:      isDialogueDay = (currentDay % 5 == 2); break;
            case Guest.YulPark:        isDialogueDay = (currentDay % 5 == 3); break;
            case Guest.SeoAhYoon:      isDialogueDay = (currentDay % 5 == 4); break;
            case Guest.MyeongJinChoi:  isDialogueDay = (currentDay % 5 == 0); break;
        }

        if (!isDialogueDay)
            return string.Empty;

        if (_savedDialogue.TryGetValue(guest, out var data))
        {
            if (data.History != null && data.History.Count > 0)
            {
                return data.History.Last().Content;
            }
        }

        // 🛠️ 수식 단일화 교정 완료
        int order = ((currentDay - 1) / 5) + 1;
        if (_dialogueDataCore.GetFirstDialogueByDay(sheetName, order, out var firstData))
        {
            return firstData.content;
        }

        return string.Empty;
    }

    public bool IsDialogueUnread(Guest guest)
    {
        if (!_dialogueDataCore.GetSheetNameByGuest(guest, out _))
            return false;

        int currentDay = _gameStatueCore.CurrentDay;
    
        bool isDialogueDay = false;
        switch (guest)
        {
            case Guest.JaeYoonLee:     isDialogueDay = (currentDay % 5 == 1); break;
            case Guest.DaEunJung:      isDialogueDay = (currentDay % 5 == 2); break;
            case Guest.YulPark:        isDialogueDay = (currentDay % 5 == 3); break;
            case Guest.SeoAhYoon:      isDialogueDay = (currentDay % 5 == 4); break;
            case Guest.MyeongJinChoi:  isDialogueDay = (currentDay % 5 == 0); break;
        }

        if (!isDialogueDay)
            return false;

        if (_savedDialogue.TryGetValue(guest, out var data))
        {
            return !data.IsCompleted;
        }

        return false;
    }
}

public struct SavedDialogueData
{
    public string LastDialogueKey;
    public bool WasChatNpc;
    public List<DialogueHistoryData> History;
    
    public bool HasActiveChoice;
    public int ChoiceSeq;
    public bool IsCompleted;
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
