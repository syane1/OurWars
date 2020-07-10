using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Picker : MonoBehaviour
{
    int coins = 0;
    [SerializeField] Text coinText;

    //参数
    Role role;

    private void Start()
    {
        role = GetComponentInParent<Role>()as Player;
        if (role == null)
        {
            Debug.LogError("获取不到Player，请先指定");
        }
        setCoins(coins);
    }

    /// <summary>
    /// 增加积分
    /// </summary>
    /// <param name="coinsToAdd">本次增加的积分</param>
    internal void AddCoin(int coinsToAdd)
    {
        setCoins(coins + coinsToAdd);
    }

    /// <summary>
    /// 花费积分
    /// </summary>
    /// <param name="coinsToSpend"></param>
    internal void SpendCoin(int coinsToSpend)
    {
        setCoins(coins - coinsToSpend);
    }

    /// <summary>
    /// 增加子弹
    /// </summary>
    /// <param name="bulletToAdd">本次增加的数量</param>
    internal void AddBullet(int bulletToAdd)
    {
        role.myWeapon.AddBullet(bulletToAdd);
    }

    /// <summary>
    /// 获取积分/钥匙
    /// </summary>
    /// <returns></returns>
    internal int getCoins()
    {
        return coins;
    }

    /// <summary>
    /// 设置积分/钥匙
    /// </summary>
    /// <param name="coins"></param>
    internal void setCoins(int coins)
    {
        this.coins = coins;
        if (coinText == null)
        {
            Debug.LogError("获取不到coinText，请先指定");
        }
        coinText.text = this.coins.ToString();
    }
}
