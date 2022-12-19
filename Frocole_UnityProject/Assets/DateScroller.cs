using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateScroller : MonoBehaviour
{
    public string[] MinuteValues, HourValues, DayValues, MonthValues, YearValues;
    public ExtenderScrollView MinuteScroller, HourScroller, DayScroller, MonthScroller, YearScroller;
    public GameObject Graph;
    public FillProfileDate Profile;


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


    }

    void GetDateTime()
    {
        string DateTime = ($"{YearScroller.value()}-{MonthScroller.value()}-{DayScroller.value()} {HourScroller.value()}:{MinuteScroller.value()}:00");
        Profile.TestDatePicker = DateTime;
        //Graph.SetActive(false);
        Profile.Refresh();


    }


    
}
