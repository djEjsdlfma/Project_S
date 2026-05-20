using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CCTVRenderer : MonoBehaviour
{
    [Header("Vision")]
    [Range(0f, 360f)] public float fov = 90f;
    public float range = 5f;
    public LayerMask wallLayer;
    [Range(8, 128)] public int segments = 60;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    void Start()
    {
        mesh = new Mesh { name = "FOV Mesh" };
        GetComponent<MeshFilter>().mesh = mesh;

        // 정점 = 중심 1개 + 호 위의 (segments+1)개
        vertices = new Vector3[segments + 2];
        // 삼각형 = segments개 (각 삼각형은 인덱스 3개)
        triangles = new int[segments * 3];

        // 삼각형 인덱스는 한 번만 세팅하면 됨
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;       // 중심
            triangles[i * 3 + 1] = i + 1;   // 호 위 점
            triangles[i * 3 + 2] = i + 2;   // 다음 호 위 점
        }
    }

    void LateUpdate()
    {
        BuildFovMesh();
    }

    void BuildFovMesh()
    {
        // 중심점 (로컬 좌표 기준)
        vertices[0] = Vector3.zero;

        float halfFov = fov * 0.5f;
        Vector3 origin = transform.position;

        for (int i = 0; i <= segments; i++)
        {
            // -halfFov ~ +halfFov 범위 (로컬 기준)
            float a = -halfFov + (fov * i / segments);
            float rad = a * Mathf.Deg2Rad;

            // 로컬 방향
            Vector2 localDir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            // 월드 방향 (오브젝트 회전 반영)
            Vector2 worldDir = transform.TransformDirection(localDir);

            // 벽까지 레이캐스트 - 벽에 막히면 그 지점까지만
            RaycastHit2D hit = Physics2D.Raycast(origin, worldDir, range, wallLayer);
            float dist = hit.collider != null ? hit.distance : range;

            // 메시는 로컬 좌표계라 localDir X dist 로 저장
            vertices[i + 1] = (Vector3)(localDir * dist);
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }
}
