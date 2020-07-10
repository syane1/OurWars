using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    /*AI状态 巡逻(A,B),返回巡逻点,(对目标的)探测(探测范围)感知(感知范围),攻击,思考(攻击|追踪|继续巡逻|返回巡逻点)*/
    internal enum AIState {
        Thinking,
        Patrolling,
        Sensing,
        BackToPatrolling,
        Attacking,
        Jumping
    }

    #region 配置
    [SerializeField] GameObject patrolA;            //巡逻点A
    [SerializeField] GameObject patrolB;            //巡逻点B
    [SerializeField] float patrolGap = 0.1f;        //巡逻点误差
    [SerializeField] float thinkingInterval = 1f;   //思考间隔
    [SerializeField] float detectDistance = 5f;     //探测长度
    [SerializeField] float trigDistance = 2f;       //感知范围
    [SerializeField] float trackDuration = 2f;      //追踪持续时间
    #endregion

    #region 参数
    Vector2 targetPos;         //目标位置
    [SerializeField] AIState aiState = AIState.Patrolling;
    Vector2 patrolAPos;     //巡逻点A位置
    Vector2 patrolBPos;     //巡逻点B位置
    Role role;
    #endregion
    
    #region 缓存
    Vector2 lastTargetPos;     //上一个目标位置
    AIState lastAIState;       //上一个状态
    float trackTimer;       //追踪用计时器
    #endregion

    //校验及初始化一些值
    private void Start()
    {
        if (patrolA == null || patrolB == null)
        {
            Debug.LogError("请先指定巡逻点AB");
        }
        else
        {
            patrolA.transform.parent = GameObject.Find("Patrols").transform;
            patrolB.transform.parent = GameObject.Find("Patrols").transform;
        }
        patrolAPos = patrolA.transform.position;
        patrolBPos = patrolB.transform.position;
        role = GetComponent<Role>();
        if(role==null)
        {
            Debug.LogError("未找到role组件");
        }
        //取AB中近的一点作为目标的初始对象
        setTarget((patrolAPos - (Vector2)transform.position).magnitude > (patrolBPos - (Vector2)transform.position).magnitude ? patrolBPos : patrolAPos);
        lastAIState = aiState;
        lastTargetPos = targetPos;
    }

    private void Update()
    {
        if (!role.myLife.isAlive()) return;
        Thinking();
    }

    /// <summary>
    /// 思考(攻击|继续巡逻|返回巡逻点)
    /// </summary>
    private void Thinking()
    {
        StartCoroutine(Detecting());
        switch (aiState)
        {
            case AIState.Patrolling:
                //如果当前目标不为A或B之一则切换巡逻目标点
                if (targetPos != patrolAPos && targetPos != patrolBPos)
                {
                    StartCoroutine(ChangePatrolTarget());
                    break;
                }
                //若位置足够接近目标位置，则切换目标为另一个巡逻点
                if (Mathf.Abs(targetPos.x - transform.position.x) <= patrolGap)
                {
                    StartCoroutine(ChangePatrolTarget());
                }
                break;
            case AIState.Sensing:
                float targetDistance = (targetPos - (Vector2)transform.position).magnitude;
                float trackCost = Time.time - trackTimer;
                if (targetDistance > detectDistance || trackCost > trackDuration)
                {
                    StartCoroutine(ChangePatrolTarget());
                }
                break;
            case AIState.BackToPatrolling:
                //如果当前目标不为A或B之一则切换巡逻目标点
                if (targetPos!=patrolAPos&&targetPos!=patrolBPos)
                {
                    StartCoroutine(ChangePatrolTarget());
                    break;
                }
                //若位置足够接近目标位置，设置状态为巡逻状态，并则切换目标为另一个巡逻点
                if (Mathf.Abs(targetPos.x - transform.position.x) <= patrolGap)
                {
                    setState(AIState.Patrolling);
                    StartCoroutine(ChangePatrolTarget());
                }
                break;
            case AIState.Attacking:
                role.myWeapon.Trigger((targetPos-(Vector2)transform.position).normalized);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 获取有效(小于detectDistance)的最近对手目标位置
    /// </summary>
    /// <returns>对手目标位置</returns>
    private Vector3 GetRivalPosition()
    {
        Vector3 rivalPosition = Vector3.zero;
        if (role is Enemy)
        {
            Player[] rivals = FindObjectsOfType<Player>();
            foreach (Player rival in rivals)
            {
                rivalPosition = (rivalPosition == Vector3.zero || //目标为空
                                ((rival.transform.position - transform.position).magnitude <= (rivalPosition - transform.position).magnitude)) &&  //或有较近的rival
                                (rival.transform.position - transform.position).magnitude <= detectDistance//且在探测范围内
                    ? rival.transform.position : rivalPosition;
            }
        }
        else
        {
            //TODO 暂时保留Player的获取有效(小于detectDistance)的最近对手目标位置功能
            //Enemy[] Rivals = FindObjectsOfType<Enemy>();
            //foreach (Enemy rival in Rivals)
            //{
            //    rivalPosition = (rivalPosition == Vector3.zero || //目标为空
            //                    ((rival.transform.position - transform.position).magnitude <= (rivalPosition - transform.position).magnitude)) &&  //新rival敌我距离小于当前目标的敌我距离
            //                    (rival.transform.position - transform.position).magnitude <= detectDistance//且在探测范围内
            //        ? rival.transform.position : rivalPosition;
            //}
        }
        return rivalPosition;
    }

    /// <summary>
    /// 探测
    /// </summary>
    private IEnumerator Detecting()
    {
        Vector2 tmpTarget = GetRivalPosition();
        if (tmpTarget != Vector2.zero)
        {
            //如果距离小于触发距离trigDistance,则将状态转换为
            if ((tmpTarget - (Vector2)transform.position).magnitude < trigDistance){
                RestoreState();
                setState(AIState.Attacking);
            }
            else { 
                if (getState() != AIState.Sensing) { 
                    setState(AIState.Thinking);
                    yield return new WaitForSeconds(thinkingInterval);
                    setState(AIState.Sensing);
                }
                if (targetPos != lastTargetPos) trackTimer = Time.time;
            }
            setTarget(tmpTarget);
        } else if (aiState==AIState.Sensing) {
            //找不到之后并且状态仍为Sensing，则将状态设为返回巡逻点，并将目标设为巡逻点之一
            StopAllCoroutines();
            setState(AIState.BackToPatrolling);
            StartCoroutine(ChangePatrolTarget());
        }
    }

    /// <summary>
    /// 切换巡逻点
    /// </summary>
    /// <returns></returns>
    internal IEnumerator ChangePatrolTarget()
    {
        setState(AIState.Thinking);
        yield return new WaitForSeconds(thinkingInterval);
        //当目标即不是A也不是B时，则预设为A
        if (targetPos != patrolAPos && targetPos != patrolBPos)
        {
            //Debug.Log("此时target既不是A也不是B，设置为A");
            setTarget(patrolA.transform.position);
        }
        //否则（即目标为A或B中的一个，则切换为另一个）
        //Debug.Log("切换target，A则切换为B，否则切换为A");
        setTarget(targetPos== (Vector2)patrolA.transform.position ? patrolB.transform.position : patrolA.transform.position);
        if (aiState != AIState.Patrolling) setState(AIState.Patrolling);
    }

    #region Getter/Setter
    /// <summary>
    /// 获取target
    /// </summary>
    /// <returns>target的值</returns>
    internal Vector2 getTarget()
    {
        return targetPos;
    }
    /// <summary>
    /// 设置Target
    /// </summary>
    /// <param name="newTarget"></param>
    void setTarget(Vector2 newTarget)
    {
        if(newTarget!=targetPos&&lastTargetPos!=targetPos)lastTargetPos = targetPos;
        targetPos = newTarget;
    }
    /// <summary>
    /// 恢复target
    /// </summary>
    internal void RestoreTarget()
    {
        targetPos = lastTargetPos;
    }
    /// <summary>
    /// 获取State（Action）
    /// </summary>
    /// <returns></returns>
    internal AIState getState()
    {
        return aiState;
    }
    /// <summary>
    /// 设置State/Action
    /// </summary>
    /// <param name="newState">待设置的AI状态</param>
    internal void setState(AIState newState)
    {
        //Jumping和Thinking状态存储，新状态和当前状态不能一样
        if (aiState != AIState.Jumping && aiState != AIState.Thinking && aiState != newState && lastAIState != aiState) lastAIState = aiState;
        aiState = newState;
    }
    /// <summary>
    /// 恢复aiState
    /// </summary>
    internal void RestoreState()
    {
        targetPos = lastTargetPos;
    }
    /// <summary>
    /// 获取思考时长
    /// </summary>
    /// <returns></returns>
    internal float getThinkingInterval()
    {
        return thinkingInterval;
    }
    #endregion
}
