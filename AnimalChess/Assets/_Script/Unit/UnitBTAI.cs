using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBTAI : UnitBTBase
{
    private UnitController unitController;
    //최상위 
    private Sequence aiRoot = new Sequence();
    private Sequence seqMove = new Sequence();
    private Sequence seqDead = new Sequence();
    //Move
    private SetTargetBlock          setTargetblock              = new SetTargetBlock();
    private ResetPath               resetPath                   = new ResetPath();
    private CheckMoveState          checkMoveState              = new CheckMoveState();
    private StartMoveToNextBlock    startMoveToNextBlock        = new StartMoveToNextBlock();


    //Dead
    private IsDie isDie = new IsDie();

    private IEnumerator behaviorProcess;

    public override void Init()
    {
        behaviorProcess = BehaviorProcess();
        //컨트롤러 초기화
        unitController = GetComponent<UnitController>();
        unitController.Init();

        setTargetblock.Controller = unitController;
        resetPath.Controller = unitController;
        checkMoveState.Controller = unitController;
        startMoveToNextBlock.Controller = unitController;

        isDie.Controller = unitController;
        //Tree 생성
        aiRoot.AddChild(seqMove);
        aiRoot.AddChild(seqDead);

        seqMove.AddChild(setTargetblock);
        seqMove.AddChild(resetPath);
        seqMove.AddChild(checkMoveState);
        seqMove.AddChild(startMoveToNextBlock);

        seqDead.AddChild(isDie);
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
