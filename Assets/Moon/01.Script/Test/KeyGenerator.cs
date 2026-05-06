#if UNITY_EDITOR
using System.Text;
using UnityEngine;

namespace Moon._01.Script.Test
{
    public class KeyGenerator : MonoBehaviour
    {
        void Start()
        {
            string realKey = "vDcS249oPqX1R0==";
            string dummyKey = "b2WshK2D3d25oAE45n9sQ==";

            byte[] realBytes = Encoding.UTF8.GetBytes(realKey);
            byte[] dummyBytes = Encoding.UTF8.GetBytes(dummyKey);
            byte[] obfuscatedBytes = new byte[realBytes.Length];

            string resultString = "";

            for (int i = 0; i < realBytes.Length; i++)
            {
                obfuscatedBytes[i] = (byte)(realBytes[i] ^ dummyBytes[i % dummyBytes.Length]);
                resultString += obfuscatedBytes[i] + ", ";
            }

            // 콘솔에 출력된 숫자 배열을 복사해서 위의 obfuscatedKey 배열에 넣으면 됩니다.
            Debug.Log("{ " + resultString.TrimEnd(',', ' ') + " }"); 
        }
    }
}
#endif