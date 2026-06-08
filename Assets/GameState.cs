// ============================================================
//  GameState.cs  —  The Missing Project
//  Tracks ALL game progress: clues found, dialogue flags,
//  and the final accusation.
//
//  HOW TO USE:
//  1. In Hierarchy, right-click -> Create Empty
//  2. Rename it "GameState"
//  3. Drag this script onto it
//  Only ONE GameState should exist in your scene.
// ============================================================

using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    // ============================================================
    //  A small class to store one clue's data
    //  Like a notecard with the clue's info written on it
    // ============================================================
    [System.Serializable]
    public class ClueData
    {
        public int id;
        public string name;
        public string description;
    }

    // ============================================================
    //  CLUE TRACKING
    // ============================================================

    // List of all clues the player has collected so far
    public List<ClueData> collectedClues = new List<ClueData>();

    // Total clues in the game (used for ending checks)
    public int totalClues = 5;

    // ============================================================
    //  DIALOGUE FLAGS
    //  Each flag is a true/false switch set during conversations
    //  The ending system reads these to decide the outcome
    // ============================================================

    // Dr. Reid flags
    public bool reidAskedAboutUSB = false;
    public bool reidDeniedUSB = false;
    public bool reidActedNervous = false;

    // Maya Santos flags
    public bool mayaAskedAboutNight = false;
    public bool mayaAdmittedPresence = false;
    public bool mayaMentionedPressure = false;

    // Prof. Hale flags
    public bool haleConfrontedAboutGrant = false;
    public bool haleDeflected = false;
    public bool haleKnewAboutDrive = false;

    // ============================================================
    //  ACCUSATION TRACKING
    // ============================================================

    // Who the player accused at the end
    public string accusedSuspect = "";

    // Change this to whoever you want the real culprit to be
    // Options: "DrReid" / "Maya" / "ProfHale"
    public string trueCulprit = "ProfHale";

    // ============================================================
    //  CollectClue — called by CluePickup when a clue is found
    // ============================================================
    public void CollectClue(int id, string name, string description)
    {
        // Check if we already have this clue
        foreach (ClueData existing in collectedClues)
        {
            if (existing.id == id)
            {
                Debug.Log("Clue " + id + " already collected.");
                return;
            }
        }

        // Create and store the new clue
        ClueData newClue = new ClueData();
        newClue.id = id;
        newClue.name = name;
        newClue.description = description;
        collectedClues.Add(newClue);

        Debug.Log("Clue collected: " + name + " (" + collectedClues.Count + "/" + totalClues + ")");
    }

    // ============================================================
    //  HasClue — checks if a specific clue has been found
    //  Used by SuspectDialogue to unlock new questions
    //  Example: if (gameState.HasClue(3)) { show question }
    // ============================================================
    public bool HasClue(int id)
    {
        foreach (ClueData clue in collectedClues)
        {
            if (clue.id == id) return true;
        }
        return false;
    }

    // ============================================================
    //  CluesFound — returns how many clues have been collected
    //  Used by EndingManager to pick the right ending
    // ============================================================
    public int CluesFound()
    {
        return collectedClues.Count;
    }

    // ============================================================
    //  SetFlag — sets a named flag to true
    //  Called by DialogueManager when a question is asked
    //  The flagName matches the variable names above exactly
    //  Example: SetFlag("reidDeniedUSB") sets reidDeniedUSB = true
    // ============================================================
    public void SetFlag(string flagName)
    {
        // Each "case" matches a flag variable name
        // When matched, that variable is set to true
        switch (flagName)
        {
            // Dr. Reid flags
            case "reidAskedAboutUSB":     reidAskedAboutUSB = true;     break;
            case "reidDeniedUSB":         reidDeniedUSB = true;         break;
            case "reidActedNervous":      reidActedNervous = true;      break;

            // Maya flags
            case "mayaAskedAboutNight":   mayaAskedAboutNight = true;   break;
            case "mayaAdmittedPresence":  mayaAdmittedPresence = true;  break;
            case "mayaMentionedPressure": mayaMentionedPressure = true; break;

            // Prof. Hale flags
            case "haleConfrontedAboutGrant": haleConfrontedAboutGrant = true; break;
            case "haleDeflected":         haleDeflected = true;         break;
            case "haleKnewAboutDrive":    haleKnewAboutDrive = true;    break;

            default:
                // If the flag name doesn't match anything, print a warning
                Debug.LogWarning("GameState: Unknown flag name: " + flagName);
                break;
        }

        Debug.Log("Flag set: " + flagName);
    }

    // ============================================================
    //  Accuse — called when the player makes their final accusation
    // ============================================================
    public void Accuse(string suspectName)
    {
        accusedSuspect = suspectName;
        Debug.Log("Player accused: " + suspectName);
    }
}