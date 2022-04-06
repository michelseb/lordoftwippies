using System;
using UnityEngine;

public class Objet : MonoBehaviour
{
    public Guid Id { get; private set; }

    protected virtual void Awake()
    {
        Id = Guid.NewGuid();
    }
}
