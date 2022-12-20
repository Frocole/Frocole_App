using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ExtenderScrollView : ScrollRect
{

    private int[] _values;
    public UnityEvent OnSelected;

    public bool IsYearscroller = false;

    public void SetValue(int value)
    {
        Debug.Log($"1 - value({value}) / (_values.Length - 1)({(_values.Length - 1)}) = {1 - (float)value / (_values.Length - 1)}");

        normalizedPosition = new Vector2(0, 1 - (float)value / (_values.Length - 1));

        Debug.Log(" value " + GetValue());
        Debug.Log(normalizedPosition);
    }

    public void SetValues(int[] values)
    {
        _values = values;
    }

    public int GetValue()
    {
        int i = Mathf.RoundToInt((1 - normalizedPosition.y) * (_values.Length-1));
        //Debug.Log($"Getting item {i} out of {_values.Length}.");
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
