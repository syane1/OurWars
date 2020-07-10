using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Role
{

    override protected void Start()
    {
        base.Start();
        InitialParams();
        GameSession session = FindObjectOfType<GameSession>();
        if (session !=null && session.GetSessionInfo()!=null && gameObject!=null && session.GetSessionInfo().players!=null&&session.GetSessionInfo().players.Contains(gameObject.name))
            session.RestoreSession();
    }

    override protected void Update()
    {
        if (!myLife.isAlive()) return;
        myMovement.Move();
        myWeapon.Trigger(new Vector2(transform.localScale.x, 0f));
        myWeapon.ThrowBomb();
    }

    /// <summary>
    /// 初始化参数
    /// </summary>
    override protected void InitialParams()
    {
        base.InitialParams();
        myMovement = GetComponent<PlayerMovement>();
        if (myMovement == null)
        {
            Debug.LogError("未找到PlayerMovement组件");
        }
        myWeapon = GetComponentInChildren<PlayerWeapon>();
        if (myWeapon == null)
        {
            Debug.LogError("未找到PlayerWeapon组件");
        }
        myPicker = GetComponentInChildren<Picker>();
        if (myPicker == null)
        {
            Debug.LogError("未找到Picker组件");
        }
    }

    public void RestorePlayer(float livesCount, float health, float fullHealth, float coins, float bulletNumber, float bulletTrgCount, float killedRivals)
    {
        myLife.setLivesCount((int)livesCount);
        myLife.setHealth(health);
        myLife.setFullHealth(fullHealth);
        myPicker.setCoins((int)coins);
        myWeapon.setBulletNumber((int)bulletNumber);
        myWeapon.setTrgCount((int)bulletTrgCount);
        myWeapon.setKilledRival((int)killedRivals);
    }
}
