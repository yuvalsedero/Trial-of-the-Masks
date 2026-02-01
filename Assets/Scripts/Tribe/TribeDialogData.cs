using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Tribe/Tribe Dialog")]
public class TribeDialogData : ScriptableObject
{
    public TribeType tribeType;

    [Header("Generic (always first)")]
    [TextArea(3, 6)]
    public List<string> genericLines;

    [Header("Completed")]
    [TextArea(2, 4)]
    public List<string> completedLines;
}
