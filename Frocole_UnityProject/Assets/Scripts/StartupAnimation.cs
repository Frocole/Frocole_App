using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupAnimation : MonoBehaviour
{
    public LoadScene loadScene;
    private float _elapsedTime = 0f;
    public float TotalAnimationTime = 1f;


    public RectTransform ZoomOutObject;
    public float fromScale;
    public float toScale;
    public AnimationCurve ZoomOutAnimation;

    //public RectTransform PeopleMoveUpObject;
    //public Vector2 frompos;
    //public Vector2 topos;
    //public AnimationCurve PeopleMoveUpAnimation;

    private void Update()
    {


        _elapsedTime = Mathf.Clamp(_elapsedTime + Time.deltaTime, 0, TotalAnimationTime);

        ZoomOutObject.localScale = Vector3.one * (toScale + (ZoomOutAnimation.Evaluate(_elapsedTime / TotalAnimationTime) * (fromScale - toScale)));

        if (_elapsedTime == TotalAnimationTime) loadScene.Load();
    }


}
