#region Header

/*
    Feedback and Reflection in Online Collaborative Learning.

    Copyright (C) 2021  Open University of the Netherlands

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

#endregion Header

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A spider graph axis.
/// </summary>
public class SpiderGraphAxis : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The axis head.
    /// </summary>
    public Image AxisHead;

    public Image MarkedByPA;


    /// <summary>
    /// Identifier for the axis.
    /// </summary>
    public int AxisID;

    /// <summary>
    /// The feedback submitter.
    /// </summary>
    public FeedbackSubmitter feedbackSubmitter;

    /// <summary>
    /// The interaction control.
    /// </summary>
    public Button InteractionButton;

    /// <summary>
    /// My angle.
    /// </summary>
    public float MyAngle;

    /// <summary>
    /// Name of the spidergraph axis.
    /// </summary>
    public Text SpidergraphAxisName;

    /// <summary>
    /// The text background.
    /// </summary>
    public RectTransform TextBackground;

    /// <summary>
    /// The text background pivot.
    /// </summary>
    public RectTransform TextBackgroundPivot;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Allign text.
    /// </summary>
    public void AllignText()
    {
        int mod = 1;
        if (!(MyAngle < -90 && MyAngle > -270))
        {
            mod = 1;
            TextBackgroundPivot.GetComponent<RectTransform>().rotation = Quaternion.identity;
        }
        else
        {
            mod = -1;
            TextBackgroundPivot.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 180);
        }

        TextBackground.GetComponent<RectTransform>().rotation = Quaternion.identity;

        if (MyAngle < -45 && MyAngle > -135)
        {
            SpidergraphAxisName.alignment = TextAnchor.MiddleRight;
            SpidergraphAxisName.horizontalOverflow = HorizontalWrapMode.Wrap;
            SpidergraphAxisName.verticalOverflow = VerticalWrapMode.Truncate;
            //TextBackground.a
            SpidergraphAxisName.rectTransform.sizeDelta = new Vector3(250, 100);
            //SpidergraphAxisName.rectTransform.localPosition -= new Vector3(30, 0);
            TextBackground.GetComponent<RectTransform>().localPosition -= new Vector3(30 * mod, 0);
            if (MyAngle < -80 && MyAngle > -100)
            {
                SpidergraphAxisName.rectTransform.sizeDelta = new Vector3(250, 100);
                //SpidergraphAxisName.rectTransform.localPosition -= new Vector3(50, 0);
                TextBackground.GetComponent<RectTransform>().localPosition -= new Vector3(50, 0);
                TextBackground.sizeDelta = new Vector3(/*300/**/SpidergraphAxisName.preferredWidth/**/, SpidergraphAxisName.preferredHeight + 20);
            }
        }else
        if (MyAngle < -225 && MyAngle > -315)
        {
            SpidergraphAxisName.alignment = TextAnchor.MiddleLeft;
            SpidergraphAxisName.horizontalOverflow = HorizontalWrapMode.Wrap;
            SpidergraphAxisName.verticalOverflow = VerticalWrapMode.Truncate;

            SpidergraphAxisName.rectTransform.sizeDelta = new Vector3(250, 100);
            //SpidergraphAxisName.rectTransform.localPosition -= new Vector3(-30, 0);
            TextBackground.GetComponent<RectTransform>().localPosition -= new Vector3(-30 * mod, 0);

            if (MyAngle < -260 && MyAngle > -280)
            {
                SpidergraphAxisName.rectTransform.sizeDelta = new Vector3(250, 100);
                //SpidergraphAxisName.rectTransform.localPosition -= new Vector3(-50, 0);
                TextBackground.GetComponent<RectTransform>().localPosition -= new Vector3(-50, 0);
                TextBackground.sizeDelta = new Vector3(300/*SpidergraphAxisName.preferredWidth*/, SpidergraphAxisName.preferredHeight + 20);
            }

        }else

        TextBackground.sizeDelta = new Vector3(SpidergraphAxisName.preferredWidth + 4, SpidergraphAxisName.preferredHeight + 4);
    }

    /// <summary>
    /// Executes the 'click axis' action.
    /// </summary>
    public void OnClickAxis()
    {
        if (feedbackSubmitter.isActiveAndEnabled)
        {
            feedbackSubmitter.Submitscore();
            feedbackSubmitter.HighlightAxis(AxisHead);
        }

        feedbackSubmitter.ActiveParameterName = SpidergraphAxisName.text;
        feedbackSubmitter.ParameterName.text = SpidergraphAxisName.text + " : " + Mathf.Clamp(feedbackSubmitter.Profile_UserReview.Scores[AxisID], 0, 100) + "%";
        feedbackSubmitter.ActiveParameter = AxisID;
        feedbackSubmitter.slider.SetValueWithoutNotify(Mathf.Clamp(feedbackSubmitter.Profile_UserReview.Scores[AxisID],0,100));
        feedbackSubmitter.gameObject.SetActive(true);
    }

    #endregion Methods
}