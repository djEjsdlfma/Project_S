using System.Collections;
using TMPro;
using UnityEngine;

namespace Moon._01.Script.Gimmick
{
    public class MemoNameChanger : MonoBehaviour
    {
        public TextMeshPro memoText;

        private static readonly string[] Names = { "리철민", "린쩌하오", "알렉세이 박", "박율" };
        private static readonly string[] DevNames = { "RiCheolMin", "LinZehao", "AlexeiPark", "ParkYul" };

        [SerializeField] private ChangedCamObject camObj;

        [Range(0.1f, 10f), SerializeField]
        private float changeInterval = 3.0f;

        [Range(0.1f, 3f), SerializeField]
        private float textTransitionDuration = 1.0f;

        [SerializeField] private bool useConfusionGlitch = true;
        [SerializeField] private int glitchNameCount = 3;
        [SerializeField] private float glitchTime = 0.06f;
        [Range(0, 1), SerializeField] private float glitchAlpha = 0.85f;

        private int _currentIndex = 0;

        private void Start()
        {
            if (memoText != null && Names.Length > 0)
            {
                memoText.text = Names[_currentIndex];
                camObj.ChangeName(DevNames[_currentIndex]);
                StartCoroutine(NameCycleRoutine());
            }
        }

        private IEnumerator NameCycleRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(changeInterval);

                int prevIndex = _currentIndex;
                _currentIndex = (_currentIndex + 1) % Names.Length;

                yield return StartCoroutine(AnimateTextChange(prevIndex, _currentIndex));
            }
        }

        private IEnumerator AnimateTextChange(int prevIndex, int targetIndex)
        {
            var glitchTimer = new WaitForSeconds(glitchTime);
            
            float halfDuration = textTransitionDuration / 2f;
            Color originalColor = memoText.color;

            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;

                memoText.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
                yield return null;
            }

            if (useConfusionGlitch)
            {
                int lastGlitchIndex = prevIndex;

                for (int i = 0; i < glitchNameCount; i++)
                {
                    int rand;
                    do
                    {
                        rand = Random.Range(0, Names.Length);
                    } while (rand == lastGlitchIndex);

                    lastGlitchIndex = rand;

                    memoText.text = Names[rand];
                    camObj.ChangeName(DevNames[rand]);
                    memoText.color = new Color(originalColor.r, originalColor.g, originalColor.b, glitchAlpha);
                    yield return glitchTimer;
                }
            }

            memoText.text = Names[targetIndex];
            camObj.ChangeName(DevNames[targetIndex]);

            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;

                memoText.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(0f, 1f, t));
                yield return null;
            }

            memoText.color = originalColor;
        }
    }
}