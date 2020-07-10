using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Role
{

    override protected void Start()
    {
        base.Start();
        InitialParams();
        AI = GetComponent<SimpleAI>();
        if (AI == null)
        {
            Debug.LogError("无法获取SimpleAI组件");
            return;
        }
    }


    /// <summary>
    /// 初始化参数
    /// </summary>
    virtual protected void InitialParams()
    {
        base.InitialParams();
        myMovement = GetComponent<EnemyMovement>();
        if (myMovement == null)
        {
            Debug.LogError("未找到EnemyMovement组件");
        }
        myWeapon = GetComponentInChildren<EnemyWeapon>();
        if (myWeapon == null)
        {
            Debug.LogError("未找到EnemyWeapon组件");
        }
    }
}
