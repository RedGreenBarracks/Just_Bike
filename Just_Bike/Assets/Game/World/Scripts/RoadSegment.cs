using UnityEngine;

/// <summary>
/// 도로 세그먼트. 프로시저럴 메시로 직진/좌곡선/우곡선 도로를 생성합니다.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class RoadSegment : MonoBehaviour
{
    public enum SegmentType { Straight, CurveLeft, CurveRight }

    [HideInInspector] public SegmentType segmentType;
    [HideInInspector] public Vector3 exitPoint;
    [HideInInspector] public Quaternion exitRotation;
    [HideInInspector] public float roadHalfWidth;

    /// <summary>
    /// 도로 세그먼트 메시를 생성합니다.
    /// </summary>
    public void Build(SegmentType type, float length, float width, float curveAngle, int resolution)
    {
        segmentType = type;
        roadHalfWidth = width / 2f;

        Mesh mesh = new Mesh();
        int vertCount = (resolution + 1) * 2;
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];
        int[] triangles = new int[resolution * 6];

        float halfWidth = width / 2f;

        if (type == SegmentType.Straight)
        {
            BuildStraight(vertices, uv, triangles, length, halfWidth, resolution);
            exitPoint = transform.TransformPoint(new Vector3(0, 0, length));
            exitRotation = transform.rotation;
        }
        else
        {
            float sign = (type == SegmentType.CurveLeft) ? -1f : 1f;
            float radius = length / (curveAngle * Mathf.Deg2Rad);
            BuildCurve(vertices, uv, triangles, radius, halfWidth, curveAngle, resolution, sign);

            // 출구 위치/회전 계산
            float angleRad = curveAngle * Mathf.Deg2Rad;
            Vector3 localExit;
            if (type == SegmentType.CurveRight)
            {
                // 곡선 중심은 오른쪽
                localExit = new Vector3(
                    radius * (1f - Mathf.Cos(angleRad)),
                    0,
                    radius * Mathf.Sin(angleRad));
            }
            else
            {
                localExit = new Vector3(
                    -radius * (1f - Mathf.Cos(angleRad)),
                    0,
                    radius * Mathf.Sin(angleRad));
            }
            exitPoint = transform.TransformPoint(localExit);

            float yawOffset = sign * curveAngle;
            exitRotation = transform.rotation * Quaternion.Euler(0, yawOffset, 0);
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void BuildStraight(Vector3[] verts, Vector2[] uv, int[] tris,
        float length, float halfW, int res)
    {
        for (int i = 0; i <= res; i++)
        {
            float t = (float)i / res;
            float z = t * length;
            int idx = i * 2;

            verts[idx] = new Vector3(-halfW, 0, z);
            verts[idx + 1] = new Vector3(halfW, 0, z);

            uv[idx] = new Vector2(0, t);
            uv[idx + 1] = new Vector2(1, t);

            if (i < res)
            {
                int ti = i * 6;
                tris[ti] = idx;
                tris[ti + 1] = idx + 2;
                tris[ti + 2] = idx + 1;
                tris[ti + 3] = idx + 1;
                tris[ti + 4] = idx + 2;
                tris[ti + 5] = idx + 3;
            }
        }
    }

    void BuildCurve(Vector3[] verts, Vector2[] uv, int[] tris,
        float radius, float halfW, float angle, int res, float sign)
    {
        // sign: +1 = 오른쪽 곡선, -1 = 왼쪽 곡선
        Vector3 center = new Vector3(sign * radius, 0, 0);

        for (int i = 0; i <= res; i++)
        {
            float t = (float)i / res;
            float a = t * angle * Mathf.Deg2Rad;

            // 곡선 중심에서 안쪽/바깥쪽
            float innerR = radius - halfW;
            float outerR = radius + halfW;

            Vector3 inner, outer;
            if (sign > 0)
            {
                // 오른쪽 곡선: 중심이 오른쪽, 시작각 180도에서 반시계방향
                inner = center + new Vector3(-innerR * Mathf.Cos(a), 0, innerR * Mathf.Sin(a));
                outer = center + new Vector3(-outerR * Mathf.Cos(a), 0, outerR * Mathf.Sin(a));
            }
            else
            {
                // 왼쪽 곡선: 중심이 왼쪽, 시작각 0도에서 시계방향
                inner = center + new Vector3(innerR * Mathf.Cos(a), 0, innerR * Mathf.Sin(a));
                outer = center + new Vector3(outerR * Mathf.Cos(a), 0, outerR * Mathf.Sin(a));
            }

            int idx = i * 2;
            // 왼쪽/오른쪽 정렬
            if (sign > 0)
            {
                verts[idx] = outer;
                verts[idx + 1] = inner;
            }
            else
            {
                verts[idx] = inner;
                verts[idx + 1] = outer;
            }

            uv[idx] = new Vector2(0, t);
            uv[idx + 1] = new Vector2(1, t);

            if (i < res)
            {
                int ti = i * 6;
                tris[ti] = idx;
                tris[ti + 1] = idx + 2;
                tris[ti + 2] = idx + 1;
                tris[ti + 3] = idx + 1;
                tris[ti + 4] = idx + 2;
                tris[ti + 5] = idx + 3;
            }
        }
    }

    /// <summary>
    /// 특정 월드 좌표가 이 도로 세그먼트 위에 있는지 대략적으로 판단합니다.
    /// </summary>
    public bool IsPositionOnRoad(Vector3 worldPos)
    {
        Vector3 local = transform.InverseTransformPoint(worldPos);
        // 간단한 판정: X축 기준 도로 폭 내에 있는지
        return Mathf.Abs(local.x) <= roadHalfWidth + 0.5f;
    }

    /// <summary>
    /// 도로 중심선 상의 가장 가까운 점을 반환합니다 (간단 버전).
    /// </summary>
    public Vector3 GetNearestCenterPoint(Vector3 worldPos)
    {
        Vector3 local = transform.InverseTransformPoint(worldPos);
        local.x = 0;
        local.y = 0;
        return transform.TransformPoint(local);
    }
}
