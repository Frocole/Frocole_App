using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpiderGraph : MonoBehaviour
{
    public bool Teacher;
    public UnityEvent IfViewerOnly;

    public enum FeedbackType
    {
        GPF_RD,
        IPF_RD
    };

    public CourseObject MyCourseObject;
    public FeedbackType MyFeedbackType = FeedbackType.GPF_RD;


    public int Axis_count = 3;
    public int Axis_scale = 100;

    public bool TestRefresh = false;

    public SpiderGraphAxis[] spiderGraphAxes;
    public string[] AxisNames;

    public UnityEvent OnReady;

    private UserDataManager _persistentLoginDataManager;


    void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        MyFeedbackType = _persistentLoginDataManager.SubjectData.FeedbackType;
        MyCourseObject = _persistentLoginDataManager.CourseData;
        InstantiateAxes();
    }

    private void Update()
    {
        if (TestRefresh)
        {
            TestRefresh = false;
            InstantiateAxes();
        }
    }

    public void InstantiateAxes()
    {
        switch (MyFeedbackType)
        {
            case FeedbackType.GPF_RD:
                AxisNames = MyCourseObject.GPF_RD_parameters.Split('/');

                break;
            case FeedbackType.IPF_RD:

                AxisNames = MyCourseObject.IPF_RD_parameters.Split('/');
                break;
        }




        Axis_count = Mathf.Clamp(AxisNames.Length, 3, 10);



        foreach (SpiderGraphAxis item in spiderGraphAxes)
        {
            item.gameObject.SetActive(false);
        }

        float angle = 360f / (float)Axis_count;

        for (int i = 0; i < Axis_count; i++)
        {
            spiderGraphAxes[i].AxisID = i;
            spiderGraphAxes[i].SpidergraphAxisName.text = AxisNames[i];
            spiderGraphAxes[i].gameObject.SetActive(true);
            spiderGraphAxes[i].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, -angle * i);
            spiderGraphAxes[i].MyAngle = -angle * i;

            spiderGraphAxes[i].AllignText();
        }

        if (Teacher && (MyFeedbackType == FeedbackType.IPF_RD || MyFeedbackType == FeedbackType.GPF_RD))
        {
            foreach (var item in spiderGraphAxes)
            {
                item.InteractionButton.interactable = false;
            }
            IfViewerOnly.Invoke();

        }

        OnReady.Invoke();
    }



}
