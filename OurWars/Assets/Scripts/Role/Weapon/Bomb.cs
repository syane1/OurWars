using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Projectile
{
    [SerializeField] float damageRadius = 1f;
    private Vector3 targetPosition;
    private float verticalSpeed;
    private Vector3 moveDirection;
    private float angleSpeed;
    private float angle;
    private float flyTime;
    private bool bombed = false;
    
    override protected void Update()
    {
        if (!bombed) { 
            flyTime += Time.deltaTime;
            float riseSpeed = verticalSpeed + UnityEngine.Physics2D.gravity.y * flyTime;
            transform.Translate(transform.right * ProjectileSpeed * Time.deltaTime, Space.World);
            transform.Translate(transform.up * riseSpeed * Time.deltaTime, Space.World);
        }
    }

    /// <summary>
    /// 炮弹开火
    /// </summary>
    /// <param name="target">目标位置</param>
    /// <param name="trgWeapon">开火武器</param>
    public override void Fire(Vector2 target, Weapon trgWeapon)
    {
        targetPosition = new Vector3(target.x,target.y,0f);
        weapon = trgWeapon;

        float flyxDistance = Vector3.Distance(transform.position, targetPosition);
        float flyTime = flyxDistance / ProjectileSpeed;
        float riseTime, downTime;
        riseTime = downTime = flyTime / 2;
        verticalSpeed = -UnityEngine.Physics2D.gravity.y * riseTime;
        ProjectileSpeed *= Mathf.Sign(targetPosition.x - transform.position.x);
    }

    /// <summary>
    /// 碰撞发生
    /// </summary>
    /// <param name="other"></param>
    override protected void OnTriggerExit2D(Collider2D other)
    {
        List<Life> targetLvies = new List<Life>();
        Life[] existedLives = FindObjectsOfType<Life>();
        if (weapon != null) { 
            foreach (Life targetLife in existedLives)
            {
                //如果是敌对状态
                if ((weapon.getOwner() is Player && targetLife.GetComponent<Role>() is Enemy) || (weapon.getOwner() is Enemy && targetLife.GetComponent<Role>() is Player))
                {
                    //如果距离小于杀伤半径
                    if(Vector3.Distance(gameObject.transform.position , targetLife.gameObject.transform.position)<= damageRadius)
                    { 
                        //扣血
                        targetLife.TakeHealth(damage);
                        //若目标是敌人的话
                        if (targetLife.GetComponent<Role>() is Enemy)
                        {
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
                }
            }
        }
        bombed = true;
        StartCoroutine(dealExplosion());
    }
}
