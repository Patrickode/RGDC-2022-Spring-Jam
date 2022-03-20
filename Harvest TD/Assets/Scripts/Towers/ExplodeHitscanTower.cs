using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeHitscanTower : HitscanTower
{
    [Header("Explode Fields")]
    [SerializeField] private ParticleSystem explosionPrefab;
    [SerializeField] [Min(0)] private float splashRadius;
    [SerializeField] private float splashDamageModifier;

    protected override void ActOnTarget(GameObject target)
    {
        base.ActOnTarget(target);

        Collider[] splashHits = Physics.OverlapSphere(
            target.transform.position,
            splashRadius,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore);

        foreach (Collider hit in splashHits)
        {
            if (hit != target && hit.gameObject && hit.CompareTag(tTracker.TargetTag))
            {
                inflictDamage?.Invoke(hit.gameObject, damage + splashDamageModifier);
            }
        }

        ParticleSystem explosion = Instantiate(
            explosionPrefab,
            target.transform.position + lineTargetOffset,
            Quaternion.identity);

        //Increase the scale of the boom so it matches the splash radius better
        explosion.transform.localScale = Vector3.one * (splashRadius / 2);
        //https://youtu.be/DyN7G2ad5jw
        explosion.emission.SetBurst(0, new ParticleSystem.Burst(0,
            (short)(explosion.emission.GetBurst(0).count.constant * explosion.transform.localScale.x * 0.75f)));
    }
}
