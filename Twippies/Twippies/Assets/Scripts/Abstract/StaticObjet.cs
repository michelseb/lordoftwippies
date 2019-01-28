using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaticObjet : DraggableObjet {

    private bool _grounded;

    protected override void Start()
    {
        base.Start();
        _r.isKinematic = false;
    }


    protected virtual void FixedUpdate()
    {
        if (_grounded)
        {
            _r.velocity = Vector3.zero;
            _r.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (!_grounded)
        {
            if (other.gameObject.GetComponent<Planete>() != null)
            {
                _r.isKinematic = true;
                _grounded = true;
            }
        }
    }

    protected override void OnMouseDrag()
    {
        base.OnMouseDrag();
        _r.isKinematic = false;
        _grounded = false;
    }

}
