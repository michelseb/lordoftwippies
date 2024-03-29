﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
/*
Radial Layout Group by Just a Pixel (Danny Goodayle) - http://www.justapixel.co.uk
Copyright (c) 2015
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
public class RadialLayout : LayoutGroup
{
    public float fDistance;
    [Range(0f, 360f)]
    public float MinAngle, MaxAngle, StartAngle;
    protected override void OnEnable() { base.OnEnable(); CalculateRadial(); }
    public override void SetLayoutHorizontal()
    {
    }
    public override void SetLayoutVertical()
    {
    }
    public override void CalculateLayoutInputVertical()
    {
        CalculateRadial();
    }
    public override void CalculateLayoutInputHorizontal()
    {
        CalculateRadial();
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        CalculateRadial();
    }
#endif
    void CalculateRadial()
    {
        m_Tracker.Clear();
        List<RectTransform> children = new List<RectTransform>();
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf)
                continue;
            var rectChild = (RectTransform)child;
            if (rectChild != null)
            {
                children.Add(rectChild);
            }
        }
        if (children.Count == 0)
            return;

        float fOffsetAngle = ((MaxAngle - MinAngle)) / (children.Count - 1);

        float fAngle = StartAngle;
        foreach (RectTransform child in children.OrderBy(c => c.gameObject.name))
        {
            if (child != null)
            {
                //Adding the elements to the tracker stops the user from modifiying their positions via the editor.
                m_Tracker.Add(this, child,
                DrivenTransformProperties.Anchors |
                DrivenTransformProperties.AnchoredPosition |
                DrivenTransformProperties.Pivot);
                child.localPosition = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad) * fDistance, Mathf.Sin(fAngle * Mathf.Deg2Rad) * fDistance, child.localPosition.z);
                child.localRotation = Quaternion.Euler(0, 0, fAngle - 90);
                //Force objects to be center aligned, this can be changed however I'd suggest you keep all of the objects with the same anchor points.
                child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.2f);
                fAngle += fOffsetAngle;
            }
        }

    }
}