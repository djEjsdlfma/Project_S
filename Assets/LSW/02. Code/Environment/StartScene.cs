
using System;
using System.Threading.Tasks;
using LSW._02._Code.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.Importer;
using UnityEngine.UI;

namespace LSW._02._Code.Environment
{
    public class StartScene : MonoBehaviour
    {
        [SerializeField] private DialogueSheetImporter dialogueSheetImporter;
        [SerializeField] private float timeToLoad = 5f;
        [SerializeField] private Image fadeImg;

        private DialogueDataCore _dialogueDataCore;
        
        private async void Start()
        {
            try
            {
                _dialogueDataCore = CoreHandler.Instance.GetCore<DialogueDataCore>();

                var minimumWaitTask = Task.Delay(TimeSpan.FromSeconds(timeToLoad));
                var importTask = dialogueSheetImporter.ImportDialogueSheets();

                var database = await importTask;
                if (database == null || database.sheets.Count == 0)
                {
                    Debug.LogError("종료 : 데이터 임포트 실패");
                    return; 
                }
                
                _dialogueDataCore.SetDatabase(database);
                
                float remainingTime = timeToLoad - 1.5f;
                if (remainingTime > 0)
                {
                    await fadeImg.DOFade(1f, remainingTime).AsyncWaitForCompletion();
                }
                
                await minimumWaitTask;

                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((int)SceneType.MainTabletScene);
                while (asyncLoad is { isDone: false })
                {
                    await Task.Yield();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"씬 시작 에러: {e}");
            }
        }
    }
}