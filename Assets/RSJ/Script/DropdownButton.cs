using TMPro;
using UnityEngine;

public class DropdownButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _btnText;

    public string SetAndGiveText(string text)
    {
        string temp = _btnText.text;
        _btnText.text = text;

        return temp;
    }

}
