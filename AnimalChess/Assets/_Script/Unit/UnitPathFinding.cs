using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPathFinding
{
    private List<PathNode> openNodeList = new List<PathNode>();
    private List<PathNode> closeNodeList = new List<PathNode>();
    
    public PathNode ChangeBlockToNode(BlockOnBoard block)
    {
        var tmpnode = new PathNode(new Vector2Int((int)(block.transform.position.x * 0.5f), (int)(block.transform.position.z * 0.5f)));
        return tmpnode;
    }
    /// <summary>
    /// NOTE : 길을 찾지 못할경우 FALSE,
    /// </summary>
    /// <param name="startnode"></param>
    /// <param name="endnode"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool FindPath(BlockOnBoard startblock, BlockOnBoard endblock, BlockOnBoard[,] map, ref List<BlockOnBoard> path, float attackRange)
    {
        var startnode = new PathNode(new Vector2Int((int)(startblock.groundArrayIndex.x), (int)(startblock.groundArrayIndex.y)));
        var endnode  = new PathNode(new Vector2Int((int)(endblock.groundArrayIndex.x), (int)(endblock.groundArrayIndex.y)));
        //리스트 클리어
        openNodeList.Clear();
        closeNodeList.Clear();
        //시작 노드 설정
        PathNode currentNode = startnode;
        openNodeList.Add(currentNode);
        //depth 설정
        int currentdepth = 0;
        currentNode.depth = currentdepth;
        //이웃노드
        List<PathNode> neighborNodes = new List<PathNode>();
        //총 횟수 설정하기 위함
        int count = 0;
        while(count < 300)
        {
            count++;

            if (openNodeList.Count == 0)
                break;
            //가장 처음 노드 초기화 및 리스트 제거 
            currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            //목적지와 현재 노드가 같다면 처리 리스트 역순으로 추가
            if(DistanceNode(currentNode, endnode, attackRange))
            {
                while (currentNode != null)
                {
                    path.Add(map[currentNode.arrayIndex.x,currentNode.arrayIndex.y]);
                    currentNode = currentNode.parentNode;
                }
                return true;
            }
            //목표점이 아닌 경우 닫힌 NODE 리스트 추가
            closeNodeList.Add(currentNode);

            ++currentdepth;//깊이 증가
            GetNeighborNode(currentNode,ref neighborNodes);
            //이웃 노드 추가 
            
            foreach(var nnode in neighborNodes)
            {
                nnode.parentNode = currentNode;
                nnode.CalcDist(endnode, currentdepth);
                InsertOpenNode(nnode);
            }

            SortOpenNode();

        }
        return false;
    }
    private bool DistanceNode(PathNode currentnode, PathNode endnode, float range)
    {
        if (Vector2Int.Distance(currentnode.arrayIndex, endnode.arrayIndex) < range)
            return true;
        
        return false;
    }

    /// <summary>
    /// NOTE : 중복 노드 삽입 되지 않도록 처리, 이미 중복된 노드들을 비용을 체크하여 변경)
    /// </summary>
    /// <param name="addNode"></param>
    private void InsertOpenNode(PathNode addNode)
    {
        //인덱스 검색 없을 경우 -1
        var overlapNodeindex = openNodeList.FindIndex(x => x.arrayIndex.Equals(addNode.arrayIndex));
        if(overlapNodeindex!=-1)
            openNodeList[overlapNodeindex] = CompareNodeG(openNodeList[overlapNodeindex], addNode) ? openNodeList[overlapNodeindex] : addNode;
        else
            openNodeList.Add(addNode);
    }

    /// <summary>
    /// NOTE : OPEN NODE SORT ( 버블 정렬 )
    /// </summary>
    private void SortOpenNode()
    {
        //노드가 1개이하
        if (openNodeList.Count < 2)
            return;
        bool bcontinue = true;

        while (bcontinue)
        {
            bcontinue = false;
            for (int i = 0; i < openNodeList.Count - 1; i++)
            {
                if (CompareNodeF(openNodeList[i], openNodeList[i + 1]))
                {
                    PathNode tmpnode = openNodeList[i];
                    openNodeList[i] = openNodeList[i + 1];
                    openNodeList[i + 1] = tmpnode;
                    bcontinue = true;
                }
            }
        }
    }
    /// <summary>
    /// NOTE : n1이 n2보다 저비용일 경우 true ( 거리가 까갑거나 탐색깊이가 적음)
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    private bool CompareNodeF(PathNode n1, PathNode n2)
    {
        if (n1.distF > n2.distF)
            return true;
        if (n1.distF < n2.distF)
            return false;
        //distF가 같은 값일 경우 depth 비교 (depth가 더 크면 변경)
        if (n1.depth > n2.depth)
            return true;

        return false;
    }

    /// <summary>
    /// NOTE : 2개의 노드의 현재시작점에서의 비용 체크
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    private bool CompareNodeG(PathNode n1, PathNode n2)
    {
        if (n1.distG < n2.distG)
            return true;
        if (n1.distG > n2.distG)
            return false;
        if (n1.depth <= n2.depth)
            return true;
        return false;
    }
    
    /// <summary>
    /// NOTE : 이웃 검색
    /// </summary>
    /// <param name="currentnode"></param>
    /// <param name="nodelist"></param>
    public void GetNeighborNode(PathNode currentnode, ref List<PathNode> nodelist)
    {
        //노드 초기화
        nodelist.Clear();
        //원점을 기준으로 0,1 좌우 , 2,3 상하 
        for (int y = -1; y < 2; ++y)
        {
            for (int x = -1; x < 2; ++x)
            {
                //원점이면 제외
                if (x == 0 && y == 0)
                    continue;
                int tmpx = currentnode.arrayIndex.x + x;
                int tmpy = currentnode.arrayIndex.y + y;

                //해당 배열 인덱스가 음수일경우 제외
                if (tmpx < 0 || tmpy < 0)
                    continue;
                if (tmpx > (int)MAP_INFO.Width-1 || tmpy > (int)MAP_INFO.Height-1)
                    continue;     
                var tmparrayindex = new Vector2Int(tmpx, tmpy);
                if (closeNodeList.Exists(i => i.arrayIndex.Equals(tmparrayindex)))
                    continue;
                //이동불가 지역 필요 X
                var tmpblock = BoardManager.instance.allGroundBlocks[tmpx, tmpy];
                if (tmpblock != null)
                {
                    //유닛이 존재하거나 대기 블록일경우 리턴
                    if (tmpblock.GetUnitInBattle()!=null||tmpblock.IsWaitingBlock)
                        continue;
                }
                
                //이동가능한 지점이면 목록에 추가하기
                nodelist.Add(new PathNode(tmparrayindex));
            }
        }
    }
}

