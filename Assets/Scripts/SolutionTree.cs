using System.Collections.Generic;
using UnityEngine;

public class SolutionTree
{
    //maximum height (or depth) of the tree, i.e the maximum number of nodes allowed along a path
    //public int m_maximumHeight { get; set; }
    //public bool m_maximumHeightReached { get; set; } //did we reach a leaf node of this tree

    private List<SolutionNode> m_successNodes;

    public Grid m_grid;
    public Slot m_startSlot; //the slot that serves as start point for or tree    
    
    public bool m_stopWhenTreeIsSolved; //In case we just want to know if at least one solution exists, we can stop processing nodes as soon as we found one
    public bool m_isSolved; //is this tree solved (i.e contains at least one successful path to target)

    public bool m_isProcessingNewBranch;

    private long m_processedNodesCount;
    private long m_processedBranchesCount;

    public SolutionTree(Grid grid, Slot startSlot)
    {
        m_grid = grid;
        m_startSlot = startSlot;
        m_successNodes = new List<SolutionNode>();
        m_stopWhenTreeIsSolved = true;
        m_isSolved = false;

        m_processedNodesCount = 0;
        m_processedBranchesCount = 0;
    }

    //filters to know which types of solutions we want
    public const int ONE_SOLUTION = 1; //the solution that requires the less movements
    public const int ALL_SOLUTIONS = 2;

    /**
    * Traverse the floor for all directions possible at each step making the tree grow until each path has reached the maximum height of the tree or
    * found a success path
    **/
    public SolutionNode[][] SearchForSolutions(int bFilters = ONE_SOLUTION)
    {
        m_isProcessingNewBranch = true;

        SolutionNode startNode = new SolutionNode(this, m_startSlot, Grid.ConstrainedDirection.LEFT, null);
        startNode.Split();

        //now search for paths that are marked as successful and return them
        return ExtractSuccessPaths(bFilters);
    }

    public void AddSuccessNode(SolutionNode node)
    {
        m_successNodes.Add(node);
    }

    public void IncrementProcessedNodesCount()
    {
        m_processedNodesCount++;
    }

    public void IncrementProcessedBranchesCount()
    {
        m_processedBranchesCount++;
    }

    public long GetProcessedNodesCount()
    {
        return m_processedNodesCount;
    }

    public long GetProcessedBranchesCount()
    {
        return m_processedBranchesCount;
    }

    private SolutionNode[][] ExtractSuccessPaths(int bFilters = ONE_SOLUTION)
    {
        if (m_successNodes.Count == 0)
            return null;        

        if ((bFilters & ALL_SOLUTIONS) > 0)
        {
            SolutionNode[][] allSuccessPaths = new SolutionNode[m_successNodes.Count][];
            for (int i = 0; i != m_successNodes.Count; i++)
            {
                SolutionNode node = m_successNodes[i];
                allSuccessPaths[i] = ExtractPathFromNode(node);
            }

            return allSuccessPaths;
        }
        else if ((bFilters & ONE_SOLUTION) > 0)
        {
            SolutionNode[][] successPath = new SolutionNode[1][];
            successPath[0] = ExtractPathFromNode(m_successNodes[m_successNodes.Count - 1]);

            return successPath;
        }

        return null;
    }

    /**
    * Return the full path with 'node' parameter a leaf node
    **/
    private SolutionNode[] ExtractPathFromNode(SolutionNode node)
    {
        int pathLength = 0;
        SolutionNode tmpNode = node;
        while (tmpNode != null)
        {
            pathLength++;
            tmpNode = tmpNode.m_parentNode;
        }

        SolutionNode[] path = new SolutionNode[pathLength];

        while (node != null)
        {
            path[pathLength - 1] = node;
            pathLength--;
            node = node.m_parentNode;
        }

        return path;
    }
}

/**
* Node of the above tree that contains various informations such as:
* -a reference to a brick that will simulate the rolling operation over the currently edited level floor
* -the direction of the rolling operation to perform
* -the height of the node inside the tree, tree base nodes have height = 0
**/
public class SolutionNode
{
    public Slot m_slot;
    public Grid.ConstrainedDirection m_direction;
    public SolutionNode m_parentNode; //store the parent node to so we can navigate inside the tree along a path from bottom to top
    //public int m_distanceFromRoot; //the distance from root of this node, must be between 0 and (parentTree.MaximumHeight - 1)
    //public Brick m_brick; //a brick object that will simulate the rolling operation
    //public Tile[] m_coveredTiles; //as a brick object can be reused across several nodes, store here the tiles that are covered before the brick rolled

    public SolutionTree m_parentTree;

    public SolutionNode(SolutionTree parentTree, Slot slot, Grid.ConstrainedDirection direction, SolutionNode parentNode/*, int distanceFromRoot, Brick brick*/)
    {
        m_parentTree = parentTree;
        m_slot = slot;
        m_direction = direction;
        m_parentNode = parentNode;
        //m_distanceFromRoot = distanceFromRoot;
        //m_brick = brick;
        //m_coveredTiles = new Tile[2];
        //m_coveredTiles[0] = brick.CoveredTiles[0];
        //m_coveredTiles[1] = brick.CoveredTiles[1];
    }

