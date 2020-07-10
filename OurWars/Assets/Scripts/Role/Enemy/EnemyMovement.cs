using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : Movement
{
    /// <summary>
    /// 初始化一些值及校验它们是否为空
    /// </summary>
    override protected void Start()
    {
        base.Start();
        role = GetComponent<Enemy>();
        if (role == null)
        {
            Debug.LogError("获取不到Player/Enemy组件");
        }
        GAME_SESSION = FindObjectOfType<GameSession>();
    }

    #region x轴移动相关
    /// <summary>
    /// HorizontalMove平行移动
    /// </summary>
    override protected void hMove()
    {
        //TODO 重写Enemy的hMovement
        if (role.AI == null || role.AI.getState() == SimpleAI.AIState.Thinking)
        {
            role.myAnimator.SetBool("isRunning", false);
            role.myRigidbody.velocity = new Vector2(0f,role.myRigidbody.velocity.y);
            return;
        }
        Vector2 tmpVelocity = role.myRigidbody.velocity + new Vector2(Mathf.Sign(((Vector3)role.AI.getTarget() - transform.position).x) * hMoveSpeed * Time.deltaTime, 0);
        role.myRigidbody.velocity = new Vector2(Mathf.Clamp(tmpVelocity.x, -hMoveSpeed, hMoveSpeed), tmpVelocity.y);
        if (hHasHorizontalSpeed())
        {
            hFlipSprite();
            role.myAnimator.SetBool("isRunning", true);
        }
        else
        {
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
        if (role.AI == null || role.AI.getState() == SimpleAI.AIState.Thinking)
        {
            role.myAnimator.SetBool("isJumping", false);
            return;
        }
        if (role.myRigidbody == null || role.myAnimator == null)
        {
            Debug.LogError("未找到myPlayer.myRigidbody[Player]或myPlayer.myAnimator[Player]，请检查是否有该对象");
            return;
        }

        EnemyJump();
    }
    /// <summary>
    /// Enemy的跳跃逻辑
    /// </summary>
    void EnemyJump()
    {
        //TODO 实现敌人需要跳跃的功能
        float maxJumpCost = vMoveSpeed;
        //过0.5秒再判断是否进行跳跃处理
        if(Time.time - jumpTimer > 0.5f) { 
            //当ai的状态为跳跃，且脚碰到地面且身体不接触时，则不再尝试
            if (role.AI.getState() == SimpleAI.AIState.Jumping && role.feetCollider.IsTouchingLayers(LayerMask.GetMask("proForeground")))
            {
                //切换到巡逻转态
                role.AI.setState(SimpleAI.AIState.Patrolling);
                if(role.bodyCollider.IsTouchingLayers(LayerMask.GetMask("proForeground")))
                    //并切换巡逻目标点
                    StartCoroutine(role.AI.ChangePatrolTarget());
                jumpTimer = Time.time+role.AI.getThinkingInterval();
                return;
            }
            //如果身体碰到foreground则跳跃
            if (role.bodyCollider.IsTouchingLayers(LayerMask.GetMask("proForeground")))
            {
                if ((vCanVMove()))
                {
                    Jump();
                    role.AI.setState(SimpleAI.AIState.Jumping);
                }
            }
        }
        //隔一定时间x轴无法移动则尝试跳跃
        //再隔一定时间x轴仍无法移动尝试第二次跳跃
        //切换巡逻目标
    }
    #endregion

    #region Climb相关
    override protected void cClimb()
    {
        //TODO 重写Enemy的cClimbing
        if (role.AI == null || role.AI.getState() == SimpleAI.AIState.Thinking) return;
    }
    #endregion
}
