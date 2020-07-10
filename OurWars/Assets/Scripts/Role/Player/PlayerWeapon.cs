using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerWeapon : Weapon
{
    [Tooltip("必需")]
    [SerializeField] protected GameObject Bomb;
    [SerializeField] int bombCostKey = 3;

    override protected void Start()
    {
        base.Start();
        if (Bomb == null) Debug.LogError("Bomb未指定");
        ResetThrowParams();
    }

    private void Update()
    {
        //只有当可以扔炸弹的时候才要计算这些
        if (canThrowBomb()) {
            //增加长度throwDistance并描绘出线段，否则
            if (CrossPlatformInputManager.GetButton("Fire2"))
            {
                throwDistance = Mathf.Clamp(throwDistance + Time.deltaTime * 1f* role.transform.localScale.x, -maxThrowDistance,maxThrowDistance);
                var tmpTarget = new Vector3(transform.position.x + throwDistance, transform.position.y);
                Debug.DrawLine(transform.position, tmpTarget);
            }
            if (CrossPlatformInputManager.GetButtonUp("Fire2"))
            {
                lastBombTimer = Time.time;
                throwTarget = new Vector3(transform.position.x + throwDistance, transform.position.y);
            }
        }
    }

    /// <summary>
    /// Player开枪
    /// </summary>
    /// <param name="direction">开火方向</param>
    override public void Trigger(Vector2 direction)
    {
        //如果不能射击则返回
        if (!canShoot()) return;
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            GameObject Instantiated = Instantiate(Bullet, transform.position, transform.rotation) as GameObject;
            Instantiated.transform.parent = bulletCollecter.transform;
            Instantiated.GetComponent<Projectile>().Fire(direction,this);
            lastShootTimer = Time.time;
            AddBullet(-1);
            AddTrgCount(1);
        }
    }

    /// <summary>
    /// Player开枪
    /// </summary>
    /// <param name="direction">开火方向</param>
    override public void ThrowBomb()
    {
        //如果不能扔炸弹则返回
        if (!canThrowBomb()) return;
        if (Mathf.Abs(throwDistance)>minThrowDistance && throwTarget!=Vector2.zero)
        {
            GameObject Instantiated = Instantiate(Bomb, transform.position, transform.rotation) as GameObject;
            Instantiated.transform.parent = bulletCollecter.transform;
            Instantiated.GetComponent<Bomb>().Fire(throwTarget, this);
            lastBombTimer = Time.time;
            ResetThrowParams();
            role.myPicker.AddCoin(-bombCostKey);
            AddTrgCount(1);
        }
    }

    /// <summary>
    /// 重置扔炸弹的相关参数
    /// </summary>
    override internal void ResetThrowParams()
    {
        throwDistance = role.transform.localScale.x * minThrowDistance;
        throwTarget = Vector2.zero;
        lastBombTimer = Time.time;
    }

    /// <summary>
    /// 是否可以扔炸弹
    /// </summary>
    /// <returns></returns>
    override protected bool canThrowBomb()
    {
        return Bomb != null && role.myPicker.getCoins() > bombCostKey && Time.time - lastBombTimer >= bombInterval;
    }
}
