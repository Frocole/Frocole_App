using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DateScroller : MonoBehaviour
{
    public int[] MinuteValues, HourValues, DayValues, MonthValues, YearValues;
    public ExtenderScrollView MinuteScroller, HourScroller, DayScroller, MonthScroller, YearScroller;
    public GameObject Graph;
    public FillProfileDate Profile;

    public GameObject[] VariableDays;
    public TextMeshProUGUI[] VariableYears;



    // Start is called before the first frame update
    void Start()
    {
        YearValues[3] = System.DateTime.Now.Year;
        VariableYears[3].text = System.DateTime.Now.Year.ToString();
        YearValues[2] = System.DateTime.Now.Year-1;
        VariableYears[2].text = (System.DateTime.Now.Year - 1).ToString();
        YearValues[1] = System.DateTime.Now.Year-2;
        VariableYears[1].text = (System.DateTime.Now.Year - 2).ToString();
        YearValues[0] = System.DateTime.Now.Year-3; 
        VariableYears[0].text = (System.DateTime.Now.Year - 3).ToString();


        MinuteScroller.SetValues(MinuteValues);
        HourScroller.SetValues(HourValues);
        DayScroller.SetValues(DayValues);
        MonthScroller.SetValues(MonthValues);
        YearScroller.SetValues(YearValues);

        MinuteScroller.OnSelected.AddListener(() => GetDateTime());
        HourScroller.OnSelected.AddListener(() => GetDateTime());
        DayScroller.OnSelected.AddListener(() => GetDateTime());
        MonthScroller.OnSelected.AddListener(() => GetDateTime());
        YearScroller.OnSelected.AddListener(() => GetDateTime());

        /*
        Debug.Log($"Year : {System.DateTime.Now.Year}.");
        Debug.Log($"Month : {System.DateTime.Now.Month}.");
        Debug.Log($"Day : {System.DateTime.Now.Day}.");
        Debug.Log($"Hour : {System.DateTime.Now.Hour}.");
        Debug.Log($"Minute : {System.DateTime.Now.Minute}.");
        */

        YearScroller.SetValue(YearValues.Length-1);
        Debug.Log($"Year : {System.DateTime.Now.Year} -> {YearScroller.GetValue()}.");

        MonthScroller.SetValue(System.DateTime.Now.Month-1);
        Debug.Log($"Month : {System.DateTime.Now.Month} -> {MonthScroller.GetValue()}.");

        DayScroller.SetValue(System.DateTime.Now.Day - 1);
        Debug.Log($"Day : {System.DateTime.Now.Day} -> {DayScroller.GetValue()}.");

        HourScroller.SetValue(System.DateTime.Now.Hour);
        Debug.Log($"Hour : {System.DateTime.Now.Hour} -> {HourScroller.GetValue()}.");

        MinuteScroller.SetValue(Mathf.CeilToInt((System.DateTime.Now.Minute / 60f) * 11f));
        Debug.Log($"Minute : {System.DateTime.Now.Minute} -> {MinuteScroller.GetValue()}.");

        GetDateTime();
    }

    void GetDateTime()
    {
        int DaysInMonth = System.DateTime.DaysInMonth(YearScroller.GetValue(), MonthScroller.GetValue());

        DayScroller.SetValues(DayValues[0..(DaysInMonth)]);
        VariableDays[0].SetActive((DaysInMonth > 28));//29th
        VariableDays[1].SetActive((DaysInMonth > 29));//30th
        VariableDays[2].SetActive((DaysInMonth > 30));//31th

        string DateTime = ($"{YearScroller.GetValue()}-{MonthScroller.GetValue()}-{DayScroller.GetValue()} {HourScroller.GetValue()}:{MinuteScroller.GetValue()}:00");
        Debug.Log(DateTime);

        Profile.TestDatePicker = DateTime;
        if (!Graph.activeSelf) Graph.SetActive(true);
        Profile.Refresh();
    }



}
