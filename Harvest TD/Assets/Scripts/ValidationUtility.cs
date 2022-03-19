using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <b>Code Author: cxode, via https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/#post-7841490 </b><br/>
/// Sometimes, when you use Unity's built-in OnValidate, it will spam you with a very annoying warning message,
/// even though nothing has gone wrong.<br/>
/// To avoid this, you can run your OnValidate code through this utility.
/// </summary>
public static class ValidationUtility
{
    /// <summary>
    /// Call this during OnValidate.
    /// Runs <paramref name="onValidateAction"/> once, after all inspectors have been updated.
    /// </summary>
    /// <example><code>
    /// private void OnValidate()
    ///{
    ///    ValidationUtility.SafeOnValidate(() =>
    ///    {
    ///        // Put your OnValidate code here
    ///    });
    ///}
    /// </code></example>
    public static void SafeOnValidate(Action onValidateAction)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += _OnValidate;


        void _OnValidate()
        {
            UnityEditor.EditorApplication.delayCall -= _OnValidate;

            onValidateAction();
        }
#endif
    }
}