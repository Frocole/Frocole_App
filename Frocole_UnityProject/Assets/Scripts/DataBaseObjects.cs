[System.Serializable]
public class RootUserObject
{
    public UserObject[] users;
}

[System.Serializable]
public class UserObject
{
    public string UserID;
    public string Username;  // Unique User Name 
    public string Password;  // Encrypted password
    public string Nickname;  // non unique user nickname 
    public string Public;
}

[System.Serializable]
public class RootGroupObject
{
    public GroupObject[] groups;
}

[System.Serializable]
public class GroupObject
{
    public string GroupID;
    public string GroupNickname;                    
    public string CourseID;
    public string Public;
}

[System.Serializable]
public class RootCourseObject
{
    public CourseObject[] courses;
}

[System.Serializable]
public class CourseObject
{
    public string CourseID;                         // Unique group Nickname
    public string CourseName;                       // Non-unique group Nickname
    public string IPF_RD_parameters;                // A set of up to 10 parameters on which peers can review each other
    public string GPF_RD_parameters;                // A set of up to 10 parameters on which peers can review their collaborative product
    public string GradingScale;
    public string LeraarUserID;
    public string CourseActive;

}

[System.Serializable]
public class RootFeedBackObject
{
    public FeedBackObject[] Feedback;
}

[System.Serializable]
public class FeedBackObject
{
    public string FeedBackItemID;                   
    public string Timestamp;                        
    public string GroupID;                          
    public string FeedbackSuplierID;               
    public string Subject;
    public string Parameter;
    public string Score;
}

[System.Serializable]
public class SubjectObject
{
    public string SubjectName;
    public string SubjectID;
    public string[] Contributors;
    public SpiderGraph.FeedbackType FeedbackType;
    public bool Public;
}