// ============================================================
//  SuspectInteract.cs  —  The Missing Project
//  Attach this to each suspect alongside SuspectDialogue.
//  This handles detecting when the player is close and
//  showing the "Press E to talk" prompt.
//
//  HOW TO USE:
//  1. Attach to the same suspect object as SuspectDialogue
//  2. Connect the Prompt UI and Prompt Text in the Inspector
//     (you can reuse the same PromptUI from CluePickup)
// ============================================================

using UnityEngine;
using TMPro;

public class SuspectInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    // How close the player must be to see the talk prompt
    public float interactionDistance = 3f;

    // The key to press to start talking
    public KeyCode interactKey = KeyCode.E;

    [Header("UI — reuse your existing PromptUI")]
    // The prompt panel (same one used for clues is fine)
    public GameObject promptUI;

    // The prompt text object
    public TextMeshProUGUI promptText;

    // ============================================================
    //  PRIVATE VARIABLES
    // ============================================================

    // The player's camera — used to measure distance
    private Camera playerCamera;

    // Is the prompt currently showing?
    private bool promptShowing = false;

    // The SuspectDialogue script on this same object
    // We get it automatically in Start
    private SuspectDialogue suspectDialogue;

    // ============================================================
    //  START — runs once when the game begins
    // ============================================================
    void Start()
    {
        // Find the main camera automatically
        playerCamera = Camera.main;

        // Get the SuspectDialogue script on this same object
        suspectDialogue = GetComponent<SuspectDialogue>();

        // Hide the prompt at the start
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    // ============================================================
    //  UPDATE — runs every frame
    // ============================================================
 void Update()
{
    if (playerCamera == null)
    {
        Debug.LogError("SuspectInteract: playerCamera is NULL on " + gameObject.name);
        return;
    }

    if (DialogueManager.instance != null && DialogueManager.instance.IsInConversation)
    {
        HidePrompt();
        return;
    }

    float distance = Vector3.Distance(
        playerCamera.transform.position,
        transform.position
    );

    // This will print the distance every second so we can see it
    Debug.Log(gameObject.name + " distance: " + distance);

    if (distance <= interactionDistance)
    {
        ShowPrompt();
        if (Input.GetKeyDown(interactKey))
        {
            if (DialogueManager.instance != null)
                DialogueManager.instance.StartConversation(suspectDialogue);
        }
    }
    else
    {
        HidePrompt();
    }
}

    // ============================================================
    //  ShowPrompt — displays the "Press E to talk" message
    // ============================================================
    void ShowPrompt()
    {
        if (!promptShowing)
        {
            if (promptUI != null)
                promptUI.SetActive(true);

            if (promptText != null)
                promptText.text = "Press E to talk to " + suspectDialogue.suspectName;

            promptShowing = true;
        }
    }

    // ============================================================
    //  HidePrompt — hides the prompt message
    // ============================================================
    void HidePrompt()
    {
        if (promptShowing)
        {
            if (promptUI != null)
                promptUI.SetActive(false);

            promptShowing = false;
        }
    }
}