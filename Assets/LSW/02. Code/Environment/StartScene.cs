
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

        private IEnumerator Start()
        {
            FadeImg.DOFade(1f, timeToLoad);
            yield return new WaitForSeconds(timeToLoad);
            SceneManager.LoadScene((int)SceneType.MainTabletScene);
        }
    }
}