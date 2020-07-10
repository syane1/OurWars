using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : Movement
{
    // 初始化参数及校验配置
    override protected void Start()
    {
        role = GetComponent<Player>();
        if (role == null)
        {
            Debug.LogError("获取不到Player组件");
        }
        GAME_SESSION = FindObjectOfType<GameSession>();
    }

    #region x轴移动相关
    /// <summary>
    /// HorizontalMove平行移动
    /// </summary>
    override protected void hMove()
    {
        float hThrottle = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 tmpVelocity = role.myRigidbody.velocity + new Vector2(hThrottle * hMoveSpeed * Time.deltaTime, 0);
        role.myRigidbody.velocity = new Vector2(Mathf.Clamp(tmpVelocity.x, -hMoveSpeed, hMoveSpeed), tmpVelocity.y);
        if (hHasHorizontalSpeed())
        {
            hFlipSprite();
            role.myAnimator.SetBool("isRunning", true);
        }
        else {
            role.myAnimator.SetBool("isRunning", false);
        }
    }
    #endregion

    #region y轴移动相关(Jump)
    /// <summary>
    /// VerticalMove纵向移动
    /// </summary>
    override protected void vMove()
    {
        //判空以免出现空对象
        if (role.myRigidbody == null|| role.myAnimator == null)
        {
            Debug.LogError("未找到myPlayer.myRigidbody[Player]或myPlayer.myAnimator[Player]，请检查是否有该对象");
            return;
        }
        //仅在可跳跃的环境（可以一次、二次跳跃且按下了Jump键时）
        if ((vCanVMove())&&CrossPlatformInputManager.GetButtonDown("Jump"))//若按下Jump键
        {
            Jump();
        }
    }

    
    #endregion

    #region Climb相关
    override protected void cClimb()
    {
        //判空以免出现空对象
        if (role.myRigidbody == null)
        {
            Debug.LogWarning("未找到myPlayer.myRigidbody[Player]，请检查是否有该对象");
            return;
        }
        //仅在碰到ClimbingLayer图层时处理本函数
        if (cIsTouchingClimbingLayer())
        {
            role.myRigidbody.velocity = new Vector2(role.myRigidbody.velocity.x ,CrossPlatformInputManager.GetAxis("Vertical")*climbSpeed);
        }
    }
    #endregion
}
