using UnityEngine;

public class CCTV : MonoBehaviour
{
    [Header("Vision Settings")]
    [Range(0f, 360f)] public float fov = 90f;      // 시야각 (도)
    public float range = 5f;                        // 시야 반경
    public LayerMask wallLayer;                     // 벽 레이어
    public LayerMask targetLayer;                   // 감지 대상 레이어

    [Header("Gizmo Settings")]
    public Color visionColor = new Color(0.2f, 0.6f, 1f, 0.15f);
    public Color detectedColor = new Color(1f, 0.3f, 0.3f, 0.25f);
    public Color edgeColor = new Color(0.2f, 0.6f, 1f, 0.8f);
    [Range(8, 128)] public int gizmoSegments = 40;  // 부채꼴 매끄러움

    // 감지 로직 — 런타임에서 호출
    public bool CanSee(Transform target)
    {
        Vector2 origin = transform.position;
        Vector2 toTarget = (Vector2)target.position - origin;
        float dist = toTarget.magnitude;

        if (dist > range) return false;

        // 카메라가 바라보는 방향 (transform.right 기준)
        Vector2 dir = transform.right;
        float dot = Vector2.Dot(dir, toTarget.normalized);
        if (dot < Mathf.Cos(fov * 0.5f * Mathf.Deg2Rad)) return false;

        // 차단 검사
        RaycastHit2D hit = Physics2D.Raycast(origin, toTarget.normalized, dist, wallLayer);
        if (hit.collider != null) return false;

        return true;
    }

    // 시야 안의 모든 타겟 찾기
    public Transform[] FindVisibleTargets()
    {
        Collider2D[] candidates = Physics2D.OverlapCircleAll(
            transform.position, range, targetLayer);

        var visible = new System.Collections.Generic.List<Transform>();
        foreach (var c in candidates)
        {
            if (CanSee(c.transform)) visible.Add(c.transform);
        }
        return visible.ToArray();
    }

    void OnDrawGizmos()
    {
        DrawFovGizmo();
    }

    void DrawFovGizmo()
    {
        Vector3 origin = transform.position;
        float halfFov = fov * 0.5f;
        // transform.right 기준 각도
        float baseAngle = Mathf.Atan2(transform.right.y, transform.right.x) * Mathf.Rad2Deg;

        // 1. 벽에 맞춰 잘린 부채꼴 외곽선 점들 계산
        Vector3[] points = new Vector3[gizmoSegments + 1];
        for (int i = 0; i <= gizmoSegments; i++)
        {
            float a = baseAngle - halfFov + (fov * i / gizmoSegments);
            float rad = a * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            // 벽까지 레이캐스트해서 실제 도달 거리 계산
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, range, wallLayer);
            float d = hit.collider != null ? hit.distance : range;
            points[i] = origin + (Vector3)(dir * d);
        }

        // 2. 부채꼴 채우기 (반투명 삼각형들)
        Gizmos.color = visionColor;
        for (int i = 0; i < gizmoSegments; i++)
        {
            // OnDrawGizmos에선 삼각형을 직접 못 그려서 선으로 fill 흉내
            Gizmos.DrawLine(origin, points[i]);
        }

        // 3. 부채꼴 외곽선
        Gizmos.color = edgeColor;
        for (int i = 0; i < gizmoSegments; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
        // 양 끝 (카메라 > 호의 양 끝점)
        Gizmos.DrawLine(origin, points[0]);
        Gizmos.DrawLine(origin, points[gizmoSegments]);

        // 4. 감지된 타겟 강조
        if (Application.isPlaying && targetLayer != 0)
        {
            Gizmos.color = detectedColor;
            foreach (var t in FindVisibleTargets())
            {
                Gizmos.DrawLine(origin, t.position);
                Gizmos.DrawWireSphere(t.position, 0.2f);
            }
        }
    }
}
