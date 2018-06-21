using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerOnUI : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler{

	public void OnPointerExit(PointerEventData data)
    {
        StaticLibrary.pointerOnUI = false;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        StaticLibrary.pointerOnUI = true;
    }
}
