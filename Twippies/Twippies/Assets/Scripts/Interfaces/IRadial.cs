using System.Collections;
using UnityEngine.EventSystems;

public interface IRadial : IPointerEnterHandler, ISelectHandler, IPointerExitHandler
{
    void Open();
    void Close();
    void Select();
    void DeSelect();
}
