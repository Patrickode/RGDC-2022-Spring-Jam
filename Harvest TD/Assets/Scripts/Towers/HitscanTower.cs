using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanTower : SingleTargetTower
{
    [Header("Hitscan Tower Fields")]
    [SerializeField] [Min(0)] protected float damage;
    [Space(10)]
    [SerializeField] protected LineRenderer shootLine;
    [SerializeField] protected Vector3 lineOriginOffset;
    [SerializeField] protected Vector3 lineTargetOffset;
    [SerializeField] [Min(0)] protected float shootDuration;

    protected override void ActOnTarget(GameObject target)
    {
        inflictDamage?.Invoke(target, damage);

        Vector3[] pos = new Vector3[shootLine.positionCount];
        for (int i = 0; i < shootLine.positionCount; i++)
        {
            pos[i] = Vector3.Lerp(
                transform.position + lineOriginOffset,
                target.transform.position + lineTargetOffset,
                (1f / shootLine.positionCount) * i);
        }
        shootLine.SetPositions(pos);
        shootLine.enabled = true;
        Coroutilities.DoAfterDelay(this, () => shootLine.enabled = false, shootDuration);
    }


}
