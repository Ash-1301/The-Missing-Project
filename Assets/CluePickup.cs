// ============================================================
//  CluePickup.cs  —  The Missing Project
//  Attach this script to any object in your scene that you
//  want the player to pick up as a clue (USB drive, letter, etc.)
// ============================================================

// These lines import Unity's built-in tools so our script can use them.
// Think of them like "turning on" features we need.
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // This is for TextMeshPro — the text system we use for UI

// This line creates the script as a "component" that can be attached to game objects.
// The name here MUST match the filename exactly: CluePickup
public class CluePickup : MonoBehaviour
{
    // ============================================================
    //  SETTINGS — these appear as fields in the Unity Inspector
    //  so you can fill them in without touching code
    // ============================================================

    [Header("Clue Information")]
    // The name of this clue — e.g. "USB Drive" or "Deleted Email"
    // [Header] just adds a label in the Inspector to keep things tidy
    public string clueName = "Unknown Clue";

    // A longer description shown when the player examines the clue
    public string clueDescription = "A mysterious item...";

    // A number to identify this clue (1 through 5)
    // GameState uses this number to track which clues have been found
    public int clueID = 1;

    [Header("Interaction Settings")]
    // How close the player must be to see the "Press E" prompt
    // 3 means 3 Unity units away — adjust this if needed
    public float interactionDistance = 3f;

    // Which key the player presses to pick up the clue
    // KeyCode.E means the "E" key on the keyboard
    public KeyCode interactKey = KeyCode.E;

    [Header("UI Elements — drag these in from the Inspector")]
    // This is the panel (box) that appears on screen saying "Press E to examine"
    // You need to create this in your Canvas and drag it here
    public GameObject promptUI;

    // The text inside that prompt panel
    // Drag your TextMeshPro text object here
    public TextMeshProUGUI promptText;

    // The panel that shows the clue details when picked up
    // A bigger popup showing the clue name and description
    public GameObject cluePopupUI;

    // The text fields inside the popup
    public TextMeshProUGUI clueNameText;
    public TextMeshProUGUI clueDescText;

    [Header("Optional")]
    // Should the clue object disappear after being picked up?
    // true = yes it vanishes, false = it stays in the world
    public bool disappearOnPickup = true;

    // A sound to play when the clue is picked up
    // Drag an AudioClip from your Project panel here (optional)
    public AudioClip pickupSound;

    // ============================================================
    //  PRIVATE VARIABLES — used internally, not shown in Inspector
    // ============================================================

    // Stores a reference to the player's camera so we can measure distance
    private Camera playerCamera;

    // Tracks whether this clue has already been collected
    // Prevents the player from picking it up twice
    private bool alreadyCollected = false;

    // Tracks whether the prompt is currently showing on screen
    private bool promptShowing = false;

    // ============================================================
    //  START — runs once when the game begins
    // ============================================================
    void Start()
    {
        // Find the Main Camera in the scene automatically
        // This is the camera tagged "MainCamera" in Unity
        playerCamera = Camera.main;

        // Hide the prompt UI at the start — we only show it when close enough
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }

