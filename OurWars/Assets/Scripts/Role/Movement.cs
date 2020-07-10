using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //配置
    [SerializeField] protected float hMoveSpeed = 5f;
    [SerializeField] protected float vMoveSpeed = 5f;
    [SerializeField] protected float climbSpeed = 3f;
    [SerializeField] protected float jumpInterval = 0.25f;

    //参数
    protected GameSession GAME_SESSION;
    protected Role role;
    /// <summary>
    /// 是否可以二次跳跃，默认为true
    /// </summary>
    protected bool canJump2nd = true;
    /// <summary>
    /// 用于记录起跳时间
    /// </summary>
    protected float jumpTimer;

    // 初始化参数及校验配置
    virtual protected void Start()
    {
        role = GetComponent<Role>();
        if (role == null)
        {
            Debug.LogError("获取不到Player/Enemy组件");
        }
        GAME_SESSION = FindObjectOfType<GameSession>();
    }

    private void Update()
    {
        if(GAME_SESSION==null) GAME_SESSION = FindObjectOfType<GameSession>();
    }

    /// <summary>
    /// 角色动作相关(x轴，y轴，攀爬)
    /// </summary>
    virtual internal void Move()
    {
        hMove();
        vMove();
        cClimb();
    }

    #region x轴移动相关
    /// <summary>
    /// HorizontalMove平行移动
    /// </summary>
    virtual protected void hMove()
    {
    }
    /// <summary>
    /// 翻转Sprite
    /// </summary>
    virtual protected void hFlipSprite()
    {
        //只有在他们不相等的时候才进行翻转
        if (role!=null && GetComponent<Transform>().localScale.x != Mathf.Sign(role.myRigidbody.velocity.x))
        {
            GetComponent<Transform>().localScale
                = new Vector2(Mathf.Sign(role.myRigidbody.velocity.x), 1f);//取横向速度的符号(1/-1)作为Scale的值达到翻转Sprite的目的
            role.myWeapon.ResetThrowParams();
        }
    }
    /// <summary>
    /// 是否有横向速度
    /// </summary>
    /// <returns></returns>
    virtual protected bool hHasHorizontalSpeed()
    {
        //获取Rigidbody2D的x速度与Epsilon作比较，大于则代表有速度
        return Mathf.Abs(role.myRigidbody.velocity.x) > 0.1f;
    }
    #endregion

    #region y轴移动相关(Jump)
    /// <summary>
    /// VerticalMove纵向移动
    /// </summary>
    virtual protected void vMove()
    {
    }
    /// <summary>
    /// 是否有纵向速度
    /// </summary>
    /// <returns></returns>
    virtual protected bool vHasVerticalSpeed()
    {
        return Mathf.Abs(role.myRigidbody.velocity.y) > Mathf.Epsilon;
    }
    /// <summary>
    /// 是否可以纵向移动
    /// </summary>
    /// <returns>可以/不能</returns>
    virtual protected bool vCanVMove()
    {
        //判空以免出现空对象
        if (GAME_SESSION == null || role.myAnimator == null || role.feetCollider == null)
        {
            Debug.LogWarning("未找到GameSession或myPlayer.myAnimator[Player]或myPlayer.feetCollider[Player]，请检查是否有该对象");
            return false;
        }
        //在判断是否可跳跃的前提下对一二次跳跃状态进行设置
        if (role.feetCollider.IsTouchingLayers(LayerMask.GetMask("proForeground")) && Time.time > jumpTimer + jumpInterval)
        {
            if (role.myAnimator.GetBool("isJumping")) role.myAnimator.SetBool("isJumping", false);
            if (!canJump2nd) canJump2nd = true;
        }
        //剖面游戏时在非跳跃状态状态下可跳跃，且在跳跃间隔过后才可跳跃
        //          鸟瞰游戏时y轴不受限
        return (GAME_SESSION.GetSessionInfo().gameType == GameSession.GameType.PROFILE_VIEW && (!role.myAnimator.GetBool("isJumping") || canJump2nd))
                || GAME_SESSION.GetSessionInfo().gameType == GameSession.GameType.AERIAL_VIEW;
    }
    /// <summary>
    /// 跳
    /// </summary>
    virtual protected void Jump()
    {
        role.myRigidbody.velocity += new Vector2(0f, vMoveSpeed * Time.timeScale);
        //设置跳跃状态
        if (role.myAnimator.GetBool("isJumping"))
        {
            canJump2nd = false;
        }
        else
        {
            role.myAnimator.SetBool("isJumping", true);
        }
        jumpTimer = Time.time;
    }
    #endregion

    #region Climb相关
    virtual protected void cClimb()
    {
    }
    /// <summary>
    /// 是否碰到Climb层
    /// 根据Climbing层判断是否需要设置重力为0
    /// </summary>
    virtual protected bool cIsTouchingClimbingLayer()
    {
        //判空以免出现空对象
        if (role.feetCollider == null || role.myAnimator == null || role.myRigidbody == null)
        {
            Debug.LogWarning("未找到myPlayer.feetCollider[Player]或myPlayer.myAnimator[Player]或myPlayer.myRigidbody[Player]，请检查是否有该对象");
            return false;
        }
        bool isTouched = role.feetCollider.IsTouchingLayers(LayerMask.GetMask("proClimbing")) || role.feetCollider.IsTouchingLayers(LayerMask.GetMask("aerClimbing"));
        if (!isTouched)
        {
            role.myAnimator.SetBool("isClimbing", false);
            role.myRigidbody.gravityScale = 1f;//重力设为0
        }
        return isTouched;
    }
    #endregion
}
