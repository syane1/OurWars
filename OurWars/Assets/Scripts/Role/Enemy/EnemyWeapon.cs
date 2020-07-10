using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : Weapon
{
    /// <summary>
    /// Enemy开枪
    /// </summary>
    /// <param name="transform">开火位置</param>
    /// <param name="direction">开火方向</param>
    override public void Trigger(Vector2 direction)
    {
        //如果不能射击则返回
        if (!canShoot()) return;

        Debug.DrawLine(transform.position, GetComponentInParent<SimpleAI>().getTarget(), Color.gray, 0.5f);
        GameObject Instantiated = Instantiate(Bullet, transform.position, transform.rotation) as GameObject;
        Instantiated.transform.parent = bulletCollecter.transform;
        Instantiated.GetComponent<Projectile>().Fire(direction,this);
        lastShootTimer = Time.time;
        AddBullet(-1);
        AddTrgCount(1);
    }
}
