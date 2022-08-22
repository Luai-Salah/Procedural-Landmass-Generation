using UnityEngine;
using System.Collections.Generic;

public class TerrainGeneration : MonoBehaviour
{
    public static Vector2 ViewerPosition { get { return m_ViewerPosition; } }
    private static Vector2 m_ViewerPosition;

    public const float MaxViewDistance = 450.0f;

    [SerializeField] private Transform m_Viewer;

    private int m_ChunkSize;
    private int m_ChunksVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> m_TerrainChunkDeictionary = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> m_TerrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    private void Awake()
    {
        m_ChunkSize = MapGenerator.MapChunkSize - 1;
        m_ChunksVisibleInViewDistance = Mathf.RoundToInt(MaxViewDistance / m_ChunkSize);
    }

    private void Update()
    {
        m_ViewerPosition = new Vector2(m_Viewer.position.x, m_Viewer.position.z);
        UpdateVisiableChunks();
    }

    private void UpdateVisiableChunks()
    {
        foreach (TerrainChunk terrain in m_TerrainChunksVisibleLastUpdate)
            terrain.SetVisible(false);

        m_TerrainChunksVisibleLastUpdate.Clear();

        int curChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / m_ChunkSize);
        int curChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / m_ChunkSize);

        for (int yOffset = -m_ChunksVisibleInViewDistance; yOffset <= m_ChunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -m_ChunksVisibleInViewDistance; xOffset <= m_ChunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(curChunkCoordX + xOffset, curChunkCoordY + yOffset);

                if (m_TerrainChunkDeictionary.ContainsKey(viewedChunkCoord))
                {
                    m_TerrainChunkDeictionary[viewedChunkCoord].Update();
                    if (m_TerrainChunkDeictionary[viewedChunkCoord].IsVisible)
                        m_TerrainChunksVisibleLastUpdate.Add(m_TerrainChunkDeictionary[viewedChunkCoord]);
                }

                else m_TerrainChunkDeictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, m_ChunkSize, transform));
            }
        }
    }

    public class TerrainChunk
    {
        public bool IsVisible { get { return m_MeshObject.activeSelf; } }

        private GameObject m_MeshObject;
        private Vector2 m_Position;
        Bounds m_Bounds;

        public TerrainChunk(Vector2 coords, int size, Transform parent)
        {
            m_Position = coords * size;
            m_Bounds = new Bounds(m_Position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(m_Position.x, 0.0f, m_Position.y);
            m_MeshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            m_MeshObject.transform.position = positionV3;
            m_MeshObject.transform.localScale = Vector3.one * size / 10.0f;
            m_MeshObject.transform.parent = parent;
            SetVisible(false);
        }

        public void Update()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(m_Bounds.SqrDistance(ViewerPosition));
            bool visible = viewerDstFromNearestEdge <= MaxViewDistance;
            SetVisible(visible);
        }

        public void SetVisible(bool visible) => m_MeshObject.SetActive(visible);
    }
}
