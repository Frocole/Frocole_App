using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalenderManager : MonoBehaviour
{
    // components
    public List<CalenderWeek> Weeks = new List<CalenderWeek>();
    public Button NextMonth, PrevMonth;
    public Text Month_Year;

    public string MonthNames = "januari/februari/maart/april/mei/juni/juli/augustus/september/oktober/november/december";

    // Data 
    public int year = 2022;
    public int month = 10;

    // UnityEvent
    public UnityEvent<string> OnDateSelected = new UnityEvent<string>();


    // UtilityFunction
    [ContextMenu("SetData")] public void SetData() => UpdateData(year, month);


    private void Start()
    {
        month = DateTime.Now.Month;
        year = DateTime.Now.Year;

        UpdateData(year, month);


        NextMonth.onClick.AddListener(AddMonth);
        PrevMonth.onClick.AddListener(RemoveMonth);
    }

    /// <summary>
    /// Refreshes the Data in the calender.
    /// </summary>
    /// <param name="Year"></param>
    /// <param name="Month"></param>
    public void UpdateData(int Year, int Month)
    {       
        Month_Year.text = MonthNames.Split('/')[Month-1] + " " + Year.ToString();

        // determine the calender-position of the first day this month.
        DateTime FirstDay = new DateTime(Year, Month, 1);
        int Dayi = 0;
        switch (FirstDay.DayOfWeek)
        {
            case DayOfWeek.Monday:
                Dayi = 0;
                break;

            case DayOfWeek.Tuesday:
                Dayi = 1;
                break;

            case DayOfWeek.Wednesday:
                Dayi = 2;
                break;

            case DayOfWeek.Thursday:
                Dayi = 3;
                break;

            case DayOfWeek.Friday:
                Dayi = 4;
                break;

            case DayOfWeek.Saturday:
                Dayi = 5;
                break;

            case DayOfWeek.Sunday:
                Dayi = 6;
                break;
        }
        foreach (var week in Weeks)
        {
            foreach (var day in week.Days)
            {
                day.text = "";
            }
        }

        // Update the individual days of the month;
        void WriteDayInSlot(int Daynr, int StartSlotnr)
        {
            int WeekNr = 0;
            int slotNr = StartSlotnr + Daynr - 1;

            while (slotNr > 6)
            {
                WeekNr += 1;
                slotNr -= 7;
            }
            Weeks[WeekNr].Days[slotNr].text = Daynr.ToString();
        }

        for (int i = 1; i <= DateTime.DaysInMonth(Year, Month); i++)
        {
            WriteDayInSlot(i, Dayi);
        }

        // Update the weeks of the month;
        foreach (var week in Weeks)
        {
            week.Refresh(Year, Month);
        }
    }

    public void AddMonth()
    {
        //Debug.Log("addmonth");
        month += 1;

        if (month > 12)
        {
            month -= 12;
            year++;
        }

        UpdateData(year, month);
    }

    public void RemoveMonth()
    {
        //Debug.Log("reducemonth");

        month -= 1;

        if (month < 1)
        {
            month += 12;
            year--;
        }

        UpdateData(year, month);
    }

}
