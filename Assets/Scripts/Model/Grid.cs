using UnityEngine;

public class Grid
{
    public const int GRID_SIZE = 10; //10x10 grid
    public Slot[] m_slots;

    public enum ConstrainedDirection
    {
        LEFT,
        TOP_LEFT,
        TOP,
        TOP_RIGHT,
        RIGHT,
        BOTTOM_RIGHT,
        BOTTOM,
        BOTTOM_LEFT
    }

    public Grid()
    {
        m_slots = new Slot[GRID_SIZE * GRID_SIZE];
    }

    public Slot GetSlotForDirection(Slot currentSlot, ConstrainedDirection direction)
    {
        int slotLine, slotColumn;
        switch (direction)
        {
            case ConstrainedDirection.LEFT:
                slotLine = currentSlot.m_line;
                slotColumn = currentSlot.m_column - 3;
                break;
            case ConstrainedDirection.TOP_LEFT:
                slotLine = currentSlot.m_line - 2;
                slotColumn = currentSlot.m_column - 2;
                break;
            case ConstrainedDirection.TOP:
                slotLine = currentSlot.m_line - 3;
                slotColumn = currentSlot.m_column;
                break;
            case ConstrainedDirection.TOP_RIGHT:
                slotLine = currentSlot.m_line - 2;
                slotColumn = currentSlot.m_column + 2;
                break;
            case ConstrainedDirection.RIGHT:
                slotLine = currentSlot.m_line;
                slotColumn = currentSlot.m_column + 3;
                break;
            case ConstrainedDirection.BOTTOM_RIGHT:
                slotLine = currentSlot.m_line + 2;
                slotColumn = currentSlot.m_column + 2;
                break;
            case ConstrainedDirection.BOTTOM:
                slotLine = currentSlot.m_line + 3;
                slotColumn = currentSlot.m_column;
                break;
            case ConstrainedDirection.BOTTOM_LEFT:
                slotLine = currentSlot.m_line + 2;
                slotColumn = currentSlot.m_column - 2;
                break;
            default:
                slotLine = -1;
                slotColumn = -1;
                break;
        }

        if (slotLine >= 0 && slotLine < GRID_SIZE && slotColumn >= 0 && slotColumn < GRID_SIZE)
        {
            int slotIndex = slotLine * GRID_SIZE + slotColumn;
            return m_slots[slotIndex];
        }

        return null;
    }

    public Slot GetSlotForNumber(int number)
    {
        for (int i = 0; i != m_slots.Length; i++)
        {
            if (m_slots[i].m_number == number)
                return m_slots[i];
        }

        return null;
    }

    /**
    * Clear slots that have a higher number than the 'above' parameter
    **/
    public void ClearSlots(int above = 0)
    {
        for (int i = 0; i != m_slots.Length; i++)
        {
            if (m_slots[i].m_number >= above)
            {
                m_slots[i].Clear();
            }
        }
    }
}

public class Slot
{
    public int m_line;
    public int m_column;
    
    public int m_number; //a striclty positive number between 1 and 100 if this slot is not empty

    public Slot(int line, int column)
    {
        m_line = line;
        m_column = column;

        m_number = 0;
    }

    public int GetIndex()
    {
        return m_line * Grid.GRID_SIZE + m_column;
    }

    public void Clear()
    {
        m_number = 0;
    }

    public override string ToString()
    {
        if (m_number > 0)
            return "Slot (" + m_column + ", " + m_line + ")" + " num:" + m_number;
        else
            return "Slot (" + m_column + ", " + m_line + ")";
    }
}