public class PathNode
{
    public Vector2Int arrayIndex; 
    public float distG; // 시작지점에서 현재 위치 까지 거리 
    public float distH; // 현재 위치에서 목적지까지 거리 
    public float distF; // G + H 총 값
    public int depth;
    public PathNode parentNode;

    public PathNode(Vector2Int nodepos)
    {
        arrayIndex = nodepos;
        distH = 0;
        distG = 0;
        distF = 0;
        depth = 0;
        parentNode = null;
    }
    /// <summary>
    /// NOTE : 유클리드
    /// </summary>
    /// <param name="destinationNode"></param>
    /// <param name="currentdepth"></param>
    public void CalcDist(PathNode destinationNode, int currentdepth)
    {
        int tmpHx = destinationNode.arrayIndex.x - arrayIndex.x;
        int tmpHy = destinationNode.arrayIndex.y - arrayIndex.y;
        distH = Mathf.Sqrt((tmpHx * tmpHx) + (tmpHy * tmpHy));

        int tmpGx = parentNode.arrayIndex.x - arrayIndex.x;
        int tmpGy = parentNode.arrayIndex.y - arrayIndex.y;
        float tmpG = Mathf.Sqrt((tmpGx * tmpGx) + (tmpGy * tmpGy));
        distG = tmpG + parentNode.distG;

        depth = currentdepth;
        distF = distG + distH;
    }
}

