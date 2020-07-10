using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    enum PickType{
        Coin,
        Bullet
    }

    //配置
    [SerializeField] PickType pickType = PickType.Coin;
    [SerializeField] int addValue = 10;
    
    private void OnTriggerEnter2D(Collider2D gainer)
    {
        Picker picker = gainer.GetComponentInChildren<Picker>();
        if (picker != null)
        {
            switch (pickType) {
                case PickType.Coin:
                    picker.AddCoin(addValue);
                    break;
                case PickType.Bullet:
                    picker.AddBullet(addValue);
                    break;
                default:
                    break;
            }
        }
        else {
            Debug.Log("Picker是空的");
        }
        Destroy(gameObject);
    }
}
