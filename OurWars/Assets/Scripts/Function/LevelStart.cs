using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStart : MonoBehaviour
{
    AutoScroll startAutoScroll ;
    GameObject startButton;
    Role[] roles;
    LevelDealer levelDealer;
    GameSession gameSession;

    // Start is called before the first frame update
    void Start()
    {
        roles = FindObjectsOfType<Role>();
        foreach (Role role in roles)
        {
            role.gameObject.SetActive(false);
        }
        startAutoScroll = GameObject.Find("StartAboutText").GetComponent<AutoScroll>();
        if(startAutoScroll==null)Debug.LogError("查找StartAboutText对象时出错");
        startButton = GameObject.Find("StartButton");
        if (startButton == null) Debug.LogError("查找StartButton对象时出错");
        startButton.SetActive(false);
        levelDealer = GameObject.Find("LevelDealer").GetComponent<LevelDealer>();
        if (levelDealer == null) Debug.LogError("查找levelDealer对象时出错");
        gameSession = GameObject.Find("GameSession").GetComponent<GameSession>();
        if (gameSession == null) Debug.LogError("查找gameSession对象时出错");
        //若sessionInfo中的sceneName是当前level的名称时，则恢复session
        if (gameSession.GetSessionInfo().sceneName == levelDealer.gameObject.name)
        {
            gameSession.RestoreSession();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!startAutoScroll.isActive)
        {
            startButton.SetActive(true);
        }
    }

    public void DestroyLevelLoader()
    {
        //roles = FindObjectsOfType<Role>();
        foreach (Role role in roles)
        {
            if(role!=null)
            role.gameObject.SetActive(true);
        }
        Destroy(gameObject);
    }
}
