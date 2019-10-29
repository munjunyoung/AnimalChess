using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBTBase : MonoBehaviour
{
    public abstract void Init();
    public abstract void StartBT();
    public abstract void StopBT();
    public abstract IEnumerator BehaviorProcess();
}

public abstract class ActionNode { public abstract bool Invoke(); }

/// <summary>
/// NOTE : Node stack 
/// </summary>
public class CompositeActionNode : ActionNode
{
    private List<ActionNode> childrens = new List<ActionNode>();

    public override bool Invoke()
    {
        throw new System.NotImplementedException();
    }

    public void AddChild(ActionNode _Node)
    {
        childrens.Add(_Node);
    }

    public List<ActionNode> GetChildrens()
    {
        return childrens;
    }
}

/// <summary>
/// NOTE : 노드들중 하나라도 True면 true를 리턴
/// </summary>
public class Selector : CompositeActionNode
{
    public override bool Invoke()
    {
        foreach (var node in GetChildrens())
        {
            if (node.Invoke())
                return true;
        }
        return false;
    }
}

/// <summary>
/// NOTE : 모든 자식들이 True여야 True를 리턴
/// </summary>
public class Sequence : CompositeActionNode
{
    public override bool Invoke()
    {
        foreach (var node in GetChildrens())
        {
            if (!node.Invoke())
                return false;
        }
        return true;
    }
}

#region chaseAttack
public class SetTargetBlock : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        return _controller.SetTargetBlock();
        
    }
}
/// <summary>
/// NOTE : 현재 타겟 상태 죽었을 경우 다시 리턴
/// </summary>
public class SetPath : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        return _controller.SetPath();
    }
}

/// <summary>
/// NOTE : 현재 타겟 상태 죽었을 경우 다시 리턴
/// </summary>
public class CheckMoveState : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        return _controller.CheckMoveState();
    }
}

/// <summary>
/// NOTE : 타겟에게 이동
/// </summary>
public class StartMoveToNextBlock : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        _controller.StartMoveToNextBlock();
        return false;
    }
}
#endregion
#region Attack
/// <summary>
/// NOTE : 지정한 거리에 타겟 존재 확인
/// </summary>
public class CheckAttackRangeCondition : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        if (_controller.CheckAttackRangeCondition())
            return true;
        return false;
    }
}

public class LookAtTarget : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
       return _controller.LookAtTarget();
    }
}


/// <summary>
/// NOTE : 공격 가능한 상태인지 체크
/// </summary>
public class CheckAttackCondition : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        return _controller.CheckAttackCondition();
    }
}

/// <summary>
/// NOTE : 공격 시작
/// </summary>
public class StartAttack : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        _controller.AttackAction();
        return false;
    }
}
#endregion

#region skill
/// <summary>
/// NOTE : 스킬 사용이 가능한지 체크
/// </summary>
public class CheckSkillCondition : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        return _controller.CheckSkillCondition();
    }
}

/// <summary>
/// NOTE : 스킬 ACTION 
/// </summary>
public class SkillAction : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        _controller.SkillAction();
        return false;
    }
}
#endregion

#region Dead
public class IsDie : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        return _controller.IsDie();
    }
}

/// <summary>
/// NOTE : 죽음
/// </summary>
public class DeadAction : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        _controller.DeadAction();
        return true;
    }
}
#endregion

#region Victory
public class CheckVictory : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        return _controller.CheckVictroy();
    }
}

public class StartVictoryAnimation : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        _controller.StartVictoryAnimation();
        return true;
    }
}

#endregion

#region Anim
public class SetAnimation : ActionNode
{
    public UnitController Controller
    {
        set { _controller = value; }
    }
    private UnitController _controller;

    public override bool Invoke()
    {
        _controller.SetAnimation();
        return false;
    }
}
#endregion
