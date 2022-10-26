using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalenderManager : MonoBehaviour
{
    public List<CalenderWeek> Weeks = new List<CalenderWeek>();

    public int year = 2022;
    public int month = 10;

    [ContextMenu("SetData")]
    public void SetData()
    {
        UpdateData(year, month);
    }


    public void UpdateData(int Year, int Month)
    {

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

        for (int i = 1; i <= DateTime.DaysInMonth(Year, Month); i++)
        {
            WriteDayInSlot(i, Dayi);
        }

        foreach (var week in Weeks)
        {
            week.Refresh(Year, Month);
        }
    }

    void WriteDayInSlot(int Daynr, int StartSlotnr)
    {
        int WeekNr = 0;
        int slotNr = StartSlotnr + Daynr - 1;

        while (slotNr > 6)
        {
            WeekNr += 1;
            slotNr -= 7;
        }
        //Debug.Log($" Weeks[{WeekNr}].Days[{slotNr}].text");
        Weeks[WeekNr].Days[slotNr].text = Daynr.ToString();

    }


}
