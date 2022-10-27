using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class CalenderWeek : MonoBehaviour
{
    public Text WeekNR;
    public Text[] Days;

    [SerializeField] private string Year = "2020";
    [SerializeField] private string Month = "1";
    [SerializeField]private string Day = "1";
    public string Date => $"{Year}_{Month}_{Day}";

    public FillProfileDate FillProfileDate_RD;
    public GameObject DatedGraph;


    public void Refresh(int year, int month)
    {
        string LastDay = "";
        foreach (var _day in Days)
        {
            if (_day.text != "") LastDay = _day.text;
        }
        gameObject.SetActive(LastDay != "");

        if (LastDay == "") return;

        int day = 1;
        int.TryParse(LastDay, out day);

        Year = year.ToString();
        Month = month.ToString();
        Day = day.ToString();

        DateTime DT_LastDay = new DateTime(year, month, day);

        CultureInfo myCI = new CultureInfo("en-US");
        Calendar myCal = myCI.Calendar;

        // Gets the DTFI properties required by GetWeekOfYear.
        CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
        DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

        WeekNR.text = myCal.GetWeekOfYear(DT_LastDay, myCWR, myFirstDOW).ToString();
    }

    public void OnClicked()
    {
        Debug.Log(Date + " + " + $"{Year}_{Month}_{Day}");
        FillProfileDate_RD.TestDatePicker = Date;
        DatedGraph.SetActive(true);
    }


    
}
