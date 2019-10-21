using UnityEngine;

public class UnitBlockData : MonoBehaviour
{
    [HideInInspector]
    public  UnitPropertyData unitPdata;
    [HideInInspector]
    public  UnitController   unitController;
    [HideInInspector]
    public UnitBTAI          unitBTAI;

    private BlockOnBoard    currentBlockInWaiting;
    private BlockOnBoard    currentBlockInBattle;

    private void Awake()
    {
        unitController = GetComponent<UnitController>();
        unitBTAI = GetComponent<UnitBTAI>();
        unitBTAI.Init();
    }
    //private ownBlock 
    public void SetCurrentBlockInWaiting(BlockOnBoard _currentBlock)
    {
        currentBlockInWaiting = _currentBlock;
        currentBlockInBattle = currentBlockInWaiting;
    }
    public BlockOnBoard GetCurrentBlockInWaiting() { return currentBlockInWaiting; }

    public void SetCurrentBlockInBattle(BlockOnBoard _currentBlock)
    {
        currentBlockInBattle = _currentBlock;
    }

    public BlockOnBoard GetCurrentBlockInBattle() { return currentBlockInBattle; }
}
