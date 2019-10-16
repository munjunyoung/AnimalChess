using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitPropertyData unitPdata;

    private BlockOnBoard currentBlock;
    //private ownBlock =
    public void SetCurrentBlock(BlockOnBoard _currentBlock) { currentBlock = _currentBlock; }
    public BlockOnBoard GetCurrentBlock() { return currentBlock; }
}
