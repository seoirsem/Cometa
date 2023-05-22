using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureTestScript : MonoBehaviour
{
    List<Vector3> vertices;
    GameObject textureTest;

    void Start()
    {
        textureTest = this.gameObject;

        vertices = new List<Vector3> { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(2, 0, 0) };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();

        int[] triangles = new int[] { 0, 1, 2, 2, 3, 0 };
        mesh.triangles = triangles;

        Vector2[] uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
        mesh.uv = uv;

        Texture2D texture = Resources.Load<Texture2D>("texture");
        MeshRenderer renderer = textureTest.GetComponent<MeshRenderer>();
        renderer.material.mainTexture = texture;

        MeshFilter filter = textureTest.GetComponent<MeshFilter>();
        filter.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