    public void Process()
    {
        if (m_parentTree.m_stopWhenTreeIsSolved && m_parentTree.m_isSolved)
            return;

        m_parentTree.IncrementProcessedNodesCount();  
        int slotNumber = m_parentNode.m_slot.m_number + 1;

        if (m_parentTree.m_isProcessingNewBranch)
        {
            //clear slots that have a higher number than this slot number
            m_parentTree.m_grid.ClearSlots(slotNumber);

            m_parentTree.m_isProcessingNewBranch = false;
        }
        
        m_slot.m_number = slotNumber;

        if (!Split())
        {
            if (slotNumber > 99)
            {
                m_parentTree.AddSuccessNode(this);
                m_parentTree.m_isSolved = true;
            }
            
            m_parentTree.m_isProcessingNewBranch = true;
            m_parentTree.IncrementProcessedBranchesCount();
        }
    }

    /**
    * Split the current node in the eight possible directions
    * if at least one direction is valid return true, otherwise return false
    **/
    public bool Split()
    {
        SolutionNode[] childNodes = new SolutionNode[8];

        int i = 0;
        GenerateNodeForDirection(Grid.ConstrainedDirection.LEFT, childNodes, ref i);
        GenerateNodeForDirection(Grid.ConstrainedDirection.TOP_LEFT, childNodes, ref i);
        GenerateNodeForDirection(Grid.ConstrainedDirection.TOP, childNodes, ref i);
        GenerateNodeForDirection(Grid.ConstrainedDirection.TOP_RIGHT, childNodes, ref i);
        GenerateNodeForDirection(Grid.ConstrainedDirection.RIGHT, childNodes, ref i);
        GenerateNodeForDirection(Grid.ConstrainedDirection.BOTTOM_RIGHT, childNodes, ref i);
        GenerateNodeForDirection(Grid.ConstrainedDirection.BOTTOM, childNodes, ref i);
        GenerateNodeForDirection(Grid.ConstrainedDirection.BOTTOM_LEFT, childNodes, ref i);

        if (i == 0)
            return false;

        i = 0;
        while (i < childNodes.Length && childNodes[i] != null)
        {
            childNodes[i].Process();
            i++;
        }

        return true;

        //SolutionNode[] childNodes = new SolutionNode[3];

        //if (m_direction == Brick.RollDirection.LEFT)
        //{
        //    childNodes[0] = new SolutionNode(m_parentTree, Brick.RollDirection.LEFT, this, m_distanceFromRoot + 1, m_brick);
        //    childNodes[1] = new SolutionNode(m_parentTree, Brick.RollDirection.TOP, this, m_distanceFromRoot + 1, new Brick(m_brick));
        //    childNodes[2] = new SolutionNode(m_parentTree, Brick.RollDirection.BOTTOM, this, m_distanceFromRoot + 1, new Brick(m_brick));
        //}
        //else if (m_direction == Brick.RollDirection.RIGHT)
        //{
        //    childNodes[0] = new SolutionNode(m_parentTree, Brick.RollDirection.RIGHT, this, m_distanceFromRoot + 1, m_brick);
        //    childNodes[1] = new SolutionNode(m_parentTree, Brick.RollDirection.TOP, this, m_distanceFromRoot + 1, new Brick(m_brick));
        //    childNodes[2] = new SolutionNode(m_parentTree, Brick.RollDirection.BOTTOM, this, m_distanceFromRoot + 1, new Brick(m_brick));
        //}
        //else if (m_direction == Brick.RollDirection.TOP)
        //{
        //    childNodes[0] = new SolutionNode(m_parentTree, Brick.RollDirection.LEFT, this, m_distanceFromRoot + 1, m_brick);
        //    childNodes[1] = new SolutionNode(m_parentTree, Brick.RollDirection.RIGHT, this, m_distanceFromRoot + 1, new Brick(m_brick));
        //    childNodes[2] = new SolutionNode(m_parentTree, Brick.RollDirection.TOP, this, m_distanceFromRoot + 1, new Brick(m_brick));
        //}
        //else
        //{
        //    childNodes[0] = new SolutionNode(m_parentTree, Brick.RollDirection.LEFT, this, m_distanceFromRoot + 1, m_brick);
        //    childNodes[1] = new SolutionNode(m_parentTree, Brick.RollDirection.RIGHT, this, m_distanceFromRoot + 1, new Brick(m_brick));
        //    childNodes[2] = new SolutionNode(m_parentTree, Brick.RollDirection.BOTTOM, this, m_distanceFromRoot + 1, new Brick(m_brick));
        //}
    }

    private SolutionNode GenerateNodeForDirection(Grid.ConstrainedDirection direction, SolutionNode[] nodes, ref int index)
    {
        Slot nextSlot = m_parentTree.m_grid.GetSlotForDirection(m_slot, direction);

        if (nextSlot != null && nextSlot.m_number == 0)
        {
            nodes[index] = new SolutionNode(m_parentTree, nextSlot, direction, this);
            index++;
        }

        return null;
    }
}