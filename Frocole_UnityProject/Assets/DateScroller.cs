using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateScroller : MonoBehaviour
{
    public int[] MinuteValues, HourValues, DayValues, MonthValues, YearValues;
    public ExtenderScrollView MinuteScroller, HourScroller, DayScroller, MonthScroller, YearScroller;
    public GameObject Graph;
    public FillProfileDate Profile;

    public GameObject[] VariableDays;




    // Start is called before the first frame update
    void Awake()
    {
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

        YearScroller.SetValue(YearValues.Length);
        MonthScroller.SetValue(System.DateTime.Now.Month);
        DayScroller.SetValue(System.DateTime.Now.Day - 1);
        HourScroller.SetValue(System.DateTime.Now.Hour);
        MinuteScroller.SetValue(Mathf.CeilToInt((System.DateTime.Now.Minute / 60f) * 11f));
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
        Profile.Refresh();
    }



}
