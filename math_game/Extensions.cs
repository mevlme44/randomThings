﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Extensions
{
    #region LookAt2D
    public static void LookAt2D(this Transform me, Vector3 target, Vector3? eye = null)
    {
        float signedAngle = Vector2.SignedAngle(eye ?? me.gameObject.GetComponent<Citizen>().GetNormal(), target - me.position);

        if (Mathf.Abs(signedAngle) >= 1e-3f)
        {
            var angles = me.eulerAngles;
            angles.z += signedAngle;
            me.eulerAngles = angles;
        }
    }
    public static void LookAt2D(this Transform me, Transform target, Vector3? eye = null)
    {
        me.LookAt2D(target.position, eye);
    }
    public static void LookAt2D(this Transform me, GameObject target, Vector3? eye = null)
    {
        me.LookAt2D(target.transform.position, eye);
    }
    #endregion
}
