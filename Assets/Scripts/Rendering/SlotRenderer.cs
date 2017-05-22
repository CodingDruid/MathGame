using UnityEngine;
using UnityEngine.UI;

public class SlotRenderer : MonoBehaviour
{
    public Slot m_slot { get; set; }

    public void Init(Slot slot)
    {
        m_slot = slot;
    }

    public void SetNumber(int number)
    {
        m_slot.m_number = number;
        this.GetComponentInChildren<Text>().text = number.ToString();
    }
}
