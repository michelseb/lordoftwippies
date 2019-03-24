﻿using UnityEngine;
using UnityEngine.EventSystems;

public class RadialElement : GraphicElement
{
    [SerializeField]
    protected RadialSubMenu _subMenu;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Open()
    {
        _animator.ResetTrigger("Close");
        _animator.SetTrigger("Open");
    }
    public virtual void Close()
    {
        _animator.ResetTrigger("Open");
        _animator.SetTrigger("Close");
        if (_subMenu != null)
        {
            _subMenu.Close();
        }
    }
}
