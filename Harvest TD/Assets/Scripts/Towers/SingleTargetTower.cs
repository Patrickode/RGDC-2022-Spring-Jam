using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SingleTargetTower : MonoBehaviour
{
    [Header("Base Class Fields")]
    [SerializeField] protected TargetTracker tTracker;
    [Tooltip("How long this tower will last. Negative = infinite.")]
    [SerializeField] [Min(-1)] protected float lifespan = 30;
    [Tooltip("This tower will act upon its current target every `actInterval` seconds.")]
    [SerializeField] protected float actInterval = 0.25f;
    [Tooltip("How much money the player needs to place this tower.")]
    public int cost = 100;

    [SerializeField] protected Slider expireSlider;
    protected Image expSliderFill;

    protected Coroutine expireCorout;
    protected Coroutine actOnTargetCorout;

    /// <summary>
    /// <b>Arguments:</b><br/>
    /// - <see cref="GameObject"/>: The object being damaged.
    /// - <see cref="float"/>: The amount of damage dealt.
    /// </summary>
    public static Action<GameObject, float> inflictDamage;

    protected virtual void Start()
    {
        if (!tTracker)
            Debug.LogError("SingleTargetTower {name} needs a TargetTracker to target things!");

        expSliderFill = expireSlider.fillRect.GetComponent<Image>();
        ResetLifespan();
    }

    public void ResetLifespan()
    {
        //We use a coroutine instead of Destroy's built in delay param to make the destruction cancellable
        Coroutilities.TryStopCoroutine(this, ref expireCorout);

        float progress = 0;
        expireCorout = Coroutilities.DoUntil(this,
            () =>
            {
                progress += Time.deltaTime / lifespan;
                expireSlider.value = progress;
                expSliderFill.color = Color.Lerp(Color.gray, Color.blue, expireSlider.value / 1);

                if (progress >= 1)
                    Destroy(gameObject);
            },
            () => progress >= 1);
    }

    protected virtual void OnEnable()
    {
        //Do until repeatedly does a thing until the condition it's given is true; here, it'll go on forever until
        //stopped in OnDisable.
        actOnTargetCorout = Coroutilities.DoUntil(this, TryActOnTarget, () => false, actInterval, true);
    }

    protected virtual void OnDisable()
    {
        Coroutilities.TryStopCoroutine(this, ref actOnTargetCorout);
    }

    private void TryActOnTarget()
    {
        if (tTracker.CurrentTarget)
            ActOnTarget(tTracker.CurrentTarget);
    }
    protected abstract void ActOnTarget(GameObject target);
}