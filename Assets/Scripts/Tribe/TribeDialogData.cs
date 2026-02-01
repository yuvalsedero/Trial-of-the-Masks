using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Tribe/Tribe Dialog")]
public class TribeDialogData : ScriptableObject
{
    public TribeType tribeType;

    [Header("Cost")]
    public int meatCost = 0;

    [Header("Generic (always first)")]
    [TextArea(3, 6)]
    public List<string> genericLines;

    [Header("Not enough meat")]
    [TextArea(2, 4)]
    public List<string> notEnoughMeatLines;

    [Header("Completed")]
    [TextArea(2, 4)]
    public List<string> completedLines;
}
