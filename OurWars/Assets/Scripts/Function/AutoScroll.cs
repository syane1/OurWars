using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    //滚动方向
    internal enum scrollType {
        VerticleUp, VerticleDown,
        HorizontalLeft, HorizontalRight
    }

    [SerializeField] scrollType scroll = scrollType.VerticleUp;
    [SerializeField] float scrollSpeed = 1f;
    [SerializeField] Vector2 endPosition = new Vector2(0, 0);
    public bool isActive;

    // Update is called once per frame
    void Update()
    {
        isActive = scrollSpeed == 0 ? false : true;
        switch (scroll) {
            case scrollType.VerticleUp:
                transform.Translate(Vector2.up * scrollSpeed * Time.timeScale);
                Stop();
                break;
            case scrollType.VerticleDown:
                transform.Translate(Vector2.down * scrollSpeed * Time.timeScale);
                Stop();
                break;
            case scrollType.HorizontalLeft:
                transform.Translate(Vector2.right * scrollSpeed * Time.timeScale);
                Stop();
                break;
            case scrollType.HorizontalRight:
                transform.Translate(Vector2.right * scrollSpeed * Time.timeScale);
                Stop();
                break;
            default:
                Debug.LogError("scroll 类型错误");
                break;
        }
    }

    /// <summary>
    /// 阻止AutoScroll继续运行
    /// </summary>
    private void Stop()
    {
        switch (scroll)
        {
            case scrollType.VerticleUp:
                if (endPosition != Vector2.zero && transform.localPosition.y >= endPosition.y) scrollSpeed = 0;
                break;
            case scrollType.VerticleDown:
                if (endPosition != Vector2.zero && transform.localPosition.y <= endPosition.y) scrollSpeed = 0;
                break;
            case scrollType.HorizontalLeft:
                if (endPosition != Vector2.zero && transform.localPosition.x <= endPosition.x) scrollSpeed = 0;
                break;
            case scrollType.HorizontalRight:
                if (endPosition != Vector2.zero && transform.localPosition.x >= endPosition.x) scrollSpeed = 0;
                break;
            default:
                Debug.LogError("scroll 类型错误");
                break;
        }
        
    }

    /// <summary>
    /// 设置滚动类型 （上、下、左、右
    /// </summary>
    /// <param name="type"></param>
    internal void SetScroll(scrollType type)
    {
        scroll = type;
    }
    
    /// <summary>
    /// 设置滚动速度
    /// </summary>
    /// <param name="speed"></param>
    internal void SetScrollSpeed(float speed)
    {
        scrollSpeed = speed;
    }
    
}
