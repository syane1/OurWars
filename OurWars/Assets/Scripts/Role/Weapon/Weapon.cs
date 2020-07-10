using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected int bulletNumber = 10;
    [Tooltip("必需")]
    [SerializeField] protected GameObject Bullet;
    [Tooltip("非必需,Enemy的就没有")]
    [SerializeField] protected Text bulletText;
    [Tooltip("每次开枪的间隔")]
    [SerializeField] protected float fireInterval = 0.5f;
    [Tooltip("每次开枪的间隔")]
    [SerializeField] protected float bombInterval = 1.5f;

    protected Role role;
    //父节点
    const string BULLETCOLLECTER_NAME = "BulletCollecter";
    protected GameObject bulletCollecter;
    protected float lastShootTimer;
    protected int bulletsTrgCount=0;
    protected int killedRivalsCnt = 0;
    //炸弹相关
    protected float lastBombTimer;
    protected Vector2 throwTarget = Vector2.zero;
    protected float minThrowDistance = 0.75f;
    protected float maxThrowDistance = 5f;
    protected float throwDistance;

    virtual protected void Start()
    {
        setBulletNumber(bulletNumber);
        if (Bullet == null) Debug.LogError("Bullet未指定");
        role = GetComponentInParent<Role>();
        if (role == null) Debug.LogError("Role未找到");
        bulletCollecter = GameObject.Find(BULLETCOLLECTER_NAME);
        if (bulletCollecter == null) bulletCollecter = new GameObject(BULLETCOLLECTER_NAME);
    }

    /// <summary>
    /// 开枪
    /// </summary>
    /// <param name="direction">开火方向</param>
    virtual public void Trigger(Vector2 direction) { }

    /// <summary>
    /// 扔炸弹
    /// </summary>
    /// <param name="direction"></param>
    virtual public void ThrowBomb() { }

    /// <summary>
    /// 是否可以射击
    /// </summary>
    /// <returns></returns>
    protected bool canShoot()
    {
        return Bullet != null && bulletNumber > 0 && Time.time - lastShootTimer >= fireInterval;
    }

    /// <summary>
    /// 重置炸弹参数
    /// </summary>
    virtual internal void ResetThrowParams()
    {
        throwDistance = role.transform.localScale.x * minThrowDistance;
        throwTarget = Vector2.zero;
        lastBombTimer = Time.time;
    }
    /// <summary>
    /// 是否可以扔炸弹
    /// </summary>
    /// <returns></returns>
    virtual protected bool canThrowBomb(){ return false; }

    #region getter/setter
    /// <summary>
    /// 获取子弹数
    /// </summary>
    /// <returns></returns>
    internal int getBulletNumber()
    {
        return bulletNumber;
    }

    /// <summary>
    /// 子弹数增加
    /// </summary>
    /// <param name="bulletToAdd"></param>
    internal void AddBullet(int bulletToAdd)
    {
        setBulletNumber(bulletNumber + bulletToAdd);
    }

    /// <summary>
    /// 设置子弹数量
    /// </summary>
    /// <param name="bulletNumber"></param>
    internal void setBulletNumber(int bulletNumber)
    {
        this.bulletNumber = bulletNumber;
        if(bulletText != null) bulletText.text = this.bulletNumber.ToString();
    }

    /// <summary>
    /// 获取开火次数
    /// </summary>
    /// <returns></returns>
    internal int getTrgCount()
    {
        return bulletsTrgCount;
    }

    /// <summary>
    /// 增加开火次数
    /// </summary>
    /// <param name="trgNumberToAdd"></param>
    internal void AddTrgCount(int trgNumberToAdd)
    {
        setTrgCount(bulletsTrgCount + trgNumberToAdd);
    }

    /// <summary>
    /// 设置开火次数
    /// </summary>
    /// <param name="bulletTrgCount"></param>
    internal void setTrgCount(int bulletTrgCount)
    {
        this.bulletsTrgCount = bulletTrgCount;
    }

    /// <summary>
    /// 获取杀敌数
    /// </summary>
    /// <returns></returns>
    internal int getKilledRival()
    {
        return killedRivalsCnt;
    }

    /// <summary>
    /// 设置杀敌数
    /// </summary>
    /// <param name="killedRival">待设置的杀敌数</param>
    internal void setKilledRival(int killedRival)
    {
        this.killedRivalsCnt = killedRival;
    }

    /// <summary>
    /// 获取持有者
    /// </summary>
    internal Role getOwner()
    {
        return role;
    }
    #endregion
}
