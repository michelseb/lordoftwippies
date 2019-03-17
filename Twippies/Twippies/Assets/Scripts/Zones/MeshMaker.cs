﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MIConvexHull;

public class MeshMaker {

    public static GameObject CreateSelection(ZoneManager zManager, Zone z, Vector3 worldPos, SortedDictionary<int, Vector3> deformedVertices = null)
    {
        GameObject go = new GameObject("ZoneObject");
        go.layer = 16;
        go.transform.position = z.Center;
        MeshFilter mf = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
        List<Vector3> newPoints = new List<Vector3>();
        
        foreach (int p in z.VerticeIds)
        {
            Vector3 nP;
            if (deformedVertices != null)
            {
                nP = deformedVertices[p] - z.Center;
                newPoints.Add(nP);
                nP += (deformedVertices[p] - worldPos).normalized * .5f;
            }
            else
            {
                nP = zManager.Vertices[p] - z.Center;
                newPoints.Add(nP);
                nP += (zManager.Vertices[p] - worldPos).normalized * .5f;
            }         
            newPoints.Add(nP);
        }
        mf.mesh = CreateMesh(newPoints);

        MeshRenderer mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshCollider col = go.AddComponent(typeof(MeshCollider)) as MeshCollider;
        mr.material.shader = Shader.Find("Transparent/Diffuse");
        Texture2D tex = new Texture2D(1, 1);
        mr.material.mainTexture = tex;
        col.sharedMesh = mf.mesh;
        col.inflateMesh = true;
        col.skinWidth = .1f;
        col.convex = true;
        col.isTrigger = true;
        go.transform.parent = z.transform;
        return go;
    }

    private static Mesh CreateMesh(IEnumerable<Vector3> vertices)
    {

        Mesh m = new Mesh();
        m.name = "ScriptedMesh";
        List<int> triangles = new List<int>();
        var usedVertices = vertices.Select(x => new Vertex(x)).ToList();
        var result = ConvexHull.Create(usedVertices);
        if (result.Result != null)
        {
            m.vertices = result.Result.Points.Select(x => x.ToVec()).ToArray();

            var xxx = result.Result.Points.ToList();

            foreach (var face in result.Result.Faces)
            {
                triangles.Add(xxx.IndexOf(face.Vertices[0]));
                triangles.Add(xxx.IndexOf(face.Vertices[1]));
                triangles.Add(xxx.IndexOf(face.Vertices[2]));
            }
            m.triangles = triangles.ToArray();
            m.RecalculateBounds();
            m.RecalculateNormals();

            return m;
        }
        return null;
    }

}
