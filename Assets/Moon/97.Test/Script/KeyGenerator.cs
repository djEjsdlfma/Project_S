#if UNITY_EDITOR
using System.Text;
using UnityEngine;

namespace Moon._01.Script.Test
{
    public class KeyGenerator : MonoBehaviour
    {
        [ContextMenu("Generate Key")]
        void Start()
        {
            string realKey = "Jdv4F3g7ujU8sd==";
            string dummyKey = "U2seg1yS4x4D7h8Lnfdi8==";

            byte[] realBytes = Encoding.UTF8.GetBytes(realKey);
            byte[] dummyBytes = Encoding.UTF8.GetBytes(dummyKey);
            byte[] obfuscatedBytes = new byte[realBytes.Length];

            string resultString = "";

            for (int i = 0; i < realBytes.Length; i++)
            {
                obfuscatedBytes[i] = (byte)(realBytes[i] ^ dummyBytes[i % dummyBytes.Length]);
                resultString += obfuscatedBytes[i] + ", ";
            }

            // 콘솔에 출력된 숫자 배열을 복사하여 DataManager 안에있는 obfuscatedKey 배열에 넣으면 됨
            Debug.Log("{ " + resultString.TrimEnd(',', ' ') + " }"); 
        }
    }
}
#endif