using UnityEngine;
using UnityEngine.UI;

public class GridRenderer : MonoBehaviour
{
    public Grid m_grid;

    public SlotRenderer m_slot;
    public GameObject m_line;

    private SlotRenderer[] m_slots;

    public void Build(Grid grid)
    {
        m_grid = grid;
        m_slots = new SlotRenderer[grid.m_slots.Length];

        for (int i = 0; i != Grid.GRID_SIZE; i++)
        {
            GameObject line = Instantiate(m_line);
            line.transform.SetParent(this.transform, false);

            for (int j = 0; j != Grid.GRID_SIZE; j++)
            {
                SlotRenderer slotRenderer = Instantiate(m_slot);
                slotRenderer.transform.SetParent(line.transform, false);

                Slot slot = new Slot(i, j);
                int slotIndex = slot.GetIndex();

                m_slots[slotIndex] = slotRenderer;
                m_grid.m_slots[slotIndex] = slot;

                slotRenderer.Init(slot);               
            }
        }
    }

    public void SetNumberOnSlot(int number, int slotIndex)
    {
        m_slots[slotIndex].SetNumber(number);
    }
}
