using UnityEngine;

public class MeshData
{
    public Vector3[] Verticies;
    public int[] Tringles;
    public Vector2[] Uvs;

    public int Index;

    public MeshData(int meshWidth, int meshHeight)
    {
        Verticies = new Vector3[meshWidth * meshHeight];
        Uvs = new Vector2[meshWidth * meshHeight];
        Tringles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTringle(int a, int b, int c)
    {
        Tringles[Index + 0] = a;
        Tringles[Index + 1] = b;
        Tringles[Index + 2] = c;
        Index += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Verticies;
        mesh.triangles = Tringles;
        mesh.uv = Uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] hieghtMap, float heightMultiplier, AnimationCurve hightCurve, int levelOfDetail)
    {
        int width = hieghtMap.GetLength(0);
        int height = hieghtMap.GetLength(1);

        float topLeftX = (width - 1) / -2.0f;
        float topLeftZ = (height - 1) / 2.0f;

        int simplificationImplement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        int vertsPerLine = (width - 1) / simplificationImplement + 1;

        MeshData meshData = new MeshData(vertsPerLine, vertsPerLine);
        int vertexIndex = 0;

        for (int y = 0; y < height; y+= simplificationImplement)
        {
            for (int x = 0; x < width; x+= simplificationImplement)
            {
                meshData.Verticies[vertexIndex] = new Vector3(topLeftX + x, hightCurve.Evaluate(hieghtMap[x, y]) * heightMultiplier, topLeftZ - y);
                meshData.Uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTringle(vertexIndex, vertexIndex + vertsPerLine + 1, vertexIndex + vertsPerLine);
                    meshData.AddTringle(vertexIndex + vertsPerLine + 1, vertexIndex, vertexIndex + 1);
                }    

                vertexIndex++;
            }
        }

        return meshData;
    }
}
