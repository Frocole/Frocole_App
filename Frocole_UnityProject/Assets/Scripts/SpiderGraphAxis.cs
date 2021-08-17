using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderGraphAxis : MonoBehaviour
{
    public Text SpidergraphAxisName;
    public int AxisID;
    public Button InteractionButton;
    public RectTransform TextBackgroundPivot;

    public float MyAngle;

    public FeedbackSubmitter feedbackSubmitter;
    public RectTransform TextBackground;

    public Image AxisHead;


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
                TextBackground.sizeDelta = new Vector3(300/*SpidergraphAxisName.preferredWidth*/, SpidergraphAxisName.preferredHeight + 20);
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

}
