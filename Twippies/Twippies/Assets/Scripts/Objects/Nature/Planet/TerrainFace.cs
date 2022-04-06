using System.Linq;
using UnityEngine;

public class TerrainFace
{
    public GameObject GameObject { get; }
    public Mesh Mesh { get; }
    public MeshCollider Collider { get; private set; }
    public MeshFilter Filter { get; }

    private int _resolution;
    private Vector3 _localUp;
    private Vector3 _localForward;
    private Vector3 _localRight;

    public TerrainFace(GameObject gameObject, MeshFilter filter, int resolution, Vector3 localUp)
    {
        GameObject = gameObject;
        Filter = filter;
        Mesh = filter.mesh;
        _resolution = resolution;
        _localUp = localUp;

        _localForward = new Vector3(localUp.y, localUp.z, localUp.x);
        _localRight = Vector3.Cross(localUp, _localForward);
    }

    public Mesh Build()
    {
        var vertices = new Vector3[_resolution * _resolution];
        var triangles = new int[(_resolution - 1) * (_resolution - 1) * 6];
        var triangleIndex = 0;

        for (int y = 0; y < _resolution; y++)
        {
            for (int x = 0; x < _resolution; x++)
            {
                var vertexIndex = x + y * _resolution;
                
                var percent = new Vector2(x, y) / (_resolution - 1);
                var pointOnUnitCube = _localUp + (percent.x - .5f) * 2 * _localForward + (percent.y - .5f) * 2 * _localRight;
                var pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[vertexIndex] = pointOnUnitSphere;

                if (x == _resolution - 1 || y == _resolution - 1)
                    continue;

                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + _resolution + 1;
                triangles[triangleIndex + 2] = vertexIndex + _resolution;

                triangles[triangleIndex + 3] = vertexIndex;
                triangles[triangleIndex + 4] = vertexIndex + 1;
                triangles[triangleIndex + 5] = vertexIndex + _resolution + 1;

                triangleIndex += 6;

            }
        }

        Mesh.Clear();
        Mesh.vertices = vertices;
        //Mesh.normals = vertices;
        Mesh.triangles = triangles;
        //_mesh.RecalculateNormals();

        Collider = GameObject.AddComponent<MeshCollider>();
        Collider.sharedMesh = Mesh;
        Collider.convex = true;

        return Mesh;
    }
}
