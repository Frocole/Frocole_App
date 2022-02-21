using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DatePicker : MonoBehaviour
{
    public Dropdown DdYear, DdMonth, DdDay, DdHour, DdMinute;

    public int SelectedYear, SelectedMonth, SelectedDay, SelectedHour, SelectedMinute;

    public LoadScene loadSceneAfterUpdateDeadline;

    private List<string> Years => getYearsInBracket();
    private List<string> Months = new List<string>
    { 
        "januari",
        "februari",
        "maart",
        "april",
        "mei",
        "juni",
        "juli",
        "augustus",
        "september",
        "oktober",
        "november",
        "december"
    };
    private List<string> Days => getDaysInMonth(SelectedYear, SelectedMonth);
    private List<string> Hours = new List<string>
    {     
        "00",
        "01",
        "02",
        "03",
        "04",
        "05",
        "06",
        "07",
        "08",
        "09",
        "10",
        "11",
        "12",
        "13",
        "14",
        "15",
        "16",
        "17",
        "18",
        "19",
        "20",
        "21",
        "22",
        "23"        
    };
    private List<string> Minutes = new List<string>
    {  
        "00",     
        "10",
        "20",
        "30",
        "40",
        "50"                
    };

    private int[] YearBracket = { 2022, 2050 };

    private List<string> getYearsInBracket()
    {
        List<string> yearsInBracketList = new List<string>();

        for (int i = 0; i < YearBracket[1] - YearBracket[0]; i++)
        {
            yearsInBracketList.Add((YearBracket[0] + i).ToString());
        }

        return yearsInBracketList;
    }
    private List<string> getDaysInMonth(int yeari, int monthi)
    {
        List<string> daysInBracketList = new List<string>();

        int days = DateTime.DaysInMonth(YearBracket[0] + yeari, monthi+1);

        for (int i = 0; i < days;  i++)
        {
            daysInBracketList.Add((i+1).ToString());
        }

        return daysInBracketList;
    }

    private void Start()
    {
        DdYear.onValueChanged.AddListener((int Y) => SelectedYear = Y);
        DdMonth.onValueChanged.AddListener((int Y) => SelectedMonth = Y);
        DdDay.onValueChanged.AddListener((int Y) => SelectedDay = Y);
        DdHour.onValueChanged.AddListener((int Y) => SelectedHour = Y);
        DdMinute.onValueChanged.AddListener((int Y) => SelectedMinute = Y);

        PrepareDropdown(DdYear, Years, DateTime.Now.Year - YearBracket[0]);
        PrepareDropdown(DdMonth, Months, DateTime.Now.Month - 1);
        PrepareDropdown(DdDay, Days, DateTime.Now.Day - 1);
        PrepareDropdown(DdHour, Hours, DateTime.Now.Hour);
        PrepareDropdown(DdMinute, Minutes, Mathf.FloorToInt(DateTime.Now.Minute/10)+1);

        DdYear.onValueChanged.AddListener((int a) => PrepareDropdown(DdDay, getDaysInMonth(a, SelectedMonth), SelectedDay));
        DdMonth.onValueChanged.AddListener((int a) => PrepareDropdown(DdDay, getDaysInMonth(SelectedYear, a), SelectedDay));

        if (!string.IsNullOrEmpty(PersistentData.Instance.LoginDataManager.CourseData.Deadline))
        {
            // 2022-02-23 16:25:41
            string[] StringArr = PersistentData.Instance.LoginDataManager.CourseData.Deadline.Split(' ',':','-');
            int itemAsInt;
            List<int> itemsAsInt = new List<int>();
            bool succes = true;
            foreach (var item in StringArr)
            {
                if (int.TryParse(item, out itemAsInt))
                {
                    itemsAsInt.Add(itemAsInt);
                }
                else 
                {
                    succes = false;
                }
            }            

            if (succes)
            {
                DdYear.value = Mathf.Clamp(itemsAsInt[0]- YearBracket[0], 0, Years.Count - 1);
                DdMonth.value = Mathf.Clamp(itemsAsInt[1] - 1, 0, 11);
                DdDay.value = Mathf.Clamp(itemsAsInt[2] - 1, 0, Days.Count - 1);

                DdHour.value = Mathf.Clamp(itemsAsInt[3], 0, 23);
                DdMinute.value = Mathf.Clamp(itemsAsInt[4] - 1, 0, Mathf.FloorToInt(DateTime.Now.Minute / 10));
            }
            else 
            {
                Debug.Log($"INVALID DEADLINE : {PersistentData.Instance.LoginDataManager.CourseData.Deadline}.");           
            
            }
        }
    }

    private void PrepareDropdown(Dropdown dropdown, List<string> options, int currentValue)
    {
        dropdown.options.Clear();
        foreach (string option in options)
        {
            dropdown.options.Add(new Dropdown.OptionData(option));
        }

        if (currentValue == 0)
        {
            dropdown.value = 1;
            dropdown.value = 0;
        }

       dropdown.value = Mathf.Clamp(currentValue, 0 , dropdown.options.Count-1);
       
    }

    public void UpdateDeadline()
    {
        StartCoroutine(UpdateDeadlineOndDB());
    }

    private IEnumerator UpdateDeadlineOndDB()
    {
        LoadingOverlay.AddLoader();

        string Deadline = $"{Years[SelectedYear]}-{SelectedMonth + 1}-{SelectedDay + 1} {SelectedHour}:{SelectedMinute * 10}:00";

        WWWForm form = new WWWForm();

        form.AddField("username", PersistentData.Instance.LoginDataManager.Username);
        form.AddField("password", PersistentData.Instance.LoginDataManager.Password);
        form.AddField("courseid", PersistentData.Instance.LoginDataManager.CourseData.CourseID);
        form.AddField("deadline", Deadline);

        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_SetCourseDeadline.php"), form))
        {
            yield return WWW_.SendWebRequest();

            if (WWW_.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(WWW_.error);
            }
            
        }

        LoadingOverlay.RemoveLoader();

        loadSceneAfterUpdateDeadline.Load();
        yield return null;
    }


}
