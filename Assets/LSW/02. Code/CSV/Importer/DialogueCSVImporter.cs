using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using LSW._02._Code.Scriptable_Object;
using UnityEditor;
using UnityEngine;

namespace LSW._02._Code.CSV.Importer
{
    public class DialogueCSVImporter : MonoBehaviour
    {
        public List<TextAsset> dialogueCsvFile;
        public string savePath = "Assets/LSW/05. SO/Dialogue";
        
#if UNITY_EDITOR
        [ContextMenu("CSV Import")]
        public void ConvertToMultipleSOs()
        {
            if (dialogueCsvFile == null || dialogueCsvFile.Count <= 0)
                return;
            if (!Directory.Exists(savePath)) 
                Directory.CreateDirectory(savePath);
            
            for (int c = 0; c < dialogueCsvFile.Count; c++)
            {
                if(dialogueCsvFile[c] == null)
                    continue;
                string[] lines = dialogueCsvFile[c].text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                Regex csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                for (int i = 3; i < lines.Length; i++)
                {
                    string[] values = csvParser.Split(lines[i]);
                    if (values.Length < 14)
                        continue;

                    string dialogueName = dialogueCsvFile[i].name;
                    string fullPath = $"{savePath}/{dialogueName}.asset";

                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                        AssetDatabase.Refresh();
                    }

                    DialogueSo dialogueSo = AssetDatabase.LoadAssetAtPath<DialogueSo>(fullPath);
                    bool isNew = false;

                    if (dialogueSo == null)
                    {
                        isNew = true;
                    }

                    if (dialogueSo == null)
                        continue;

                    DialogueData dialogueData = new DialogueData
                    {
                        expression = values[1].Trim(),
                        condition = values[2].Trim(),
                        dialogue = values[3].Trim(),
                        actionEvent = values[4].Trim(),
                        nextKey = values[5].Trim()
                    };
                    
                    
                    dialogueSo.AddDialogueData(values[0].Trim(),  dialogueData);

                    if (isNew)
                    {
                        AssetDatabase.CreateAsset(dialogueSo, fullPath);
                    }

                    EditorUtility.SetDirty(dialogueSo);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // private T ParseFlags<T>(string rawData) where T : struct, Enum
        // {
        //     string cleanData = rawData.Replace("\"", "").Trim();
        //     if (string.IsNullOrEmpty(cleanData) || cleanData == "-") return default;
        //
        //     T result = default;
        //     string[] splitData = cleanData.Contains(",") ? cleanData.Split(',') : cleanData.Split('/');
        //
        //     foreach (var s in splitData)
        //     {
        //         if (Enum.TryParse(s.Trim(), out T val))
        //         {
        //             int intVal = Convert.ToInt32(val);
        //             int intResult = Convert.ToInt32(result);
        //             result = (T)(object)(intResult | intVal);
        //         }
        //     }
        //     return result;
        // }
#endif
    }
}
