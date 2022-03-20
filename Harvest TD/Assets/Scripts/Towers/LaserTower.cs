using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : SingleTargetTower
{
    [Header("Laser Tower Fields")]
    [SerializeField] [Min(0)] protected float damageRate;
    [Space(10)]
    [SerializeField] protected LineRenderer laserLine;
    [SerializeField] protected Vector3 lineOriginOffset;
    [SerializeField] protected Vector3 lineTargetOffset;
    [SerializeField] [Range(0, 0.5f)] private float minLineJitter = 0;
    [SerializeField] [Range(0, 0.5f)] private float maxLineJitter = 0;

    private void OnValidate()
    {
        if (actInterval > 0)
        {
            Debug.LogWarning($"{name} has an act interval greater than zero; Laser towers are balanced " +
                $"around dealing constant damage over time, which requires a small fire rate. Are you sure you want this?");
        }
    }

    protected override void ActOnTarget(GameObject target)
    {
        inflictDamage?.Invoke(target, Time.deltaTime * damageRate);

        Vector3[] pos = new Vector3[laserLine.positionCount];
        Vector3 start = transform.position + lineOriginOffset;
        Vector3 end = target.transform.position + lineTargetOffset;

        for (int i = 0; i < laserLine.positionCount; i++)
        {
            //Jitter code taken from a Past Patrick Project�
            //https://github.com/Patrickode/Livewire-Lifesaver/blob/New-Input/Assets/Scripts/Wire%20%26%20Current/DetectPlayer.cs
            Vector3 jitterModifier = Vector3.zero;
            while (jitterModifier == Vector3.zero)
                jitterModifier = Vector3.Cross(Random.onUnitSphere, end - start);

            jitterModifier.Normalize();
            jitterModifier *= Random.Range(minLineJitter, maxLineJitter);

            pos[i] = Vector3.Lerp(start, end, (1f / laserLine.positionCount) * i) + jitterModifier;
        }
        laserLine.SetPositions(pos);
        laserLine.enabled = true;
    }

    private void Update()
    {
        if (!tTracker.CurrentTarget)
            laserLine.enabled = false;
    }
}