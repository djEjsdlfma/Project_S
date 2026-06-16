using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BubbleText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private RectTransform _Boundery;
    [SerializeField] private RectTransform myChatBound;

    [SerializeField] private SerializedDictionary<string, GameObject> ProfilPictures;

    
    public float maxWidth = 600f;
    
    private float prevHeight;
    private float originHeight;

    public string _myLog { get; set; }

    public float TextSpeed { get; set; }

    private void Awake()
    {
        originHeight = _Boundery.sizeDelta.y;
        InitScale();
    }

    private void Update()
    {
        if (prevHeight < myChatBound.sizeDelta.y)
        {
            prevHeight = myChatBound.sizeDelta.y;
            InitScale();
        }
    }

    public void InitBubble(string log, float speed, string name = null)
    {
        _myLog = log;
        TextSpeed = speed;

        if (name != null)
            _name.text = name;

        SetChatText(log);
    }

    public void SetProfil(string charName)
    {
        foreach(GameObject picture in ProfilPictures.Values)
        {
            picture.SetActive(false);
        }

        ProfilPictures[charName].SetActive(true);
    }
    
    public void SetChatText(string newText)
    {
        // 1. 텍스트 할당 (리터럴 \n을 실제 개행 문자로 변환)
        if (!string.IsNullOrEmpty(newText))
        {
            newText = newText.Replace("\\n", "\n");
        }
        
        // 2. [추가] 자동으로 \n을 삽입하여 개행 강제하기 (띄어쓰기 우선 개행)
        _text.text = "";
        string processedText = "";
        string currentLine = "";
        int lastSpaceIndexInLine = -1;
        
        for (int i = 0; i < newText.Length; i++)
        {
            char c = newText[i];
            
            if (c == '\n')
            {
                processedText += currentLine + "\n";
                currentLine = "";
                lastSpaceIndexInLine = -1;
                continue;
            }

            if (c == ' ')
            {
                lastSpaceIndexInLine = currentLine.Length;
            }

            _text.text = currentLine + c;
            if (_text.preferredWidth > maxWidth)
            {
                if (lastSpaceIndexInLine != -1)
                {
                    // 띄어쓰기가 있다면 그 위치에서 개행
                    string beforeSpace = currentLine.Substring(0, lastSpaceIndexInLine);
                    string afterSpace = currentLine.Substring(lastSpaceIndexInLine + 1);
                    processedText += beforeSpace + "\n";
                    currentLine = afterSpace + c;
                }
                else
                {
                    // 띄어쓰기가 없다면 현재 글자에서 개행
                    processedText += currentLine + "\n";
                    currentLine = c.ToString();
                }
                lastSpaceIndexInLine = currentLine.IndexOf(' ');
            }
            else
            {
                currentLine += c;
            }
        }
        processedText += currentLine;
        _text.text = processedText;

        // 3. 레이아웃 요소가 있다면 이를 활용하여 너비 제한
        LayoutElement layoutElement = GetComponent<LayoutElement>();
        if (layoutElement == null) layoutElement = myChatBound.GetComponent<LayoutElement>();

        // 4. 최종 텍스트 기준으로 너비와 높이 재계산
        _text.rectTransform.sizeDelta = new Vector2(maxWidth, _text.rectTransform.sizeDelta.y);
        _text.ForceMeshUpdate();

        float prefWidth = _text.preferredWidth;
        float finalWidth = Mathf.Min(prefWidth, maxWidth);

        // 5. 말풍선 및 텍스트 너비 적용
        if (layoutElement != null)
        {
            layoutElement.preferredWidth = finalWidth;
        }
        
        myChatBound.sizeDelta = new Vector2(finalWidth, myChatBound.sizeDelta.y);
        _text.rectTransform.sizeDelta = new Vector2(finalWidth, _text.preferredHeight); // 높이도 preferredHeight로 설정
        
        // 6. 너비 변경 후 다시 업데이트하여 높이가 정확히 계산되도록 함
        _text.ForceMeshUpdate();
        
        // 레이아웃 즉시 갱신 (부모가 LayoutGroup인 경우 대비)
        LayoutRebuilder.ForceRebuildLayoutImmediate(myChatBound);
        if (myChatBound.parent is RectTransform parentRect)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
        }
    }

    public void InitScale()
    {
        if(myChatBound.sizeDelta.y > 45)
        {
            _Boundery.sizeDelta = new Vector2(_Boundery.sizeDelta.x, originHeight + (23 * (myChatBound.sizeDelta.y / 40)));
        }
    }


    public void DoTextWithTMP(TextMeshProUGUI tmp, float duration)
    {
        tmp.maxVisibleCharacters = 0;
        DOTween.To(x => tmp.maxVisibleCharacters = (int)x, 0f, tmp.text.Length, duration);
    }
}
