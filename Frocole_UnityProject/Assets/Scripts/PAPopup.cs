using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PAPopup : MonoBehaviour
{

    // SideInAnimation;
    RectTransform _rect;
    float time = 0;
    public AnimationCurve Animation;
    public float AnimTime = 1;

    public PedagogicalAgent PA;

    // content
    public RectTransform _contentRect;
    public Text _subjectText;
    public Text _contentText;

    private void Start()
    {
        _rect = this.GetComponent<RectTransform>();

        _rect.anchorMin = new Vector2(1, 0);

        gameObject.SetActive(true);
        PA.FadeEffect.SetActive(true);

        StartCoroutine(SlideInView());
    }

    IEnumerator SlideInView()
    {
        while (time < AnimTime)
        { 
            time += Time.deltaTime;
            if (time > AnimTime) time = AnimTime;
            _rect.anchorMin = new Vector2(Animation.Evaluate(AnimTime - time), 0);
            yield return null;
        }

        yield return null;
    }

    public void Close()
    {
        GameObject.Destroy(gameObject);
        PA.ActivePopups.Remove(this);
        if (PA.ActivePopups.Count == 0) PA.FadeEffect.SetActive(false);
    }




}
