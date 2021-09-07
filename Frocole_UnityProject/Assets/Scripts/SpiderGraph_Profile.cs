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
using System.Globalization;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A spider graph profile.
/// </summary>
public class SpiderGraph_Profile : Graphic
{
    #region Fields

    /// <summary>
    /// All scores on axis.
    /// </summary>
    public List<float[]> AllScoresOnAxis = new List<float[]>();

    /// <summary>
    /// List of names of the axis.
    /// </summary>
    public string[] AxisNames;

    /// <summary>
    /// List of axis names.
    /// </summary>
    public List<string> AxisNamesList;

    /// <summary>
    /// The feedback source.
    /// </summary>
    public FillProfile.FeedbackSource feedbackSource;

    /// <summary>
    /// The line thickness.
    /// </summary>
    public float LineThickness = 10f;

    /// <summary>
    /// My collected feed back.
    /// </summary>
    public RootFeedBackObject myCollectedFeedBack;

    /// <summary>
    /// My spider graph.
    /// </summary>
    public SpiderGraph MySpiderGraph;

    /// <summary>
    /// The scores.
    /// </summary>
    public int[] Scores;

    /// <summary>
    /// The type.
    /// </summary>
    public ProfileVisualisationType type = ProfileVisualisationType.Fill;

    /// <summary>
    /// The grid scale.
    /// </summary>
    private int gridScale = 100;

    #endregion Fields

    #region Enumerations

    /// <summary>
    /// Values that represent profile visualisation types.
    /// </summary>
    public enum ProfileVisualisationType
    {
        /// <summary>
        /// An enum constant representing the fill option.
        /// </summary>
        Fill,
        /// <summary>
        /// An enum constant representing the line option.
        /// </summary>
        Line
    }

    #endregion Enumerations

    #region Methods

    /// <summary>
    /// Gets latest scores.
    /// </summary>
    public void GetLatestScores()
    {
        AxisNames = MySpiderGraph.AxisNames;
    }

    /// <summary>
    /// Score on axis.
    /// </summary>
    ///
    /// <param name="width">  The width. </param>
    /// <param name="height"> The height. </param>
    /// <param name="angle">  The angle. </param>
    /// <param name="score">  The score. </param>
    ///
    /// <returns>
    /// A Vector3.
    /// </returns>
    public Vector3 ScoreOnAxis(float width, float height, float angle, float score)
    {
        float y = (score / gridScale) * Mathf.Sin(Mathf.Deg2Rad * angle) * width / 2;
        float x = (score / gridScale) * Mathf.Cos(Mathf.Deg2Rad * angle) * height / 2;

        return new Vector3(y, x);
    }

    /// <summary>
    /// Executes the 'populate mesh' action.
    /// </summary>
    ///
    /// <param name="vh"> The vh. </param>
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

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    protected override void Start()
    {
        GetLatestScores();
    }

    /// <summary>
    /// Score on i.
    /// </summary>
    ///
    /// <param name="i"> Zero-based index of the. </param>
    ///
    /// <returns>
    /// A float.
    /// </returns>
    float ScoreOn_i(int i)
    {
        if (i >= Scores.Length)
            return Mathf.Clamp(Scores[i - Scores.Length], 0, 100);

        if (i < 0) return Mathf.Clamp(Scores[Scores.Length + i], 0, 100);

        return Mathf.Clamp(Scores[i], 0, 100);
    }

    #endregion Methods
}