using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SingleTargetTower : MonoBehaviour
{
    [Header("Base Class Fields")]
    [SerializeField] [TagSelector] protected string targetTag = "Enemy";
    [Tooltip("How long this tower will last. Negative = infinite.")]
    [SerializeField] [Min(-1)] protected float lifespan = 30;
    [Tooltip("This tower will attempt to update its target every `retargetInterval` seconds.")]
    [SerializeField] protected float retargetInterval = 0.5f;
    [Tooltip("This tower will act upon its current target every `actInterval` seconds.")]
    [SerializeField] protected float actInterval = 0.25f;
    [Tooltip("How much money the player needs to place this tower.")]
    public int cost = 100;

    [SerializeField] protected Slider healthSlider;
    protected Image healthFill;

    private HashSet<GameObject> targetsInRange;
    protected GameObject currentTarget;

    protected Coroutine expireCorout;
    private Coroutine updateTargetCorout;
    private Coroutine actOnTargetCorout;

    /// <summary>
    /// <b>Arguments:</b><br/>
    /// - <see cref="GameObject"/>: The object being damaged.
    /// - <see cref="float"/>: The amount of damage dealt.
    /// </summary>
    public static Action<GameObject, float> inflictDamage;

    protected virtual void Start()
    {
        targetsInRange = new HashSet<GameObject>();
        healthFill = healthSlider.fillRect.GetComponent<Image>();

        //Use a coroutine instead of Destroy's built in delay param to make it cancellable
        ResetLifespan();
    }

    public void ResetLifespan()
    {
        Coroutilities.TryStopCoroutine(this, ref expireCorout);

        float progress = 0;
        expireCorout = Coroutilities.DoUntil(this,
            () =>
            {
                progress += Time.deltaTime / lifespan;
                healthSlider.value = 1 - progress;
                healthFill.color = Color.Lerp(Color.gray, Color.blue, healthSlider.value / 1);

                if (progress >= 1)
                    Destroy(gameObject);
            },
            () => progress >= 1);
    }

    protected virtual void OnEnable()
    {
        //Do until repeatedly does a thing until the condition it's given is true; here, they will go on forever until
        //stopped in OnDisable.
        updateTargetCorout = Coroutilities.DoUntil(this, UpdateTarget, () => false, retargetInterval, true);
        actOnTargetCorout = Coroutilities.DoUntil(this, TryActOnTarget, () => false, actInterval, true);
    }

    protected virtual void OnDisable()
    {
        Coroutilities.TryStopCoroutine(this, ref updateTargetCorout);
        Coroutilities.TryStopCoroutine(this, ref actOnTargetCorout);
    }

    private void UpdateTarget()
    {
        //RemoveWhere removes all elements that meet a condition; in this case, if
        //target is null/pending destroy
        targetsInRange.RemoveWhere(target => !target);

        //Go through the targets in range (order is not guaranteed because hashsets aren't normally accessed like
        //this; we don't care about order), find the closest target among them, and make that the current target
        currentTarget = null;
        float minDist = Mathf.Infinity;
        foreach (GameObject target in targetsInRange)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                currentTarget = target;
            }
        }
    }

    private void TryActOnTarget()
    {
        if (currentTarget)
        {
            ActOnTarget(currentTarget);
        }
    }

    protected abstract void ActOnTarget(GameObject target);

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
