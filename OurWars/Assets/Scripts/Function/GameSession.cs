using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    #region 相关前置声明
    #region SessionRecord的相关命名
    /// <summary>
    /// SessionRecord的剩余命数KEY名称
    /// </summary>
    /// </summary>
    /// </summary>
    public static readonly string SESSION_RECORD_LIVECOUNT_KEYNAME = "_LivesCnt";
    /// <summary>
    /// SessionRecord记录的剩余健康值KEY名称
    /// </summary>
    public static readonly string SESSION_RECORD_HEALTHVALUE_KEYNAME = "_LivesHealth";
    /// <summary>
    /// SessionRecord记录的最大健康值KEY名称
    /// </summary>
    public static readonly string SESSION_RECORD_FULLHEALTHVALUE_KEYNAME = "_LivesFullHealth";
    /// <summary>
    /// SessionRecord获得的钥匙数KEY名称
    /// </summary>
    public static readonly string SESSION_RECORD_KEYSCOUNT_KEYNAME = "_KeysCnt";
    /// <summary>
    /// SessionRecord获得的子弹数KEY名称
    /// </summary>
    public static readonly string SESSION_RECORD_BULLETCOUNT_KEYNAME = "_BulletsCnt";
    /// <summary>
    /// SessionRecord开火的子弹数KEY名称
    /// </summary>
    public static readonly string SESSION_RECORD_BULLETSTRGCOUNT_KEYNAME = "_BulletsTriggeredCnt";
    /// <summary>
    /// SessionRecord杀死敌人数KEY名称
    /// </summary>
    public static readonly string SESSION_RECORD_KILLEDENEMYCOUNT_KEYNAME = "_KilledEnemyCnt";
    /// <summary>
    /// Session文件的路径
    /// </summary>
    private readonly string SESSION_FILE_PATH = "Storage";
    /// <summary>
    /// Session文件的文件名
    /// </summary>
    private readonly string SESSION_FILENAME = "session.json";
    Encoding SYSTEM_DEFAULT_ENCODING = System.Text.Encoding.Default;
    private readonly string ENC_DEC_KEY = "HopeOurWarsBeBetterrrrrrrrrrrrrr";
    #endregion

    /// <summary>
    /// 游戏类型 AERIAL_VIEW 鸟瞰型, PROFILE_VIEW 剖面型
    /// </summary>
    public enum GameType {
        AERIAL_VIEW, PROFILE_VIEW
    }

    /// <summary>
    /// scenePersist保存的GameObject的名字
    /// </summary>
    //public class sceneDealerSaver
    //{
    //    public sceneDealerSaver(string menu, string winLabel, string loseLabel) { menuName = menu;winLabelName = winLabel;loseLabelName = loseLabel; }
    //    public string menuName { get; set; }
    //    public string winLabelName { get; set; }
    //    public string loseLabelName { get; set; }
    //}
    
    /// <summary>
    /// GameInfo类型
    ///     gameType { get; set; }  游戏类型
    ///     sceneName               离开之前场景
    ///     players                 play的GameObject
    ///     sceneDealer             场景处理器
    ///     scenePersist            场景留存
    ///     sessionRecord           保存的主体(player中需要储存的部分)
    /// </summary>
    public class SessionInfo {
        /// <summary>
        /// 游戏类型 鸟瞰/剖面
        /// </summary>
        public GameType gameType { get; set; }
        /// <summary>
        /// scene的名字
        /// </summary>
        public string sceneName { get; set; }
        /// <summary>
        /// players的名字
        /// </summary>
        public List<string> players { get; set; }
        /// <summary>
        /// sceneDealer的名字
        /// </summary>
        //public string sceneDealerName { get; set; }
        /// <summary>
        /// sceneDealer存储的内容
        /// </summary>
        //public sceneDealerSaver sceneDealerSaver { get; set; }
        /// <summary>
        /// scenePersist的名字(NoUse)
        /// </summary>
        //public string scenePersistName { get; set; }
        /// <summary>
        /// 记录的Players的内容
        /// </summary>
        public Dictionary<string,float> sessionRecords { get; set; }
    };
    #endregion

    /// <summary>
    /// 变量
    /// </summary>
    static SessionInfo sessionInfo = new SessionInfo();
    private string sessionJson = "";

    private void Awake()
    {
        //单例写法
        if (FindObjectsOfType<GameSession>().Length > 1)
        {
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(this);
        }
    }
    
    void Start()
    {
        sessionInfo.gameType = GameType.PROFILE_VIEW;
        sessionInfo.sessionRecords = new Dictionary<string, float>();
        sessionInfo.players = new List<string>();
    }

    #region SessionInfo相关
    /// <summary>
    /// 添加指定player的SessionRecord
    /// </summary>
    /// <param name="playerName">player的名称</param>
    private void AddSessionRecord(string playerName)
    {
        SessionInfo empty = new SessionInfo();
        if (sessionInfo.Equals(empty) && sessionInfo.sessionRecords == null)
        {
            sessionInfo.sessionRecords = new Dictionary<string, float>();
        }
        sessionInfo.sessionRecords.Remove(playerName + SESSION_RECORD_LIVECOUNT_KEYNAME);
        sessionInfo.sessionRecords.Add(playerName + SESSION_RECORD_LIVECOUNT_KEYNAME, 3);
        sessionInfo.sessionRecords.Remove(playerName + SESSION_RECORD_HEALTHVALUE_KEYNAME);
        sessionInfo.sessionRecords.Add(playerName + SESSION_RECORD_HEALTHVALUE_KEYNAME, 100);
        sessionInfo.sessionRecords.Remove(playerName + SESSION_RECORD_FULLHEALTHVALUE_KEYNAME);
        sessionInfo.sessionRecords.Add(playerName + SESSION_RECORD_FULLHEALTHVALUE_KEYNAME, 100);
        sessionInfo.sessionRecords.Remove(playerName + SESSION_RECORD_KEYSCOUNT_KEYNAME);
        sessionInfo.sessionRecords.Add(playerName + SESSION_RECORD_KEYSCOUNT_KEYNAME, 0);
        sessionInfo.sessionRecords.Remove(playerName + SESSION_RECORD_BULLETCOUNT_KEYNAME);
        sessionInfo.sessionRecords.Add(playerName + SESSION_RECORD_BULLETCOUNT_KEYNAME, 0);
        sessionInfo.sessionRecords.Remove(playerName + SESSION_RECORD_BULLETSTRGCOUNT_KEYNAME);
        sessionInfo.sessionRecords.Add(playerName + SESSION_RECORD_BULLETSTRGCOUNT_KEYNAME, 0);
        sessionInfo.sessionRecords.Remove(playerName + SESSION_RECORD_KILLEDENEMYCOUNT_KEYNAME);
        sessionInfo.sessionRecords.Add(playerName + SESSION_RECORD_KILLEDENEMYCOUNT_KEYNAME, 0);
    }

    /// <summary>
    /// 获取到指定player的SessionRecord
    /// </summary>
    /// <param name="playerName">player的名称</param>
    /// <returns>所有包含playerName的KeyValuePair</returns>
    private Dictionary<string, float> GetSessionRecord(string playerName)
    {
        Dictionary<string, float> playerRecord = new Dictionary<string, float>();
        playerRecord.Clear();
        foreach (KeyValuePair<string, float> record in sessionInfo.sessionRecords)
        {
            if (record.Key.Contains(playerName + "_")) playerRecord.Add(record.Key, record.Value);
        }
        return playerRecord;
    }

    /// <summary>
    /// 去除指定player的相关sessionRecord
    /// </summary>
    /// <param name="playerName">player的名称</param>
    private void DropSessionRecord(string playerName)
    {
        List<string> keysToDrop=new List<string>();
        if (sessionInfo.sessionRecords.Count >= 1) { 
            foreach (var record in sessionInfo.sessionRecords)
            {
                if (record.Key.Contains(playerName)) keysToDrop.Add(record.Key);
            }
            foreach (string keyToDrop in keysToDrop)
            {
                sessionInfo.sessionRecords.Remove(keyToDrop);
            }
        }
    }

    /// <summary>
    /// 设置指定player的sessionRecord
    /// </summary>
    /// <param name="playerName">指定Player的名称</param>
    /// <param name="livesCount">剩余命数</param>
    /// <param name="keysCount">获得钥匙数</param>
    /// <param name="bulletsTrgCount">开火次数</param>
    /// <param name="killedEnemyCount">杀死敌人数</param>
    private void SetSessionRecord(string playerName, int livesCount, float livesHealth,float livesFullHealth, int keysCount, int bulletsCount, int bulletsTrgCount, int killedEnemyCount)
    {
        SetSessionRecord(playerName, SESSION_RECORD_LIVECOUNT_KEYNAME, livesCount);
        SetSessionRecord(playerName, SESSION_RECORD_HEALTHVALUE_KEYNAME, livesHealth);
        SetSessionRecord(playerName, SESSION_RECORD_FULLHEALTHVALUE_KEYNAME, livesFullHealth);
        SetSessionRecord(playerName, SESSION_RECORD_KEYSCOUNT_KEYNAME, keysCount);
        SetSessionRecord(playerName, SESSION_RECORD_BULLETCOUNT_KEYNAME, bulletsCount);
        SetSessionRecord(playerName, SESSION_RECORD_BULLETSTRGCOUNT_KEYNAME, bulletsTrgCount);
        SetSessionRecord(playerName, SESSION_RECORD_KILLEDENEMYCOUNT_KEYNAME, killedEnemyCount);
    }

    /// <summary>
    /// 存储Session
    /// </summary>
    public void SaveSession()
    {   
        sessionInfo.sceneName = SceneManager.GetActiveScene().name;
        foreach (string player in sessionInfo.players)
        {
            DropSessionRecord(player);
        }
        sessionInfo.players.Clear();
        Player[] players = FindObjectsOfType<Player>();
        if (players.Length > 0)
        {
            foreach (Player player in players)
            {
                sessionInfo.players.Add(player.gameObject.name);
                AddSessionRecord(player.gameObject.name);
                SetSessionRecord(player.gameObject.name, player.myLife.getLivesCount(), player.myLife.getHealth(), player.myLife.getFullHealth(), player.myPicker.getCoins(), player.myWeapon.getBulletNumber(), player.myWeapon.getTrgCount(), player.myWeapon.getKilledRival());
            }
        }
        //LevelDealer levelDealer = FindObjectOfType<LevelDealer>();
        //sessionInfo.sceneDealerName = levelDealer.gameObject.name;
        //sessionInfo.sceneDealerSaver = new sceneDealerSaver(levelDealer.getMenu().name,levelDealer.getWinLabel().name, levelDealer.getLoseLabel().name);
        //sessionInfo.scenePersistName = FindObjectOfType<ScenePersist>().gameObject.name;
        //Debug.Log(SessionInfoToString());
        //TODO 存储到文件
        string jsonStr = JsonConvert.SerializeObject(sessionInfo);
        if (File.Exists(SESSION_FILE_PATH + "\\" + SESSION_FILENAME))
        {
            FileStream file = new FileStream(SESSION_FILE_PATH + "\\" + SESSION_FILENAME, FileMode.Truncate, FileAccess.Write);
            byte[] bytes = SYSTEM_DEFAULT_ENCODING.GetBytes(jsonStr);
            bytes = Encrypt(bytes);
            file.Write(bytes, 0, bytes.Length);
            file.Position = 0;
            file.Close();
        }
        else {
            FileStream file = new FileStream(SESSION_FILE_PATH + "\\" + SESSION_FILENAME, FileMode.CreateNew, FileAccess.Write);
            byte[] bytes = SYSTEM_DEFAULT_ENCODING.GetBytes(jsonStr);
            bytes = Encrypt(bytes);
            file.Write(bytes, 0, bytes.Length);
            file.Position = 0;
            file.Close();
        }
    }

    /// <summary>
    /// 设置SessionRecord的值
    /// </summary>
    /// <param name="playerName">player的名称</param>
    /// <param name="keyName">Key名称</param>
    /// <param name="value">要设置的值</param>
    private void SetSessionRecord(string playerName, string keyName, float value)
    {
        float varTmp;
        if(!sessionInfo.sessionRecords.TryGetValue(playerName+keyName,out varTmp)){
            Debug.Log("设置"+playerName+keyName+"的值"+value+"失败");
            return;
        }else{
            if(varTmp!=value) sessionInfo.sessionRecords[playerName + keyName] = value;
        }
    }

    /// <summary>
    /// 从Session中恢复各个状态
    /// </summary>
    public void RestoreSession()
    {
        sessionJson = ReadSession();
        if (sessionJson != "") { 
            sessionInfo = JsonConvert.DeserializeObject<SessionInfo>(sessionJson);
            List<Player> targetPlayers=new List<Player>();
            foreach (string playerName in sessionInfo.players)
            {
                GameObject playerObj = GameObject.Find(playerName);
                if (playerObj == null) continue;
                targetPlayers.Add(playerObj.GetComponent<Player>());
            }
            if (targetPlayers.Count > 0)
            {
                foreach (Player player in targetPlayers)
                {
                    Dictionary<string, float> record = GetSessionRecord(player.gameObject.name);
                    record.TryGetValue(player.gameObject.name+SESSION_RECORD_LIVECOUNT_KEYNAME, out float livesCount);
                    record.TryGetValue(player.gameObject.name+SESSION_RECORD_HEALTHVALUE_KEYNAME, out float health);
                    record.TryGetValue(player.gameObject.name+ SESSION_RECORD_FULLHEALTHVALUE_KEYNAME, out float fullHealth);
                    record.TryGetValue(player.gameObject.name+ SESSION_RECORD_KEYSCOUNT_KEYNAME, out float coins);
                    record.TryGetValue(player.gameObject.name+ SESSION_RECORD_BULLETCOUNT_KEYNAME, out float bulletNumber);
                    record.TryGetValue(player.gameObject.name+ SESSION_RECORD_BULLETSTRGCOUNT_KEYNAME, out float bulletTrgCount);
                    record.TryGetValue(player.gameObject.name+ SESSION_RECORD_KILLEDENEMYCOUNT_KEYNAME, out float killedRivals);
                    health = health <= 0 ? fullHealth : fullHealth;
                    player.RestorePlayer(livesCount, health, fullHealth, coins, bulletNumber, bulletTrgCount, killedRivals);
                }
            }
        }
    }

    /// <summary>
    /// 读取Session
    /// </summary>
    public string ReadSession()
    {
        string str = "";
        if (File.Exists(SESSION_FILE_PATH + "\\" + SESSION_FILENAME)) {
            FileStream file = new FileStream(SESSION_FILE_PATH + "\\" + SESSION_FILENAME, FileMode.OpenOrCreate, FileAccess.Read);
            Byte[] bytes = new byte[file.Length];
            file.Read(bytes, 0, bytes.Length);
            //str = SYSTEM_DEFAULT_ENCODING.GetString(bytes);
            str = Decrypt(bytes);
            file.Position = 0;
            file.Close();
        }
        else
        {
            str = "";
        }
        return str;
    }

    /// <summary>
    /// 将SessionInfo转为String
    /// </summary>
    /// <returns></returns>
    public string SessionInfoToString()
    {
        string str="";
        float strAdd ;
        str += sessionInfo.sceneName +"\n";
        foreach (string playerName in sessionInfo.players) {
            str += playerName +"\n";
            sessionInfo.sessionRecords.TryGetValue(playerName + SESSION_RECORD_LIVECOUNT_KEYNAME, out strAdd);
            str += strAdd + "\n";
            sessionInfo.sessionRecords.TryGetValue(playerName + SESSION_RECORD_HEALTHVALUE_KEYNAME, out strAdd);
            str += strAdd + "\n";
            sessionInfo.sessionRecords.TryGetValue(playerName + SESSION_RECORD_KEYSCOUNT_KEYNAME, out strAdd);
            str += strAdd + "\n";
            sessionInfo.sessionRecords.TryGetValue(playerName + SESSION_RECORD_BULLETCOUNT_KEYNAME, out strAdd);
            str += strAdd + "\n";
            sessionInfo.sessionRecords.TryGetValue(playerName + SESSION_RECORD_BULLETSTRGCOUNT_KEYNAME, out strAdd);
            str += strAdd + "\n";
            sessionInfo.sessionRecords.TryGetValue(playerName + SESSION_RECORD_KILLEDENEMYCOUNT_KEYNAME, out strAdd);
            str += strAdd + "\n";
            str += "\n\n";
        }

        return str;
    }

    //内容加密
    public byte[] Encrypt(byte[] toEncrypt)
    {
        //加密和解密采用相同的key,具体自己填，但是必须为32位//
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(ENC_DEC_KEY);
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rDel.CreateEncryptor();

        byte[] toEncryptArray = toEncrypt;
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        return resultArray;
    }

    //内容解密
    public string Decrypt(byte[] toDecrypt)
    {
        //加密和解密采用相同的key,具体值自己填，但是必须为32位//
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(ENC_DEC_KEY);

        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rDel.CreateDecryptor();
        
        byte[] resultArray = cTransform.TransformFinalBlock(toDecrypt, 0, toDecrypt.Length);

        return SYSTEM_DEFAULT_ENCODING.GetString(resultArray);
    }
    #endregion

    /// <summary>
    /// 获得Session的信息
    /// </summary>
    /// <returns>0俯视/1剖面</returns>
    public SessionInfo GetSessionInfo()
    {
        return sessionInfo;
    }
    /// <summary>
    /// 更换游戏类型（俯视/剖面）
    /// </summary>
    public void ChangeGameType()
    {
        sessionInfo.gameType = sessionInfo.gameType==GameType.PROFILE_VIEW?GameType.AERIAL_VIEW:GameType.PROFILE_VIEW;
    }
}
