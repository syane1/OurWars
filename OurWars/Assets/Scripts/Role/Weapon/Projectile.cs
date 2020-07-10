using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected float ProjectileSpeed = 1f;
    [SerializeField] protected int damage = 100;
    protected Vector2 ProjectileDirection;
    protected Weapon weapon;

    virtual protected void Update()
    {
        transform.Translate(ProjectileDirection * ProjectileSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 驱动projectile移动
    /// </summary>
    /// <param name="direction"></param>
    virtual public void Fire(Vector2 direction,Weapon trgWeapon)
    {
        ProjectileDirection = direction;
        weapon = trgWeapon;
    }

    virtual protected void OnTriggerExit2D(Collider2D other)
    {
        Life targetLife = other.gameObject.GetComponent<Life>();
        if (targetLife != null)
        {
            targetLife.TakeHealth(damage);
            //若目标是敌人的话
            if (targetLife.GetComponent<Role>() is Enemy) {
                //则增加子弹射击的数量
                if (weapon != null)
                {
                    if (!targetLife.isAlive())
                    {
                        weapon.AddTrgCount(1);
                    }
                }
                else
                {
                    Debug.Log("获得不到weapon");
                }
            }
        }
        StartCoroutine(dealExplosion());
    }

    //处理爆炸Animation
    protected IEnumerator dealExplosion()
    {
        GetComponent<Animator>().SetTrigger("trgExplode");
        if(GetComponent<CircleCollider2D>()!=null)GetComponent<CircleCollider2D>().enabled = false;
        ProjectileDirection = new Vector2(0f, 0f);
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);

    }
}