        // Also hide the clue popup at the start
        if (cluePopupUI != null)
        {
            cluePopupUI.SetActive(false);
        }
    }

    // ============================================================
    //  UPDATE — runs every single frame (60+ times per second)
    //  This is where we check distance and key presses
    // ============================================================
    void Update()
    {
        // If this clue has already been collected, do nothing and stop here
        // The "return" keyword exits the Update function early
        if (alreadyCollected) return;

        // Calculate the distance between the player camera and this clue object
        // Vector3.Distance measures the straight-line gap between two positions
        float distanceToPlayer = Vector3.Distance(
            playerCamera.transform.position,  // where the player's camera is
            transform.position                // where this clue object is
        );

        // Check if the player is close enough to interact
        if (distanceToPlayer <= interactionDistance)
        {
            // Player is close enough — show the "Press E to examine" prompt
            ShowPrompt();

            // Now check if the player actually pressed the E key this frame
            // GetKeyDown only triggers ONCE when the key is first pressed (not held)
            if (Input.GetKeyDown(interactKey))
            {
                // The player pressed E while close enough — collect this clue!
                CollectClue();
            }
        }
        else
        {
            // Player walked away — hide the prompt
            HidePrompt();
        }
    }

    // ============================================================
    //  ShowPrompt — displays the "Press E to examine" message
    // ============================================================
    void ShowPrompt()
    {
        // Only do this if we haven't already shown the prompt
        // This avoids calling SetActive every single frame (wasteful)
        if (!promptShowing)
        {
            // Make the prompt UI visible on screen
            if (promptUI != null)
            {
                promptUI.SetActive(true);
            }

            // Update the prompt text to show the clue name
            if (promptText != null)
            {
                promptText.text = "Press E to examine: " + clueName;
            }

            // Remember that we're currently showing the prompt
            promptShowing = true;
        }
    }

    // ============================================================
    //  HidePrompt — hides the "Press E" message
    // ============================================================
    void HidePrompt()
    {
        // Only hide it if it's currently showing
        if (promptShowing)
        {
            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }

            // Remember that the prompt is now hidden
            promptShowing = false;
        }
    }

    // ============================================================
    //  CollectClue — called when the player picks up the clue
    // ============================================================
    void CollectClue()
    {
        // Mark this clue as collected so we can't pick it up again
        alreadyCollected = true;

        // Hide the "Press E" prompt since we don't need it anymore
        HidePrompt();

        // Tell the GameState that this clue has been found
        // GameState is a separate script that tracks all game progress
        // We use FindObjectOfType to locate it anywhere in the scene
        GameState gameState = FindObjectOfType<GameState>();
        if (gameState != null)
        {
            // Add this clue to the player's notebook
            gameState.CollectClue(clueID, clueName, clueDescription);
        }
        else
        {
            // If GameState isn't set up yet, just print a message so you know
            // Debug.Log prints to the Console panel at the bottom of Unity
            Debug.Log("CluePickup: No GameState found in scene. Create one!");
        }

        // Play the pickup sound if one has been assigned
        if (pickupSound != null)
        {
            // PlayClipAtPoint plays a sound at a position in 3D space
            // We play it where the clue was sitting
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Show the clue popup UI so the player can read what they found
        ShowCluePopup();

        // If disappearOnPickup is ticked, hide this object from the scene
        if (disappearOnPickup)
        {
            // SetActive(false) hides the object — it still exists but is invisible
            // Use Destroy(gameObject) instead if you want to fully delete it
            gameObject.SetActive(false);
        }
    }

    // ============================================================
    //  ShowCluePopup — shows the big popup with clue details
    // ============================================================
    void ShowCluePopup()
    {
        if (cluePopupUI != null)
        {
            // Make the popup visible
            cluePopupUI.SetActive(true);

            // Fill in the clue name text
            if (clueNameText != null)
            {
                clueNameText.text = clueName;
            }

            // Fill in the clue description text
            if (clueDescText != null)
            {
                clueDescText.text = clueDescription;
            }

            // Automatically hide the popup after 3 seconds
            // Invoke calls a function by name after a delay (in seconds)
            Invoke("HideCluePopup", 3f);
        }
    }

    // ============================================================
    //  HideCluePopup — hides the popup (called after 3 seconds)
    // ============================================================
    void HideCluePopup()
    {
        if (cluePopupUI != null)
        {
            cluePopupUI.SetActive(false);
        }
    }

    // ============================================================
    //  OnDrawGizmosSelected — draws a helpful circle in the editor
    //  This is ONLY visible in the Scene view, not in the real game
    //  It shows you the interaction radius around the clue
    // ============================================================
    void OnDrawGizmosSelected()
    {
        // Set the colour of the debug circle to yellow
        Gizmos.color = Color.yellow;

        // Draw a wireframe sphere showing the interaction range
        // This helps you visualise how close the player needs to be
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}