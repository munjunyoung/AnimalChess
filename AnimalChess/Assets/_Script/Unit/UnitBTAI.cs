using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBTAI : UnitBTBase
{
    private UnitController unitController;
    //최상위 
    private Sequence aiRoot     = new Sequence();

    private Selector selector   = new Selector();

    private Sequence seqMove    = new Sequence();
    private Sequence seqAttack  = new Sequence();
    private Sequence seqSkill   = new Sequence();
    private Sequence seqDead    = new Sequence();
    private Sequence seqVictory = new Sequence();
    private Sequence seqAnim    = new Sequence();
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
    //Skill
    private CheckCastingSkillCondition  checkCastingSkillCondition  = new CheckCastingSkillCondition();
    private CheckSkillCondition         checkSkillCondition         = new CheckSkillCondition();
    private StartSkill                  startSkill                  = new StartSkill();
    //Dead
    private IsDie                       isDie                       = new IsDie();
    private DeadAction                  deadAction                  = new DeadAction();
    //Victory
    private CheckVictory                checkVictory                = new CheckVictory();
    private StartVictoryAnimation       startVictoryAnimation       = new StartVictoryAnimation();
    //Anim
    private SetAnimation                setAnimation                = new SetAnimation();
    //Debuff
    private CheckStunning               checkStunning               = new CheckStunning();

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
        //Skill
        checkCastingSkillCondition.Controller = unitController;
        checkSkillCondition.Controller = unitController;
        startSkill.Controller = unitController;
        //Dead
        isDie.Controller = unitController;
        deadAction.Controller = unitController;
        //Victory
        checkVictory.Controller = unitController;
        startVictoryAnimation.Controller = unitController;
        
        //Anim
        setAnimation.Controller = unitController;
        //Debuff
        checkStunning.Controller = unitController;


        aiRoot.AddChild(selector);
        //Tree 생성
        selector.AddChild(seqMove);
        selector.AddChild(seqAttack);
        selector.AddChild(seqSkill);
        selector.AddChild(seqAnim);
        selector.AddChild(seqDead);
        selector.AddChild(seqVictory);
        //Move
        seqMove.AddChild(checkStunning);
        seqMove.AddChild(checkCastingSkillCondition);
        seqMove.AddChild(setTargetblock);
        seqMove.AddChild(setPath);
        seqMove.AddChild(checkMoveState);
        seqMove.AddChild(startMoveToNextBlock);
        //Attack
        seqAttack.AddChild(checkStunning);
        seqAttack.AddChild(checkAttackRangeCondition);
        seqAttack.AddChild(lookAtTarget);
        seqAttack.AddChild(checkAttackCondition);
        seqAttack.AddChild(startAttack);
        //Skill
        seqSkill.AddChild(checkStunning);
        seqSkill.AddChild(checkCastingSkillCondition);
        seqSkill.AddChild(checkSkillCondition);
        seqSkill.AddChild(checkAttackRangeCondition);
        seqSkill.AddChild(lookAtTarget);
        seqSkill.AddChild(startSkill);

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
