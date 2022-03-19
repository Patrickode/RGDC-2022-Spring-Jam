using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilFuncs
{
    /// <summary>
    /// Takes in any number of keys and returns the first one of them that's down, in the order they were passed.<br/>
    /// This allows you to use a switch statement with <paramref name="keyDown"/> instead of a ton of else-ifs.<br/><br/>
    /// <i>Originally made for 603 Game 3 by Patrick Mitchell.</i>
    /// </summary>
    /// <param name="keyDown">The first key in <paramref name="codes"/> that was found to be down, in the order they were passed.<br/>
    /// Equals <see cref="KeyCode.None"/> if none of them were found to be down.</param>
    /// <param name="codes">The keys you want to check. Will return the first one down, in the order you pass them.</param>
    /// <returns>Whether any of the keys in <paramref name="codes"/> was down or not.</returns>
    public static bool GetMultiKeyDown(out KeyCode keyDown, params KeyCode[] codes)
    {
        for (int i = 0; i < codes.Length; i++)
        {
            if (Input.GetKeyDown(codes[i]))
            {
                keyDown = codes[i];
                return true;
            }
        }

        keyDown = KeyCode.None;
        return false;
    }

    /// <summary>
    /// Divides two vectors component-wise.
    /// </summary>
    public static Vector3 InverseScale(Vector3 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);

    /// <summary>
    /// Scales this transform so that it's sized as if its parent had a scale of (1,1,1).
    /// </summary>
    public static void NegateParentScale(this Transform tform)
    {
        tform.localScale = InverseScale(tform.localScale, tform.parent.localScale);
    }
}