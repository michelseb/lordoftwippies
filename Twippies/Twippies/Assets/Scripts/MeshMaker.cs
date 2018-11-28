using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshMaker {

    public static GameObject CreateZone(Vector3[] vertices, Transform t, Material mat)
    {
        GameObject go = new GameObject("ZoneObject");
        MeshFilter mf = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        Mesh m = new Mesh();
        m.vertices = vertices;
        int[] triangles = new int[(vertices.Length-2) * 3];

        Vector2[] uvs = new Vector2[vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
        }
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = vertices.Length-1;

        for (int a = 0; a < vertices.Length-(2 * 3 + 3); a+=2)
        {
            triangles[a * 3 + 3] = (vertices.Length-1) - (a/2);
            triangles[a * 3 + 4] = (a/2) + 1;
            triangles[a * 3 + 5] = (a/2) + 2;
            triangles[(a + 1) * 3 + 3] = (vertices.Length-1) - (a/2);
            triangles[(a + 1) * 3 + 4] = (a/2) + 2;
            triangles[(a + 1) * 3 + 5] = (vertices.Length-1) - (a/2) - 1;
        }

        /*for (int a = 0; a < triangles.Length; a++)
        {
            Debug.Log(a + " " + triangles[a]);
        }*/

        /*for (int a = 0; a < vertices.Length; a++)
        {
            triangles[a*3] = 0;
            triangles[a*3 + 1] = a + 1;
            triangles[a*3 + 2] = a + 2;
        }*/
        m.uv = uvs;
        m.triangles = triangles;
        mf.mesh = m;
        MeshCollider col = go.AddComponent(typeof(MeshCollider)) as MeshCollider;
        col.sharedMesh = m;
        col.convex = true;
        m.RecalculateBounds();
        m.RecalculateNormals();
        go.transform.parent = t;
        mr.material = mat;
        return go;
    }

}
