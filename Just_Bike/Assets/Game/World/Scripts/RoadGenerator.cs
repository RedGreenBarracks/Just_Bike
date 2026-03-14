using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class RoadGenerator : MonoBehaviour
{
    [Inject] private GameManager gameManager;
    [Inject] private BikeController bikeController;

    [Header("도로 설정")]
    public float segmentLength = 30f;
    public float roadWidth = 8f;
    public float curveAngle = 30f;
    public int meshResolution = 10;

    [Header("생성 설정")]
    public int segmentsAhead = 8;
    public int segmentsBehind = 3;

    [Header("도로 색상")]
    public Color roadColor = new Color(0.3f, 0.3f, 0.35f);
    public Color lineColor = Color.white;

    private Transform player;
    private LinkedList<RoadSegment> segments = new LinkedList<RoadSegment>();
    private Vector3 nextSpawnPoint;
    private Quaternion nextSpawnRotation;
    private Material roadMaterial;
    private bool isActive;

    void Start()
    {
        player = bikeController.transform;

        roadMaterial = new Material(Shader.Find("Standard"));
        roadMaterial.color = roadColor;

        gameManager.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDestroy()
    {
        if (gameManager != null)
            gameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    void OnGameStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.Playing)
        {
            ClearRoad();
            InitRoad();
            isActive = true;
        }
        else
        {
            isActive = false;
            ClearRoad();
        }
    }

    void InitRoad()
    {
        nextSpawnPoint = Vector3.zero;
        nextSpawnRotation = Quaternion.identity;

        SpawnSegment(RoadSegment.SegmentType.Straight);

        for (int i = 1; i < segmentsAhead; i++)
        {
            SpawnRandomSegment();
        }
    }

    void Update()
    {
        if (!isActive || player == null) return;

        while (segments.Count > 0 && segments.Count < segmentsAhead + segmentsBehind)
        {
            SpawnRandomSegment();
        }

        while (segments.Count > 0)
        {
            RoadSegment first = segments.First.Value;
            float distBehind = Vector3.Dot(
                first.exitPoint - player.position,
                player.forward);

            if (distBehind < -segmentLength * segmentsBehind)
            {
                segments.RemoveFirst();
                Destroy(first.gameObject);
            }
            else break;
        }

        if (segments.Count > 0)
        {
            RoadSegment last = segments.Last.Value;
            float distAhead = Vector3.Distance(player.position, last.exitPoint);
            if (distAhead < segmentLength * segmentsAhead * 0.5f)
            {
                SpawnRandomSegment();
            }
        }

        EnforceRoadBoundary();
    }

    void SpawnRandomSegment()
    {
        float rand = Random.value;
        RoadSegment.SegmentType type;

        if (rand < 0.5f)
            type = RoadSegment.SegmentType.Straight;
        else if (rand < 0.75f)
            type = RoadSegment.SegmentType.CurveLeft;
        else
            type = RoadSegment.SegmentType.CurveRight;

        SpawnSegment(type);
    }

    void SpawnSegment(RoadSegment.SegmentType type)
    {
        GameObject obj = new GameObject("RoadSegment_" + type);
        obj.transform.position = nextSpawnPoint;
        obj.transform.rotation = nextSpawnRotation;
        obj.layer = 0;

        obj.AddComponent<MeshFilter>();
        var renderer = obj.AddComponent<MeshRenderer>();
        renderer.material = roadMaterial;
        obj.AddComponent<MeshCollider>();

        var segment = obj.AddComponent<RoadSegment>();
        segment.Build(type, segmentLength, roadWidth, curveAngle, meshResolution);

        segments.AddLast(segment);

        nextSpawnPoint = segment.exitPoint;
        nextSpawnRotation = segment.exitRotation;
    }

    void ClearRoad()
    {
        foreach (var seg in segments)
        {
            if (seg != null)
                Destroy(seg.gameObject);
        }
        segments.Clear();
    }

    void EnforceRoadBoundary()
    {
        if (bikeController == null) return;

        RoadSegment closest = null;
        float minDist = float.MaxValue;

        foreach (var seg in segments)
        {
            Vector3 center = seg.GetNearestCenterPoint(player.position);
            float dist = Vector3.Distance(player.position, center);
            if (dist < minDist)
            {
                minDist = dist;
                closest = seg;
            }
        }

        if (closest != null && !closest.IsPositionOnRoad(player.position))
        {
            Vector3 center = closest.GetNearestCenterPoint(player.position);
            bikeController.PushBackToRoad(center, closest.roadHalfWidth);
        }
    }
}
