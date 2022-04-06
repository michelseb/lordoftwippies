using System.Collections.Generic;
using UnityEngine;

public class TerrainFaceDisplay : MonoBehaviour
{
    public TerrainFace Face { get; set; }
    private Planet _planet;
    protected Vector3[] _originalVertices, _deformedVertices, _newVertices;
    private SortedDictionary<int, Vector3> _deformedVerticesDictionnary;
    public Mesh Mesh => Face.Mesh;
    public MeshCollider Collider => Face.Collider;
    public MeshFilter Filter => Face.Filter;

    private void Start()
    {
        _planet = GetComponentInParent<Planet>();

        _originalVertices = Face.Mesh.vertices;
        _newVertices = _originalVertices;
        _deformedVerticesDictionnary = new SortedDictionary<int, Vector3>();
    }


    protected virtual void OnMouseOver()
    {
        //if (!Shaping)
        //            return;

        
    }

    
}
