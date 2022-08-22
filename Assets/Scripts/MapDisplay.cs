using System;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private Renderer m_TextureRenderer;
    [SerializeField] private MeshFilter m_MeshFilter;
    [SerializeField] private MeshRenderer m_MeshRenderer;

    public void DrawTexture(Texture2D texture)
    {
        m_TextureRenderer.sharedMaterial.mainTexture = texture;
        m_TextureRenderer.transform.localScale = new Vector3(texture.width, 1.0f, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        m_MeshFilter.sharedMesh = meshData.CreateMesh();
        m_MeshRenderer.sharedMaterial.mainTexture = texture;
    }
}
