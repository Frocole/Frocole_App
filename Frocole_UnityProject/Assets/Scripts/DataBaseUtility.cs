using Time = System.DateTime;

public class DataBaseUtility
{
    // compare timestamps
    // 2022-01-06 10:53:08

    
    



    public static string CurrentTimestamp => $"{Time.Today.Year}-{Time.Today.Month}-{Time.Today.Day} {Time.Now.Hour}:{Time.Now.Minute}:{Time.Now.Second}";
    public static bool IsPastTimeStamp(string timestamp) => OldestTimeStamp(timestamp, CurrentTimestamp) == timestamp;
    public static bool IsFutureTimeStamp(string timestamp) => NewestTimeStamp(timestamp, CurrentTimestamp) == timestamp;    

    /// <summary>
    /// returns newest the time stamp. assumes "YYYY-MM-DD hh:mm:ss" with a 24 hour clock.
    /// </summary>
    /// <param name="timestamp1">The timestamp1.</param>
    /// <param name="timestamp2">The timestamp2.</param>
    /// <returns></returns>
    public static string NewestTimeStamp(string timestamp1, string timestamp2)
    {
       string[] timestamp1Arr = timestamp1.Split(' ');
       string[] timestamp2Arr = timestamp2.Split(' ');
       string newest = "";

        if (timestamp1Arr[0] == timestamp2Arr[0])
        {

            // compare time: hh:mm:ss
            timestamp1Arr = timestamp1Arr[1].Split(':');
            timestamp2Arr = timestamp2Arr[1].Split(':');

            // compare hours
            if (timestamp1Arr[0] != timestamp2Arr[0]) newest = IsBiggerInt(timestamp1Arr[0], timestamp2Arr[0]) ? timestamp1 : timestamp2;
            //compare minutes
            else if (timestamp1Arr[1] != timestamp2Arr[1]) newest = IsBiggerInt(timestamp1Arr[1], timestamp2Arr[1]) ? timestamp1 : timestamp2;
            //compare seconds
            else if (timestamp1Arr[2] != timestamp2Arr[2]) newest = IsBiggerInt(timestamp1Arr[2], timestamp2Arr[2]) ? timestamp1 : timestamp2;

        }
        else
        {

            // compare Date: YYYY-MM-DD
            timestamp1Arr = timestamp1Arr[0].Split('-');
            timestamp2Arr = timestamp2Arr[0].Split('-');

            // compare year
            if (timestamp1Arr[0] != timestamp2Arr[0]) newest = IsBiggerInt(timestamp1Arr[0], timestamp2Arr[0]) ? timestamp1 : timestamp2;
            // compare month
            else if (timestamp1Arr[1] != timestamp2Arr[1]) newest = IsBiggerInt(timestamp1Arr[1], timestamp2Arr[1]) ? timestamp1 : timestamp2;
            // compare day
            else if (timestamp1Arr[2] != timestamp2Arr[2]) newest = IsBiggerInt(timestamp1Arr[2], timestamp2Arr[2]) ? timestamp1 : timestamp2;

        }        

        return newest;
    }

    /// <summary>
    /// returns oldest the time stamp. assumes "YYYY-MM-DD hh:mm:ss" with a 24 hour clock.
    /// </summary>
    /// <param name="timestamp1">The timestamp1.</param>
    /// <param name="timestamp2">The timestamp2.</param>
    /// <returns></returns>
    public static string OldestTimeStamp(string timestamp1, string timestamp2)
    {
        string newest = NewestTimeStamp(timestamp1, timestamp2);
       
        if (timestamp1 == newest) return timestamp2;
        else if (timestamp2 == newest) return timestamp1;

        return "";
    }

    private static bool IsBiggerInt(string a, string b) => int.Parse(a) > int.Parse(b);
}
