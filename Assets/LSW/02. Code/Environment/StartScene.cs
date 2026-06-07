
using System.Collections;
using LSW._02._Code.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LSW._02._Code.Environment
{
    public class StartScene : MonoBehaviour
    {
        [SerializeField] private float timeToLoad = 5f;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(timeToLoad);
            SceneManager.LoadScene((int)SceneType.MainTabletScene);
        }
    }
}