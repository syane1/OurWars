using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class LevelDealer : MonoBehaviour
{
    //配置
    [SerializeField] GameObject menu;
    [SerializeField] GameObject winLable;
    [SerializeField] GameObject loseLable;

    //参数
    /// <summary>
    /// 上一次Level的Index
    /// </summary>
    string lastLevelName ;
    /// <summary>
    /// 当前scene的Index
    /// </summary>
    int sceneIndex = 0;
    /// <summary>
    /// 需要留存的东西
    /// </summary>
    ScenePersist scenePersist;

    private void Awake()
    {
        //单例写法
        if (FindObjectsOfType<LevelDealer>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (menu == null) Debug.LogError("menu为空，请先指定");
        else menu.SetActive(false);
        if (winLable == null) Debug.LogError("winLevel为空，请先指定");
        else winLable.SetActive(false);
        if (loseLable == null) Debug.LogError("loseLevel为空，请先指定");
        else loseLable.SetActive(false);
    }

    private void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Cancel"))
        {
            LoadMenu();
        }
    }

    #region Gaming
    /// <summary>
    /// 设置参数
    /// </summary>
    /// <param name="menuToSet">menu</param>
    /// <param name="winLabelToSet">winLabel</param>
    /// <param name="loseLabelToSet">loseLabel</param>
    /// <param name="scenePersistToSet">scenePersist</param>
    public void SetParam(GameObject menuToSet,GameObject winLabelToSet,GameObject loseLabelToSet,ScenePersist scenePersistToSet)
    {
        if(menuToSet!=null)menu = menuToSet;
        if(winLabelToSet != null)winLable = winLabelToSet;
        if(loseLabelToSet != null)loseLable = loseLabelToSet;
        if(scenePersistToSet != null)scenePersist = scenePersistToSet;
    }

    #region Getter/Setter
    public GameObject getMenu()
    {
        return menu;
    }

    public GameObject getWinLabel()
    {
        return winLable;
    }

    public GameObject getLoseLabel()
    {
        return loseLable;
    }
    #endregion

    /// <summary>
    /// 加载Menu
    /// </summary>
    public void LoadMenu()
    {
        Time.timeScale = 0;
        menu.SetActive(true);
    }

    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        menu.SetActive(false);
    }

    /// <summary>
    /// 本条命耗尽时显示失败的Label
    /// </summary>
    public void LoseThisLife()
    {
        loseLable.SetActive(true);
    }

    /// <summary>
    /// 下一条命继续战斗
    /// </summary>
    public void TryAgainNextLife()
    {
        LoadLevel(SceneManager.GetActiveScene().name);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        loseLable.SetActive(false);
        GameObject.Find("GameSession").GetComponent<GameSession>().RestoreSession();
    }
    #endregion
    
    #region Level
    /// <summary>
    /// 区分Scene和Level的Load
    /// </summary>
    /// <param name="sceneName">level的名称</param>
    private void LoadLevel(string levelName)
    {
        if (SceneManager.GetActiveScene().name.Contains("Level")) SaveLastLevel();
        SceneManager.LoadScene(levelName);
        sceneIndex++;
        if (scenePersist == null) scenePersist = FindObjectOfType<ScenePersist>();
    }

    /// <summary>
    /// 加载开始Level
    /// </summary>
    public void StartLevel()
    {
        LoadLevel("Level_Proto");
        sceneIndex = 0;
    }

    /// <summary>
    /// 加载实验室Level
    /// </summary>
    public void LoadLabLevel()
    {
        LoadLevel("LabLevel");
    }

    /// <summary>
    /// 加载下一个Level
    /// </summary>
    public void LoadNextLevel()
    {
        LoadLevel("Level" + (++sceneIndex));
    }

    /// <summary>
    /// 返回上一个Level，例如配置等场景回到游戏中
    /// </summary>
    public void BackToLevel()
    {
        LoadLevel(lastLevelName);
    }

    /// <summary>
    /// 重新加载当前level（不是Scene）
    /// </summary>
    public void ReloadCurrentLevel()
    {
        //销毁ScenePersist
        Destroy(FindObjectOfType<ScenePersist>().gameObject);
        LoadLevel(SceneManager.GetActiveScene().name);
        ResumeGame();
    }
    #endregion

    #region Scene
    /// <summary>
    /// 区分Scene和Level的Load
    /// </summary>
    /// <param name="sceneName">scene的名称</param>
    private void LoadScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.Contains("Level")) SaveLastLevel();
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 加载初始Scene
    /// </summary>
    public void LoadStartScene()
    {
        LoadScene("StartScene");
    }

    /// <summary>
    /// 加载配置Scene
    /// </summary>
    public void LoadConfigScene()
    {
        LoadScene("ConfigScene");
    }

    /// <summary>
    /// 加载关于Scene
    /// </summary>
    public void LoadAboutScene()
    {
        LoadScene("AboutScene");
    }

    /// <summary>
    /// 加载结束Scene
    /// </summary>
    public void LoadEndScene()
    {
        LoadScene("EndScene");
    }

    #endregion

    /// <summary>
    /// 离开游戏
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// 保存上次Level
    /// </summary>
    void SaveLastLevel()
    {
        lastLevelName = SceneManager.GetActiveScene().name;
        GameObject.Find("GameSession").GetComponent<GameSession>().SaveSession();
    }
}
