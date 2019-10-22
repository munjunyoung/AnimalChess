using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBTAI : UnitBTBase
{
    private UnitController unitController;
    //최상위 
    private Sequence aiRoot = new Sequence();

    private Selector selector = new Selector();

    private Sequence seqMove = new Sequence();
    private Sequence seqAttack = new Sequence();
    private Sequence seqDead = new Sequence();
    private Sequence seqAnim = new Sequence();
    //Move
    private SetTargetBlock              setTargetblock              = new SetTargetBlock();
    private ResetPath                   resetPath                   = new ResetPath();
    private CheckMoveState              checkMoveState              = new CheckMoveState();
    private StartMoveToNextBlock        startMoveToNextBlock        = new StartMoveToNextBlock();
    //AtTack
    private CheckAttackRangeCondition   checkAttackRangeCondition   = new CheckAttackRangeCondition();
    private LookAtTarget                lookAtTarget                = new LookAtTarget();
    //Dead
    private IsDie                   isDie                       = new IsDie();

    //Anim
    private SetAnimation            setAnimation                = new SetAnimation();

    private IEnumerator behaviorProcess;

    public override void Init()
    {
        behaviorProcess = BehaviorProcess();
        //컨트롤러 초기화
        unitController = GetComponent<UnitController>();
        unitController.Init();
        //Move
        setTargetblock.Controller = unitController;
        resetPath.Controller = unitController;
        checkMoveState.Controller = unitController;
        startMoveToNextBlock.Controller = unitController;
        //Attack
        checkAttackRangeCondition.Controller = unitController;
        lookAtTarget.Controller = unitController;
        //Dead
        isDie.Controller = unitController;
        //Anim
        setAnimation.Controller = unitController;

        aiRoot.AddChild(selector);
        //Tree 생성
        selector.AddChild(seqMove);
        selector.AddChild(seqDead);
        selector.AddChild(seqAnim);
        selector.AddChild(seqAttack);

        seqMove.AddChild(setTargetblock);
        seqMove.AddChild(resetPath);
        seqMove.AddChild(checkMoveState);
        seqMove.AddChild(startMoveToNextBlock);

        seqAttack.AddChild(checkAttackRangeCondition);
        seqAttack.AddChild(lookAtTarget);

        seqDead.AddChild(isDie);

        seqAnim.AddChild(setAnimation);
    }
    
    public override void StartBT()
    {
        StartCoroutine(behaviorProcess);
    }

    public override void StopBT()
    {
        StopCoroutine(behaviorProcess);
    }

    public override IEnumerator BehaviorProcess()
    {
        while (!aiRoot.Invoke())
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Exit Process");
    }
}
