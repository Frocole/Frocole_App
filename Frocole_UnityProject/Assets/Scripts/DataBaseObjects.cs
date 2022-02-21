/*
Copyright (c) 2021, G.W. van der Vegt
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided
that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the
  following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
  the following disclaimer in the documentation and/or other materials provided with the distribution.

* Neither the name of G.W. van der Vegt nor the names of its contributors may be
  used to endorse or promote products derived from this software without specific prior written
  permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF
THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
/// <summary>
/// A course object.
/// </summary>
[System.Serializable]
public class CourseObject
{
    #region Fields

    /// <summary>
    /// The course active.
    /// </summary>
    public string CourseActive;

    /// <summary>
    /// Unique group Nickname.
    /// </summary>
    public string CourseID;

    /// <summary>
    /// Non-unique group Nickname.
    /// </summary>
    public string CourseName;

    /// <summary>
    /// A set of up to 10 parameters on which peers can review their collaborative product.
    /// </summary>
    public string GPF_RD_parameters;

    /// <summary>
    /// The grading scale.
    /// </summary>
    public string GradingScale;

    /// <summary>
    /// A set of up to 10 parameters on which peers can review each other.
    /// </summary>
    public string IPF_RD_parameters;

    /// <summary>
    /// Identifier for the leraar user.
    /// </summary>
    public string LeraarUserID;

    /// <summary>
    /// The deadline on which feedback is next reviewed
    /// </summary>
    public string Deadline;

    #endregion Fields
}

/// <summary>
/// A feed back object.
/// </summary>
[System.Serializable]
public class FeedBackObject
{
    #region Fields

    /// <summary>
    /// Identifier for the feed back item.
    /// </summary>
    public string FeedBackItemID;

    /// <summary>
    /// Identifier for the feedback suplier.
    /// </summary>
    public string FeedbackSuplierID;

    /// <summary>
    /// Identifier for the group.
    /// </summary>
    public string GroupID;

    /// <summary>
    /// The parameter.
    /// </summary>
    public string Parameter;

    /// <summary>
    /// The score.
    /// </summary>
    public string Score;

    /// <summary>
    /// The subject.
    /// </summary>
    public string Subject;

    /// <summary>
    /// The timestamp.
    /// </summary>
    public string Timestamp;

    #endregion Fields
}

/// <summary>
/// A group object.
/// </summary>
[System.Serializable]
public class GroupObject
{
    #region Fields

    /// <summary>
    /// Identifier for the course.
    /// </summary>
    public string CourseID;

    /// <summary>
    /// Identifier for the group.
    /// </summary>
    public string GroupID;

    /// <summary>
    /// The group nickname.
    /// </summary>
    public string GroupNickname;

    /// <summary>
    /// The public.
    /// </summary>
    public string Public;

    #endregion Fields
}

/// <summary>
/// A root course object.
/// </summary>
[System.Serializable]
public class RootCourseObject
{
    #region Fields

    /// <summary>
    /// The courses.
    /// </summary>
    public CourseObject[] courses;

    #endregion Fields
}

/// <summary>
/// A root feed back object.
/// </summary>
[System.Serializable]
public class RootFeedBackObject
{
    #region Fields

    /// <summary>
    /// The feedback.
    /// </summary>
    public FeedBackObject[] Feedback;

    #endregion Fields
}

/// <summary>
/// A root group object.
/// </summary>
[System.Serializable]
public class RootGroupObject
{
    #region Fields

    /// <summary>
    /// The groups.
    /// </summary>
    public GroupObject[] groups;

    #endregion Fields
}

/// <summary>
/// Feedback and Reflection in Online Collaborative Learning.
/// 
///     Copyright (C) 2021  Open University of the Netherlands
/// 
///     This program is free software: you can redistribute it and/or modify it under the terms
///     of the GNU General Public License as published by the Free Software Foundation, either
///     version 3 of the License, or (at your option) any later version.
/// 
///     This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
///     without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
///     See the GNU General Public License for more details.
/// 
///     You should have received a copy of the GNU General Public License along with this
///     program.  If not, see <https://www.gnu.org/licenses/>.
/// </summary>
[System.Serializable]
public class RootUserObject
{
    #region Fields

    /// <summary>
    /// The users.
    /// </summary>
    public UserObject[] users;

    #endregion Fields
}

/// <summary>
/// A subject object.
/// </summary>
[System.Serializable]
public class SubjectObject
{
    #region Fields

    /// <summary>
    /// The contributors.
    /// </summary>
    public string[] Contributors;

    /// <summary>
    /// Type of the feedback.
    /// </summary>
    public SpiderGraph.FeedbackType FeedbackType;

    /// <summary>
    /// True to public.
    /// </summary>
    public bool Public;

    /// <summary>
    /// Identifier for the subject.
    /// </summary>
    public string SubjectID;

    /// <summary>
    /// Name of the subject.
    /// </summary>
    public string SubjectName;

    #endregion Fields
}

/// <summary>
/// A user object.
/// </summary>
[System.Serializable]
public class UserObject
{
    #region Fields

    /// <summary>
    /// Non unique user nickname.
    /// </summary>
    public string Nickname;

    /// <summary>
    /// Encrypted password.
    /// </summary>
    public string Password;

    /// <summary>
    /// The public.
    /// </summary>
    public string Public;

    /// <summary>
    /// Identifier for the user.
    /// </summary>
    public string UserID;

    /// <summary>
    /// Unique User Name.
    /// </summary>
    public string Username;

    #endregion Fields
}

[System.Serializable]
public class RootPAGuidelineObject
{
    public PAGuidelineObject[] paguidelines;
}

[System.Serializable]
public class PAGuidelineObject
{
    public string PAGuidelineID;

    public string CourseID;

    public string SubjectType; //ipf, gpf, pf -rd

    public string Parameter;

    public string Delta;

    public string Advice;

}