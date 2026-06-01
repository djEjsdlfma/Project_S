using System.Collections.Generic;
using LSW._02._Code.Environment.Takable;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder;
using UnityEngine;

namespace Moon._01.Script.Cameras
{
    public class CameraCapture : MonoBehaviour
    {
        [SerializeField] private Camera photoCamera; // 마우스를 따라다니는 서브 카메라
        [SerializeField] private RenderTexture renderTexture; // 카메라에 연결된 렌더 텍스처
        [SerializeField] private ScriptListFinderSO camerasFinder;

        [SerializeField] private LayerMask photoObjectLayer;

        // 찰칵! 하는 순간 호출할 함수
        public void TakePhoto()
        {
            // 1. 현재 활성화된 렌더 텍스처를 백업
            RenderTexture currentRT = RenderTexture.active;

            // 2. 활성 렌더 텍스처를 우리 카메라의 텍스처로 변경
            RenderTexture.active = renderTexture;

            // 3. 카메라 강제 렌더링 (최신 프레임 보장)
            photoCamera.Render();

            // =========================================================
            // 🚨 GPU를 활용한 감마(Gamma) 보정 적용 구역 시작
            // =========================================================

            // A. sRGB(감마 보정)가 강제로 적용된 '임시 렌더 텍스처' 생성
            RenderTexture tempRT = RenderTexture.GetTemporary(
                renderTexture.width, 
                renderTexture.height, 
                0, 
                RenderTextureFormat.ARGB32, 
                RenderTextureReadWrite.sRGB
            );

            // B. 원본 텍스처(renderTexture)의 화면을 임시 텍스처(tempRT)로 복사! (이때 밝기가 예쁘게 보정됨)
            Graphics.Blit(renderTexture, tempRT);

            // C. 픽셀을 읽어오기 위해 활성 렌더 텍스처를 '임시 렌더 텍스처'로 변경
            RenderTexture.active = tempRT;

            // 4. 새로운 비어있는 Texture2D 생성 
            // ⚠️ 주의: 맨 마지막 파라미터(linear)를 반드시 'false'로 설정해야 합니다!
            Texture2D capture = new Texture2D(tempRT.width, tempRT.height, TextureFormat.RGB24, false, false);

            // 5. 화면의 픽셀을 읽어와서 Texture2D에 기록
            capture.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
            capture.Apply(); // 변경사항 적용

            // D. 다 쓴 임시 렌더 텍스처는 메모리에서 해제 (매우 중요)
            RenderTexture.ReleaseTemporary(tempRT);

            // =========================================================
            // 🚨 감마 보정 구역 끝
            // =========================================================

            // 6. 원래 렌더 텍스처로 복구
            RenderTexture.active = currentRT;

            List<string> objs = AnalyzePhoto();

            Photo photo = new Photo(capture, objs);

            camerasFinder.GetTarget<PhotoStorage>().AddPhoto(photo);
        }

        private List<string> AnalyzePhoto()
        {
            Vector2 cameraPos = photoCamera.transform.position;
            float height = photoCamera.orthographicSize * 2f;
            float width = height * photoCamera.aspect;
            Vector2 boxSize = new Vector2(width, height);

            Bounds cameraBounds = new Bounds(cameraPos, boxSize);
            Collider2D[] hits = Physics2D.OverlapBoxAll(cameraPos, boxSize, 0f, photoObjectLayer);

            List<string> camObjs = new List<string>();

            foreach (Collider2D hit in hits)
            {
                CamObject camObj = hit.GetComponent<CamObject>();
                if (camObj != null)
                {
                    if (IsMoreThanHalfInside(hit, cameraBounds, camObj))
                    {
                        camObjs.Add(camObj.Name);
                        if(camObj.TryGetComponent(out ITakable takable) && takable.CanBeTaken())
                        {
                            takable.Take();
                        }
                    }
                }
            }

            return camObjs;
        }

        private bool IsMoreThanHalfInside(Collider2D collider, Bounds cameraBounds, CamObject camObj)
        {
            Bounds objBounds = collider.bounds;

            // 검사할 해상도 (40x40 = 1600개의 점 검사)
            const int resolution = 40;

            int totalPointsInObject = 0;
            int pointsInsideCamera = 0;

            float stepX = objBounds.size.x / resolution;
            float stepY = objBounds.size.y / resolution;

            for (int i = 0; i <= resolution; i++)
            {
                for (int j = 0; j <= resolution; j++)
                {
                    Vector2 point = new Vector2(
                        objBounds.min.x + (stepX * i),
                        objBounds.min.y + (stepY * j)
                    );

                    if (collider.OverlapPoint(point))
                    {
                        totalPointsInObject++;

                        if (cameraBounds.Contains(point))
                        {
                            pointsInsideCamera++;
                        }
                    }
                }
            }

            if (totalPointsInObject == 0 || camObj.Ratio <= 0f) return false;

            float estimatedOriginalPoints = totalPointsInObject / camObj.Ratio;

            float finalRatio = pointsInsideCamera / estimatedOriginalPoints;

            return finalRatio >= 0.5f;
        }
    }
}