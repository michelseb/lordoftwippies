﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MIConvexHull;

public class MeshMaker {

    public static GameObject CreateSelection(IEnumerable<Vector3> points, Transform t, Vector3 pos, Vector3 worldPos)
    {
        GameObject go = new GameObject("ZoneObject");
        go.layer = 2;
        
        go.transform.position = pos;
        MeshFilter mf = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
        List<Vector3> newPoints = new List<Vector3>();
        foreach (Vector3 p in points)
        {
            Vector3 nP = p - pos;
            newPoints.Add(nP);
            nP += (p - worldPos).normalized * .5f;
            newPoints.Add(nP);
        }
        mf.mesh = CreateMesh(newPoints);

        MeshRenderer mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshCollider col = go.AddComponent(typeof(MeshCollider)) as MeshCollider;
        mr.material.shader = Shader.Find("Transparent/Diffuse");
        Texture2D tex = new Texture2D(1, 1);
        //tex.SetPixel(0, 0, Color.white);
        //tex.Apply();
        mr.material.mainTexture = tex;
        //mr.material.color = Color.green;
        col.sharedMesh = mf.mesh;
        col.inflateMesh = true;
        col.skinWidth = .1f;
        col.convex = true;
        col.isTrigger = true;
        go.transform.parent = t;
        //go.transform.localScale = Vector3.one * 2f;
        return go;
    }

    private static Mesh CreateMesh(IEnumerable<Vector3> vertices)
    {

        Mesh m = new Mesh();
        m.name = "ScriptedMesh";
        List<int> triangles = new List<int>();
        var usedVertices = vertices.Select(x => new Vertex(x)).ToList();
        var result = ConvexHull.Create(usedVertices);
        m.vertices = result.Result.Points.Select(x => x.ToVec()).ToArray();

        var xxx = result.Result.Points.ToList();

        foreach (var face in result.Result.Faces)
        {
            triangles.Add(xxx.IndexOf(face.Vertices[0]));
            triangles.Add(xxx.IndexOf(face.Vertices[1]));
            triangles.Add(xxx.IndexOf(face.Vertices[2]));
        }
        /*
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
        }*/

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
        //m.uv = uvs;
        m.triangles = triangles.ToArray();
        m.RecalculateBounds();
        m.RecalculateNormals();

        return m;
    }

}
