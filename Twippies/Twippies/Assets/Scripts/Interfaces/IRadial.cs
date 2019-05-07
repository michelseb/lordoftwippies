using System.Collections;
using UnityEngine.EventSystems;

public interface IRadial : IPointerEnterHandler, ISelectHandler, IPointerExitHandler, IPointerClickHandler
{
    void Open();
    void Close();
    void Select();
    IEnumerator DeSelect(float delay = 0);
}
