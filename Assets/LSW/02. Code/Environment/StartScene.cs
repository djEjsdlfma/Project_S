
using System.Collections;
using LSW._02._Code.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

namespace LSW._02._Code.Environment
{
    public class StartScene : MonoBehaviour
    {
        [SerializeField] private float timeToLoad = 5f;
        [SerializeField] private Image FadeImg;
        [SerializeField] private Image FadeTitle;

        private IEnumerator Start()
        {
            FadeTitle.DOFade(0.3f, 1.5f).OnComplete(() =>
            {
                FadeImg.DOFade(1f, (timeToLoad - 1.5f));
            });
            yield return new WaitForSeconds(timeToLoad);
            SceneManager.LoadScene((int)SceneType.MainTabletScene);
        }
    }
}