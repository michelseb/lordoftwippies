using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UIObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [SerializeField]
    private Objet _objet;

    private Controls _controls;
    private ObjetManager _om;

    private void Awake()
    {
        _controls = Controls.Instance;
        _om = ObjetManager.Instance;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        _controls.NewObject = true;
        _controls.ctrl = Controls.ControlMode.Dragging;
        GameObject go = Instantiate(_objet.gameObject, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
        _controls.FocusedLayer = go.layer;
        if (go.GetComponent<ManageableObjet>() != null)
        {
            _controls.FocusedObject = go.GetComponent<ManageableObjet>();
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
    }
}
