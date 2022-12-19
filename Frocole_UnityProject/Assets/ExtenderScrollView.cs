using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ExtenderScrollView : ScrollRect
{

    private string[] _values;
    public UnityEvent OnSelected;


    public void SetValues(string[] values)
    {
        _values = values;
    }

    public string value()
    {
        int i = Mathf.RoundToInt((1 - normalizedPosition.y) * (_values.Length-1));
        return _values[i];
    }
         
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);        
        int i = Mathf.RoundToInt(normalizedPosition.y * (_values.Length-1));
        normalizedPosition =  new Vector2(0, (float)i / (_values.Length-1));
        //Debug.Log(value());
        OnSelected.Invoke();
    }
    
}
