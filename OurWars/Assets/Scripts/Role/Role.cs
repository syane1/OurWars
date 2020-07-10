using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role : MonoBehaviour
{
    //配置

    //参数
    public SimpleAI AI { get; set; }
    internal Animator myAnimator { get; set; }
    internal BoxCollider2D feetCollider { get; set; }
    internal CapsuleCollider2D bodyCollider { get; set; }
    internal Rigidbody2D myRigidbody { get; set; }
    internal Weapon myWeapon { get; set; }
    internal Life myLife;
    internal Picker myPicker;
    protected Movement myMovement;

    // Start is called before the first frame update
    virtual protected void Start()
    {
        //可以实现一些Role的共性初始化等等
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        if (!myLife.isAlive()) return;
        myMovement.Move();
    }

    /// <summary>
    /// 初始化参数
    /// </summary>
    virtual protected void InitialParams()
    {
        feetCollider = GetComponent<BoxCollider2D>();
        if (feetCollider == null)
        {
            Debug.LogError("未找到feetCollider组件");
        }
        bodyCollider = GetComponent<CapsuleCollider2D>();
        if (bodyCollider == null)
        {
            Debug.LogError("未找到bodyCollider组件");
        }
        myRigidbody = GetComponent<Rigidbody2D>();
        if (myRigidbody == null)
        {
            Debug.LogError("未找到Rigidbody2D组件");
        }
        myAnimator = GetComponent<Animator>();
        if (myAnimator == null)
        {
            Debug.LogError("未找到Animator组件");
        }
        myLife = GetComponent<Life>();
        if (myLife == null)
        {
            Debug.LogError("未找到Life组件");
        }
    }

    #region 提供给Animation的状态设置
    /// <summary>
    /// 设置isHurting为false
    /// </summary>
    public void SetIsHurtingFalse()
    {
        myAnimator.SetBool("isHurting", false);
    }
    /// <summary>
    /// 设置isHurting为false
    /// </summary>
    public void SetIsShootingFalse()
    {
        myAnimator.SetBool("isShooting", false);
    }
    #endregion
}
