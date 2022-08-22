using UnityEngine;

[System.Serializable]
public struct TerrainType
{
    public string Name;
    public float Height;
    public Color Color;
}

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap, ColorMap, Mesh
    }

    public const int MapChunkSize = 241;

    public bool AutoUpdate = false;

    [SerializeField] private DrawMode m_DrawMode = DrawMode.NoiseMap;

    [SerializeField, Range(0.0f, 6.0f)] private int m_LevelOfDetail;
    [SerializeField] private float m_Scale;

    [SerializeField] private int m_Seed;
    [SerializeField] private Vector2 m_Offset;

    [SerializeField] private int m_Octives;
    [SerializeField] private float m_MeshHeightMultiplier;
    [SerializeField] private AnimationCurve m_MeshHeightCurve;
    [SerializeField, Range(0f, 1f)] private float m_Presistence;
    [SerializeField] private float m_Lacunarity;

    [SerializeField] private TerrainType[] m_Regions;

    private MapDisplay m_MapDisplay;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(MapChunkSize, MapChunkSize, m_Scale, m_Seed,
            m_Octives, m_Presistence, m_Lacunarity, m_Offset);

        Color[] colorMap = new Color[MapChunkSize * MapChunkSize];

        for (int y = 0; y < MapChunkSize; y++)
        {
            for (int x = 0; x < MapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < m_Regions.Length; i++)
                {
                    if (currentHeight <= m_Regions[i].Height)
                    {
                        colorMap[y * MapChunkSize + x] = m_Regions[i].Color;
                        break;
                    }
                }
            }
        }

        m_MapDisplay = GetComponent<MapDisplay>();

        if (m_DrawMode == DrawMode.NoiseMap)
            m_MapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        else if (m_DrawMode == DrawMode.ColorMap)
            m_MapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, MapChunkSize, MapChunkSize));
        else if (m_DrawMode == DrawMode.Mesh)
            m_MapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, m_MeshHeightMultiplier, m_MeshHeightCurve, m_LevelOfDetail),
                TextureGenerator.TextureFromColorMap(colorMap, MapChunkSize, MapChunkSize));
    }

    private void OnValidate()
    {
        if (m_Lacunarity < 1)
            m_Lacunarity = 1;
        if (m_Octives < 0)
            m_Octives = 0;
    }
}
