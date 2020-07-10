using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] int damageValue = 100;
    [SerializeField] bool isContinuous = false;
    [SerializeField] float damageWaitTime = 2f;

    private void OnTriggerEnter2D(Collider2D playerBody)
    {
        if(playerBody is CapsuleCollider2D) { 
            Life damageTarget = playerBody.GetComponentInParent<Life>();
            if (damageTarget == null) return;
            if(isContinuous)
                StartCoroutine(TakeDamage(damageTarget));
            else
            { 
                damageTarget.TakeHealth(damageValue);
            }
        }
    }
    /// <summary>
    /// 连续伤害
    /// </summary>
    /// <param name="damageTarget">伤害目标</param>
    /// <returns>等待时间</returns>
    IEnumerator TakeDamage(Life damageTarget)
    {
        yield return new WaitForSeconds(damageWaitTime);
        damageTarget.ContinuousDamage(damageValue);
    }
    /// <summary>
    /// 用来中止TakeDamage
    /// </summary>
    /// <param name="roleBody">伤害来源的身体</param>
    private void OnTriggerExit2D(Collider2D roleBody)
    {
        if (roleBody is CapsuleCollider2D)
        {
            StopAllCoroutines();
            Life damageTarget = roleBody.GetComponentInParent<Life>();
            if (damageTarget == null) return;
            damageTarget.StopContinuousDamage();
        }
        if (!isContinuous) Destroy(gameObject);
    }
}
