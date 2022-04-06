using UnityEngine;

public abstract class StaticObjet : DraggableObjet
{
    protected override void Start()
    {
        base.Start();
        _rigidBody.isKinematic = false;
        OnZoneChanged(Zone);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (_grounded)
            return;

        base.OnCollisionEnter(other);
        
        if (!_grounded)
            return;

        _rigidBody.isKinematic = true;
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.angularVelocity = Vector3.zero;
    }

    protected override void OnMouseDrag()
    {
        base.OnMouseDrag();
        _rigidBody.isKinematic = false;
        _grounded = false;
    }

    protected abstract void OnZoneChanged(Zone zone);
}
