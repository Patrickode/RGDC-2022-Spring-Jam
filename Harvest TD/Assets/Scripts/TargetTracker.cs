using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTracker : MonoBehaviour
{
    [SerializeField] [TagSelector] private string targetTag;
    [Tooltip("The current target will be updated every `retargetInterval` seconds.")]
    [SerializeField] protected float retargetInterval = 0.5f;

    private HashSet<GameObject> targetsInRange;
    private Coroutine updateTargetCorout;

    public string TargetTag { get => targetTag; }
    public GameObject CurrentTarget { get; private set; }
    public HashSet<GameObject> GetTargetsInRange() => targetsInRange;

    /// <summary>
    /// <b>Arguments:</b><br/>
    /// - <see cref="TargetTracker"/>: The tracker that added a target.<br/>
    /// - <see cref="GameObject"/>: The target that was added.<br/>
    /// </summary>
    public static Action<TargetTracker, GameObject> targetAdded;
    /// <summary>
    /// <b>Arguments:</b><br/>
    /// - <see cref="TargetTracker"/>: The tracker that removed a target.<br/>
    /// - <see cref="GameObject"/>: The target that was removed.<br/>
    /// </summary>
    public static Action<TargetTracker, GameObject> targetRemoved;

    private void OnEnable()
    {
        //Do until repeatedly does a thing until the condition it's given is true; here, it'll go on forever until
        //stopped in OnDisable.
        updateTargetCorout = Coroutilities.DoUntil(this, UpdateTarget, () => false, retargetInterval, true);
    }
    private void OnDisable()
    {
        Coroutilities.TryStopCoroutine(this, ref updateTargetCorout);
    }
    private void UpdateTarget()
    {
        //RemoveWhere removes all elements that meet a condition; in this case, if
        //target is null/pending destroy
        targetsInRange.RemoveWhere(target => !target);

        //Go through the targets in range (order is not guaranteed because hashsets aren't normally accessed like
        //this; we don't care about order), find the closest target among them, and make that the current target
        CurrentTarget = null;
        float minDist = Mathf.Infinity;
        foreach (GameObject target in targetsInRange)
        {
            float dist = (transform.position - target.transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                CurrentTarget = target;
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            targetsInRange.Add(other.gameObject);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        targetsInRange.Remove(other.gameObject);
    }
}
