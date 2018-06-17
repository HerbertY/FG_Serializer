using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

namespace FGame.CreateMesh
{
    public class MeshCreator : MonoBehaviour
    {
        //public static void CreateQuat()
        //{
        //    GameObject ob = new GameObject();
        //    ob.name = "fatoon_quat";

        //    Mesh mesh = new Mesh();
        //    MeshFilter meshFilter = ob.AddComponent<MeshFilter>();
        //    MeshRenderer meshRenderer = ob.AddComponent<MeshRenderer>();


        //    Vector3[] vertexs = new Vector3[4];
        //    vertexs[0] = new Vector3(-0.5f, -0.5f, 0);
        //    vertexs[1] = new Vector3(0.5f, -0.5f, 0);
        //    vertexs[2] = new Vector3(-0.5f, 0.5f, 0);
        //    vertexs[3] = new Vector3(0.5f, 0.5f, 0);

        //    int[] triangles = new int[2 * 3] {0, 2, 1, 1, 2, 3};

        //    Vector2[] uvs = new Vector2[vertexs.Length];
        //    uvs[0] = new Vector2(0, 0);
        //    uvs[1] = new Vector2(1, 0);
        //    uvs[2] = new Vector2(0, 1);
        //    uvs[3] = new Vector2(1, 1);

        //    mesh.vertices = vertexs;
        //    mesh.uv = uvs;
        //    mesh.triangles = triangles;

        //    meshFilter.mesh = mesh;
        //    ob.transform.position = Vector3.zero;
        //    ob.transform.localScale = Vector3.one;

        //    for (int i = 0; i < vertexs.Length; i++)
        //    {
        //        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //        cube.transform.parent = ob.transform;
        //        cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        //        cube.transform.localPosition = vertexs[i];
        //        cube.name = i.ToString();
        //    }    
        //}
    }
}

