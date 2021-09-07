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

/// <summary>
/// A startup animation.
/// </summary>
public class StartupAnimation : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// From scale.
    /// </summary>
    public float fromScale;

    /// <summary>
    /// The load scene.
    /// </summary>
    public LoadScene loadScene;

    /// <summary>
    /// To scale.
    /// </summary>
    public float toScale;

    /// <summary>
    /// The total animation time.
    /// </summary>
    public float TotalAnimationTime = 1f;

    /// <summary>
    /// The zoom out animation.
    /// </summary>
    public AnimationCurve ZoomOutAnimation;

    /// <summary>
    /// The zoom out object.
    /// </summary>
    public RectTransform ZoomOutObject;

    /// <summary>
    /// The elapsed time.
    /// </summary>
    private float _elapsedTime = 0f;

    #endregion Fields

    #region Methods

    //public RectTransform PeopleMoveUpObject;
    //public Vector2 frompos;
    //public Vector2 topos;
    //public AnimationCurve PeopleMoveUpAnimation;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        _elapsedTime = Mathf.Clamp(_elapsedTime + Time.deltaTime, 0, TotalAnimationTime);

        ZoomOutObject.localScale = Vector3.one * (toScale + (ZoomOutAnimation.Evaluate(_elapsedTime / TotalAnimationTime) * (fromScale - toScale)));

        if (_elapsedTime == TotalAnimationTime) loadScene.Load();
    }

    #endregion Methods
}