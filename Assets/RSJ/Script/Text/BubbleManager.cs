using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BubbleManager : MonoBehaviour
{
    //처음
    [SerializeField] private BubbleText NPCFirstText;
    [SerializeField] private BubbleText PlayerFirstText;

    //이어감
    [SerializeField] private BubbleText NPCText;
    [SerializeField] private BubbleText PlayerText;
    
    [SerializeField] private ChoiceBubble PlayerChoice;

    //텍스트 치는 중
    [SerializeField] private GameObject NPCChatting;

    [SerializeField] private ScriptSO nowScript;

    [SerializeField] private Transform _contaner;
    [SerializeField] private ScrollRect scrollRect;

    private int scriptNum;
    private bool wasChatNpc;
    private GameObject nowBubble;
    private int ReactNum1;
    private int ReactNum2;

    private void Update()
    {
        if(Keyboard.current.dKey.wasPressedThisFrame)
        {
            SpawnMessage();
        }
    }

    public void SpawnMessage()
    {
        if(nowScript.scripts[scriptNum].isNPC)
        {
            int eventNum = nowScript.scripts[scriptNum].ChoiceNum;

            if(nowScript.scripts[scriptNum]._event == ScriptEvent.Normal)
            {
                ShowNPCText(nowScript.scripts[scriptNum].text, wasChatNpc);
            }
            else if (nowScript.scripts[scriptNum]._event == ScriptEvent.Choice)
            {
                ChoiceBubble choice = Instantiate(PlayerChoice, _contaner);
                FindChoice(eventNum, choice);
                choice.AddEvent(ChoseOne, ChoseTwo);
            }
        }
        else
        {
            ShowPlayerText(nowScript.scripts[scriptNum].text, wasChatNpc);
        }

        scriptNum++;
        // 3. 스크롤 최하단 이동
        StartCoroutine(ScrollToBottom());
    }

    private void FindChoice(int eventNum, ChoiceBubble choice)
    {
        int index = -1;
        foreach (ChoiceScript item in nowScript.cScript)
        {
            index++;
            if (item.ChoiceId != eventNum)
                continue;

            ReactNum1 = nowScript.cScript[index].ReactNum1;
            ReactNum2 = nowScript.cScript[index].ReactNum2;
            choice.ChoiceInit(nowScript.cScript[index].choice1, nowScript.cScript[index].choice2);
        }
    }

    private void ShowNPCText(string log, bool wasNPC = false)
    {
        if (wasNPC == false)
        {
            wasChatNpc = true;
            BubbleText text = Instantiate(NPCFirstText, _contaner);
            text.InitBubble(log, 1f, nowScript.scripts[scriptNum]._name);
            nowBubble = text.gameObject;
        }
        else
        {
            BubbleText text = Instantiate(NPCText, _contaner);
            text.InitBubble(log, 1f);
            nowBubble = text.gameObject;
        }
        ShowBubbleDelay();
    }

    private void ShowPlayerText(string log, bool wasNPC = true)
    {
        if (wasNPC == true)
        {
            wasChatNpc = false;
            BubbleText text = Instantiate(PlayerFirstText, _contaner);
            text.InitBubble(log, 1f, nowScript.scripts[scriptNum]._name);
        }
        else
        {
            BubbleText text = Instantiate(PlayerText, _contaner);
            text.InitBubble(log, 1f);
        }
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void ShowBubbleDelay()
    {
        nowBubble.SetActive(false);
        Instantiate(NPCChatting, _contaner);
        StartCoroutine(DelayChat());
    }

    private IEnumerator DelayChat()
    {
        yield return new WaitForSeconds(1f);

        nowBubble.SetActive(true);
    }

    private IEnumerator PlayerDelay(bool isOne)
    {
        yield return new WaitForSeconds(0.5f);

        if(isOne)
        {
            ShowNPCText(nowScript.reactScript[ReactNum1].text);
        }
        else
        {
            ShowNPCText(nowScript.reactScript[ReactNum2].text);
        }
    }

    private void ChoseOne(GameObject target)
    {
        ShowPlayerText(nowScript.
            cScript[nowScript.scripts[scriptNum - 1].ChoiceNum].choice1);

        StartCoroutine(PlayerDelay(true));
        Destroy(target);
    }

    private void ChoseTwo(GameObject target)
    {
        ShowPlayerText(nowScript.
            cScript[nowScript.scripts[scriptNum - 1].ChoiceNum].choice2);

        StartCoroutine(PlayerDelay(false));
        Destroy(target);
    }
}
