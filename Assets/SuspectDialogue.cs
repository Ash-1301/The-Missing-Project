// ============================================================
//  SuspectDialogue.cs  —  The Missing Project
//  Attach this to each suspect (NPC) in your scene.
//  Fill in their name, greeting, and questions in the Inspector.
//
//  HOW TO USE:
//  1. Click a suspect object in the Hierarchy
//  2. Click Add Component -> SuspectDialogue
//  3. Fill in all the fields in the Inspector
//  4. Also add SuspectInteract.cs to the same object
// ============================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SuspectDialogue : MonoBehaviour
{
    // ============================================================
    //  A class representing one question the player can ask
//  [System.Serializable] makes it show up in the Inspector
    // ============================================================
    [System.Serializable]
    public class DialogueQuestion
    {
        // The question text shown on the button
        // e.g. "Where were you that night?"
        public string questionText;

        // The suspect's answer to this question
        public string answerText;

        // Which clue ID must be found before this question unlocks
        // Set to 0 if the question is always available
        public int requiresClueID = 0;

        // The name of a flag to set in GameState when asked
        // e.g. "reidDeniedUSB" — leave blank if no flag needed
        public string flagToSet = "";

        // Has this question already been asked?
        // Asked questions disappear from the button list
        [HideInInspector] // Hides this in Inspector since we set it automatically
        public bool hasBeenAsked = false;
    }

    // ============================================================
    //  SUSPECT SETTINGS — fill these in the Inspector
    // ============================================================

    [Header("Suspect Info")]
    // The suspect's display name shown in the dialogue box
    public string suspectName = "Unknown";

    // What the suspect says when the player first talks to them
    public string greetingText = "Can I help you?";

    // Optional portrait image — drag a Sprite here if you have one
    public Sprite portrait;

    [Header("Questions")]
    // The list of questions for this suspect
    // In the Inspector you can add as many as you want
    // Click the + button next to the list to add a new question
    public List<DialogueQuestion> questions = new List<DialogueQuestion>();

    // ============================================================
    //  GetAvailableQuestions — returns questions the player can ask
    //  Only shows questions where the required clue has been found
    //  and the question hasn't been asked yet
    // ============================================================
    public List<DialogueQuestion> GetAvailableQuestions(GameState gameState)
    {
        // This will hold the questions we are allowed to show
        List<DialogueQuestion> available = new List<DialogueQuestion>();

        // Check each question one by one
        foreach (DialogueQuestion q in questions)
        {
            // Skip this question if it has already been asked
            if (q.hasBeenAsked) continue;

            // If the question requires a clue (ID > 0), check if found
            if (q.requiresClueID > 0)
            {
                // If GameState exists and the clue has been found, add it
                if (gameState != null && gameState.HasClue(q.requiresClueID))
                {
                    available.Add(q);
                }
                // Otherwise skip this question — clue not found yet
            }
            else
            {
                // requiresClueID is 0 — no clue needed, always available
                available.Add(q);
            }
        }

        return available;
    }
}