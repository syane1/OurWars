using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamingSwitcher : MonoBehaviour
{
    enum SwitchType {
        HazardRise,
        HazardDown
    }

    [SerializeField] SwitchType switchType = SwitchType.HazardRise;
    [SerializeField] GameObject[] objsToSwitch;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (objsToSwitch.Length <= 0) return;
        switch (switchType)
        {
            case SwitchType.HazardRise:
                foreach(GameObject objToSwitch in objsToSwitch)
                {
                    SwitchRise(objToSwitch);
                }
                break;
            case SwitchType.HazardDown:
                foreach (GameObject objToSwitch in objsToSwitch)
                {
                    SwitchDown(objToSwitch);
                }
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 用来开启升起的对象，开启/关闭AutoScroll的速度
    /// </summary>
    /// <param name="obj"></param>
    private void SwitchRise(GameObject obj)
    {
        if (obj == null) return;
        AutoScroll objAutoScroll = obj.GetComponent<AutoScroll>();
        if (objAutoScroll == null) return;
        objAutoScroll.SetScroll(AutoScroll.scrollType.VerticleUp);
        if (!objAutoScroll.isActive)
            objAutoScroll.SetScrollSpeed(0.02f);
        else
            objAutoScroll.SetScrollSpeed(0);
    }
    /// <summary>
    /// 开关下降的对象，通过赋予rigidbody的bodytype不同值实现
    /// </summary>
    /// <param name="obj">待开关对象</param>
    private void SwitchDown(GameObject obj)
    {
        if (obj == null) return;
            Rigidbody2D rigidbody = obj.GetComponent<Rigidbody2D>();
            if (rigidbody == null) return;
            rigidbody.bodyType = rigidbody.bodyType == RigidbodyType2D.Kinematic || rigidbody.bodyType == RigidbodyType2D.Static ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
    }
}
