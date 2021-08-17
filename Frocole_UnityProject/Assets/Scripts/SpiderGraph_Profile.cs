using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class SpiderGraph_Profile : Graphic
{
    public int[] Scores;
    private int gridScale = 100;
    public ProfileVisualisationType type = ProfileVisualisationType.Fill;
    public FillProfile.FeedbackSource feedbackSource;
    public SpiderGraph MySpiderGraph;

    public float LineThickness = 10f;
    public enum ProfileVisualisationType
    {
        Fill,
        Line
    }



    protected override void Start()
    {        
        GetLatestScores();
    }

    public void GetLatestScores()
    {
        AxisNames = MySpiderGraph.AxisNames;
    }



    public RootFeedBackObject myCollectedFeedBack;
    public List<float[]> AllScoresOnAxis = new List<float[]>();
    public List<string> AxisNamesList;
    public string[] AxisNames;



    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        if (Scores == null || Scores.Length == 0) return;

        float angle = 360f / (float)Scores.Length;

        switch (type)
        {
            case ProfileVisualisationType.Fill:
                // vertex 0 = graph center
                vertex.position = new Vector3(0, 0);
                vh.AddVert(vertex);

                for (int i = 0; i < Scores.Length; i++)
                {
                    vertex.position = ScoreOnAxis(width, height, angle * i, Mathf.Clamp(Scores[i], 0, 100)); 
                    vh.AddVert(vertex);
                }

                for (int i = 1; i < Scores.Length; i++)
                {
                    vh.AddTriangle(0, i, i + 1);
                }

                vh.AddTriangle(0, Scores.Length, 1);

                break;

            case ProfileVisualisationType.Line:
                // vertex 0 = graph center

                Vector3 ScorePos;
                Vector3 LCapScorePos;
                Vector3 RCapScorePos;


                for (int i = 0; i < Scores.Length; i++)
                {
                    ScorePos = ScoreOnAxis(width, height, angle * i, Mathf.Clamp(Scores[i], 0, 100));
                    ScorePos = ScorePos.normalized * (ScorePos.magnitude - 0.5f * LineThickness);

                    LCapScorePos = (ScoreOnAxis(width, height, angle * (i - 1), ScoreOn_i(i - 1)) - ScorePos).normalized;
                    RCapScorePos = (ScoreOnAxis(width, height, angle * (i + 1), ScoreOn_i(i + 1)) - ScorePos).normalized;

                    LCapScorePos = new Vector3(LCapScorePos.y, -LCapScorePos.x) * LineThickness;
                    RCapScorePos = new Vector3(-RCapScorePos.y, RCapScorePos.x) * LineThickness;

                    LCapScorePos += ScorePos;
                    RCapScorePos += ScorePos;

                    vertex.position = ScorePos;
                    vh.AddVert(vertex);
                    vertex.position = LCapScorePos;
                    vh.AddVert(vertex);
                    vertex.position = RCapScorePos;
                    vh.AddVert(vertex);

                    int j = i * 3;
                    vh.AddTriangle(j + 0, j + 1, j + 2);
                }

                for (int i = 0; i < Scores.Length - 1; i++)
                {
                    vh.AddTriangle(i * 3, i * 3 + 2, (i + 1) * 3);
                    vh.AddTriangle((i + 1) * 3 + 1, i * 3 + 2, (i + 1) * 3);
                }
                vh.AddTriangle((Scores.Length - 1) * 3, (Scores.Length - 1) * 3 + 2, 0);
                vh.AddTriangle(1, (Scores.Length - 1) * 3 + 2, 0);

                break;
        }
    }

    public Vector3 ScoreOnAxis(float width, float height, float angle, float score)
    {

        float y = (score / gridScale) * Mathf.Sin(Mathf.Deg2Rad * angle) * width / 2;
        float x = (score / gridScale) * Mathf.Cos(Mathf.Deg2Rad * angle) * height / 2;

        return new Vector3(y, x);
    }

    float ScoreOn_i(int i)
    {
        if (i >= Scores.Length) 
            return Mathf.Clamp(Scores[i - Scores.Length], 0, 100);
    
        if (i < 0) return Mathf.Clamp(Scores[Scores.Length + i], 0, 100);
    
        return Mathf.Clamp(Scores[i], 0, 100);
    }
}
