using System;
using UnityEngine;

public class Vertex
{
    public Guid Id { get; private set; }
    public Vector3 Position { get; set; }

    public Vertex(int index, Vector3 position)
    {
        Id = Utils.IntToGuid(index);
        Position = position;
    }
}