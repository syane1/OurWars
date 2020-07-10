using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    //配置
    [Range(1, 500)]
    [SerializeField] float fullHealth = 100;
    [SerializeField] Slider healthSlider;
    [SerializeField] float deathDelay = 2f;
    [Tooltip("Buff(PercentValue,Max = 99%,LagerValueLessDamage)")]
    [Range(0,99)][SerializeField] internal float buffPercent = 0;
    [Tooltip("Debuff(PercentValue,Max = 99%,LagerValueMoreDamage)")]
    [Range(0,99)][SerializeField] internal float debuffPercent = 0;
    [Tooltip("ContinuouslyIncreaseHealth,Max = 50")]
    [Range(0, 50)] [SerializeField] float incHealthContinuous = 0;
    [Tooltip("ContinuouslyDecreaseHealth,Max = 50")]
    [Range(0, 50)] [SerializeField] float decHealthContinuous = 0;
    [SerializeField] Text lifeCountText;

    //参数
    /// <summary>
    /// 拥有Life的role
    /// </summary>
    Role role;
    /// <summary>
    /// 命数
    /// </summary>
    int lifeCount = 3;
    [Range(0, 500)] float health;

    // 初始化参数及校验配置
    void Start()
    {
        setHealth(fullHealth);
        role = GetComponent<Role>();
        if(role is Enemy)healthSlider.transform.gameObject.SetActive(false);
        if (role == null)
        {
            Debug.LogError("获取不到Player/Enemy组件");
        }
        setLivesCount(lifeCount);
    }

    private void Update()
    {
        float changeHealth = (decHealthContinuous - incHealthContinuous ) *Time.deltaTime;
        TakeHealth(changeHealth,false);
    }

    /// <summary>
    /// 减生命(还存活则返回true,死亡则返回false)
    /// </summary>
    /// <param name="decHealth">减少的生命量</param>
    /// <returns>是否死亡</returns>
    public bool TakeHealth(float decHealth,bool showHealthBar = true)
    {
        if (health <= 0) return false;
        decHealth = Mathf.Clamp(decHealth * (1 - buffPercent / 100) * (1 + debuffPercent / 100),
                                            health-fullHealth, health);
        bool isAlive = (health -= decHealth) > 0;
        if (!isAlive)
        {
            role.myAnimator.SetTrigger("trgDead");
            if (role is Player)//如果是玩家，则游戏失败
            {
                StartCoroutine(dealLose());
            }
            else                 //否则就是敌人
            {
                StartCoroutine(DealEnemyDeath());
            }
        }
        else//设置状态
        {
            role.myAnimator.SetBool("isHurting", true);
            new WaitForSeconds(2);
            role.myAnimator.SetBool("isHurting", false);
            //设置血条
            healthSlider.value = (float)health / fullHealth;
            if (role is Enemy && showHealthBar)
            {
                StartCoroutine(EnemyDisplay());
            }
        }
        return isAlive;
    }

    /// <summary>
    /// 处理Enemy血条显示
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnemyDisplay()
    {
        healthSlider.transform.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        healthSlider.transform.gameObject.SetActive(false);
    }

    /// <summary>
    /// 处理敌人死亡后的事情
    /// </summary>
    /// <returns></returns>
    IEnumerator DealEnemyDeath()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.9f);
        Destroy(role.gameObject);
    }

    /// <summary>
    /// 处理玩家死亡后的事情
    /// </summary>
    /// <returns></returns>
    IEnumerator dealLose()
    {
        setLivesCount(lifeCount-1);
        GameObject.Find("GameSession").GetComponent<GameSession>().SaveSession();
        Time.timeScale = 0.5f;
        GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(deathDelay);
        LevelDealer levelDealer = FindObjectOfType<LevelDealer>();
        if (levelDealer == null) Debug.LogError("未找到LevelDealer");
        if (lifeCount > 0)
            levelDealer.LoseThisLife();
        else
            levelDealer.LoadEndScene();
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 持续伤害
    /// </summary>
    /// <param name="damageValue">伤害值</param>
    internal void ContinuousDamage(float damageValue)
    {
        decHealthContinuous = damageValue;
    }
    /// <summary>
    /// 停止持续伤害
    /// </summary>
    internal void StopContinuousDamage()
    {
        decHealthContinuous = 0;
    }

    /// <summary>
    /// 持续恢复生命
    /// </summary>
    /// <param name="recoverValue">回复值/s</param>
    internal void ContinuousRecover(float recoverValue)
    {
        incHealthContinuous = recoverValue;
    }
    /// <summary>
    /// 停止持续恢复生命
    /// </summary>
    internal void StopContinuousRecover()
    {
        incHealthContinuous = 0;
    }

    #region getter/setter
    /// <summary>
    /// 判断是否还存活着
    /// </summary>
    /// <returns>血量为0true，否则false</returns>
    public bool isAlive()
    {
        return health > 0;
    }

    /// <summary>
    /// 获取当前生命值
    /// </summary>
    /// <returns></returns>
    public float getHealth()
    {
        return health;
    }
    /// <summary>
    /// 设置生命
    /// </summary>
    /// <param name="health"></param>
    public void setHealth(float health)
    {
        this.health = health;
        if (healthSlider == null)
        {
            Debug.LogError("获取不到healthSlider");
        }
        healthSlider.value = (float)health / fullHealth;
    }
    
    /// <summary>
    /// 获取剩余生命数量
    /// </summary>
    /// <returns></returns>
    public int getLivesCount()
    {
        return lifeCount;
    }
    /// <summary>
    /// 设置剩余生命数量
    /// </summary>
    /// <param name="livesCount"></param>
    internal void setLivesCount(int livesCount)
    {
        this.lifeCount = livesCount;
        if(role is Player) { 
            if (lifeCountText == null)
            {
                Debug.LogError("获取不到healthSlider下的lifeCountText");
            }
            else { 
                lifeCountText.text = lifeCount.ToString();
            }
        }
    }
    
    /// <summary>
    /// 获取最大生命值
    /// </summary>
    /// <returns></returns>
    public float getFullHealth()
    {
        return fullHealth;
    }
    /// <summary>
    /// 设置最大生命
    /// </summary>
    /// <param name="fullHealth"></param>
    public void setFullHealth(float fullHealth)
    {
        this.fullHealth = fullHealth;
    }
    #endregion
}
