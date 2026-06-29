using UnityEngine;
using System.Collections.Generic;

public static class CraftSlotManager
{
    public static readonly List<UICraftingSlot> Slots = new List<UICraftingSlot>();

    public static void AddSlot(UICraftingSlot slot)
    {
        if (!Slots.Contains(slot))
            Slots.Add(slot);
    }

    public static void RemoveSlot(UICraftingSlot slot)
    {
        Slots.Remove(slot);
    }
}
