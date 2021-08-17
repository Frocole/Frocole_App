using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupGraphFromDB : MonoBehaviour
{
    public SpiderGraph SpiderGraph;
    private UserDataManager _persistentLoginDataManager;


    // Start is called before the first frame update
    void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        SpiderGraph.MyCourseObject = _persistentLoginDataManager.CourseData;
        SpiderGraph.InstantiateAxes();
      
    }

  

}
