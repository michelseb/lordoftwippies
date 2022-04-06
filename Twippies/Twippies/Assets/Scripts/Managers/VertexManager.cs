using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VertexManager : MonoBehaviour, IManager<Vertex>
{
    private static VertexManager _instance;
    public static VertexManager Instance { get { if (_instance == null) _instance = FindObjectOfType<VertexManager>(); return _instance; } }

    private Dictionary<Guid, Vertex> _vertexDictionary = new Dictionary<Guid, Vertex>();
    public Guid[] Vertices { get; private set; }

    public Vector3 GetPosition(Guid id) => FindById(id).Position;
    //public Vector3 GetPosition(int index) => transform.TransformPoint(FindByIndex(index).Position);
    //public Vertex FindByIndex(int index) => Vertices.Select(v => FindById(v)).FirstOrDefault(x => x.Index == index);
    //public Guid GetId(int index) => Vertices.FirstOrDefault(v => FindById(v).Index == index);
    //public int[] GetIndexes(IList<Guid> ids) => Vertices.Select(v => FindById(v).Index).ToArray();

    public void Initialize()
    {
        var vertices = GetComponentInChildren<MeshFilter>()?.mesh?.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            Add(new Vertex(i, vertices[i]));
        }

        Vertices = _vertexDictionary.Keys.ToArray();
    }

    public void Add(Vertex vertex)
    {
        if (_vertexDictionary.ContainsKey(vertex.Id))
            return;

        _vertexDictionary.Add(vertex.Id, vertex);
    }

    public Vertex FindById(Guid id)
    {
        if (!_vertexDictionary.ContainsKey(id))
            return null;

        return _vertexDictionary[id];
    }

    public bool TryGetById(Guid id, out Vertex zone)
    {
        zone = null;

        if (!_vertexDictionary.ContainsKey(id))
            return false;

        zone = _vertexDictionary[id];
        return true;
    }

    public void UpdateVertex(Guid id, Vector3 pos)
    {
        if (!_vertexDictionary.ContainsKey(id))
            return;

        _vertexDictionary[id].Position = pos;
    }

    public IList<Vertex> FindAll()
    {
        return _vertexDictionary.Values.ToArray();
    }
}
