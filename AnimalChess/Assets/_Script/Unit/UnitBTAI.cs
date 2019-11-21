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
    private Sequence seqVictory = new Sequence();
    private Sequence seqAnim = new Sequence();
    //Move
    private SetTargetBlock              setTargetblock              = new SetTargetBlock();
    private SetPath                     setPath                     = new SetPath();
    private CheckMoveState              checkMoveState              = new CheckMoveState();
    private StartMoveToNextBlock        startMoveToNextBlock        = new StartMoveToNextBlock();
    //Attack
    private CheckAttackRangeCondition   checkAttackRangeCondition   = new CheckAttackRangeCondition();
    private LookAtTarget                lookAtTarget                = new LookAtTarget();
    private CheckAttackCondition        checkAttackCondition        = new CheckAttackCondition();
    private StartAttack                 startAttack                 = new StartAttack();
    //Dead
    private IsDie                       isDie                       = new IsDie();
    private DeadAction                  deadAction                  = new DeadAction();

    private CheckVictory                checkVictory                = new CheckVictory();
    private StartVictoryAnimation       startVictoryAnimation       = new StartVictoryAnimation();
    //Anim
    private SetAnimation                setAnimation                = new SetAnimation();

    private IEnumerator behaviorProcess;

    public override void Init()
    {
        behaviorProcess = BehaviorProcess();
        //컨트롤러 초기화
        unitController = GetComponent<UnitController>();
        unitController.Init();
        //Move
        setTargetblock.Controller = unitController;
        setPath.Controller = unitController;
        checkMoveState.Controller = unitController;
        startMoveToNextBlock.Controller = unitController;
        //Attack
        checkAttackRangeCondition.Controller = unitController;
        lookAtTarget.Controller = unitController;
        checkAttackCondition.Controller = unitController;
        startAttack.Controller = unitController;
        //Dead
        isDie.Controller = unitController;
        deadAction.Controller = unitController;
        //Victory
        checkVictory.Controller = unitController;
        startVictoryAnimation.Controller = unitController;
        
        //Anim
        setAnimation.Controller = unitController;


        aiRoot.AddChild(selector);
        //Tree 생성
        selector.AddChild(seqMove);
        selector.AddChild(seqAttack);

        selector.AddChild(seqAnim);
        selector.AddChild(seqDead);
        selector.AddChild(seqVictory);

        seqMove.AddChild(setTargetblock);
        seqMove.AddChild(setPath);
        seqMove.AddChild(checkMoveState);
        seqMove.AddChild(startMoveToNextBlock);

        seqAttack.AddChild(checkAttackRangeCondition);
        seqAttack.AddChild(lookAtTarget);
        seqAttack.AddChild(checkAttackCondition);
        seqAttack.AddChild(startAttack);

        seqDead.AddChild(isDie);
        seqDead.AddChild(deadAction);

        seqVictory.AddChild(checkVictory);
        seqVictory.AddChild(startVictoryAnimation);

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
