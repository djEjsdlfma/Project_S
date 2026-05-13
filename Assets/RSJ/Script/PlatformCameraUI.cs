using Moon._01.Script.Cameras;
using TMPro;
using UnityEngine;

public class PlatformCameraUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _leftTryCapture;
    [SerializeField] private TextMeshProUGUI _leftTryCopy;

    [SerializeField] private PhotoStorage _photoStorage;

    private void Awake()
    {
        _leftTryCapture.text = "³²Àº ÃÔ¿µ È½¼ö: " + (_photoStorage.MaxPhoto - _photoStorage.PhotoMany);
        _leftTryCopy.text = "³²Àº º¹»ç È½¼ö: "  + 99;
    }
}
