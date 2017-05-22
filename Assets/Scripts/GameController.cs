using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public Canvas m_canvas;
    public GridRenderer m_grid;

    private static GameController s_instance;

    public static GameController GetInstance()
    {
        if (s_instance == null)
            s_instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        return s_instance;
    }

    void Start()
    {
        ShowGrid();
        //PlaceRandomFirstNumber();
        //FindSolution();

        TestAllPositions();
    }

    public void ShowGrid()
    {
        Grid grid = new Grid();

        m_grid = Instantiate(m_grid);
        m_grid.Build(grid);

        m_grid.transform.SetParent(m_canvas.transform, false);
    }

    private void PlaceRandomFirstNumber()
    {
        //Select a random slot
        int randNumber = Random.Range(0, 100);
        randNumber = 54;
        m_grid.SetNumberOnSlot(1, randNumber);
    }

    private System.Diagnostics.Stopwatch sw;
    private int m_startSlotIndex;
    private SolutionNode[] m_solution;

    private void TestAllPositions()
    {
        sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        m_startSlotIndex = 0;

        for (int i = 0; i != 10; i++)
        {
            //FindSolution();
            //OnFinishSolvingGrid();
            QueuedThreadedJobsManager threadManager = GetComponent<QueuedThreadedJobsManager>();
            threadManager.AddJob(new ThreadedJob(FindSolution, null, OnFinishSolvingGrid));
        }
    }

    private void FindSolution()
    {
        Grid grid = m_grid.m_grid;

        grid.ClearSlots();
        Slot startSlot = grid.m_slots[m_startSlotIndex];
        startSlot.m_number = 1;
        ++m_startSlotIndex;

        SolutionTree sTree = new SolutionTree(grid, startSlot);

        SolutionNode[][] solutions = sTree.SearchForSolutions();

        //if (solutions != null && solutions.Length == 1)
        //{
        //    m_solution = solutions[0];
        //}

        //Debug.Log(sTree.GetProcessedBranchesCount() + " branches and " + sTree.GetProcessedNodesCount() + " nodes processed");
    }

    public void OnFinishSolvingGrid()
    {        
        Debug.Log("Grid " + m_startSlotIndex + " Elapsed " + sw.ElapsedMilliseconds + " ms");
        sw.Stop();
        if (m_startSlotIndex < 99)
        {
            sw.Reset();
            sw.Start();
        }
        
        //for (int i = 0; i != m_solution.Length; i++)
        //{
        //    m_grid.SetNumberOnSlot((i + 1), m_solution[i].m_slot.GetIndex());
        //}
    }
}
