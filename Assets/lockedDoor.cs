// ============================================================
//  LockedDoor.cs  —  The Missing Project
//  Attach this to a door object in your scene.
//  The door will only open if the player has collected
//  the Rusty Key (Clue ID 3 in our game).
//
//  HOW TO USE:
//  1. Click your door object in the Hierarchy
//  2. Add Component -> LockedDoor
//  3. Fill in the fields in the Inspector
// ============================================================

using UnityEngine;
using TMPro;

public class LockedDoor : MonoBehaviour
{
    [Header("Door Settings")]
    // Which clue ID acts as the key for this door?
    // In our game the Rusty Key is Clue ID 3
    public int requiredClueID = 3;

    // The name of the key item — shown in the prompt
    public string keyName = "Rusty Key";

    // How far the player must be to see the door prompt
    public float interactionDistance = 3f;

    // The key to press to open the door
    public KeyCode interactKey = KeyCode.E;

    [Header("Door Animation")]
    // Should the door swing open or slide?
    // true = swing (rotate), false = slide (move)
    public bool swingOpen = true;

    // How many degrees to rotate when swinging open
    public float swingAngle = 90f;

    // How fast the door opens
    public float openSpeed = 2f;

    [Header("UI")]
    public GameObject promptUI;
    public TextMeshProUGUI promptText;

    // ============================================================
    //  PRIVATE VARIABLES
    // ============================================================

    // Is the door currently open?
    private bool isOpen = false;

    // Is the door currently animating (moving)?
    private bool isAnimating = false;

    // The starting rotation of the door
    private Quaternion closedRotation;

    // The target rotation when open
    private Quaternion openRotation;

    // Player camera for distance checking
    private Camera playerCamera;

    // Is the prompt currently showing?
    private bool promptShowing = false;

    // ============================================================
    //  START
    // ============================================================
    void Start()
    {
        playerCamera = FindObjectOfType<Camera>();

        // Remember the door's starting rotation
        closedRotation = transform.rotation;

        // Calculate what the open rotation should be
        // We rotate around the Y axis by swingAngle degrees
        openRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y + swingAngle,
            transform.eulerAngles.z
        );

        // Hide prompt at start
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    // ============================================================
    //  UPDATE — checks distance and input every frame
    // ============================================================
    void Update()
    {
        // If door is already open, nothing to do
        if (isOpen && !isAnimating) return;

        // Animate the door opening if triggered
        if (isAnimating)
        {
            AnimateDoor();
            return;
        }

        // Measure distance from player to door
        float distance = Vector3.Distance(
            playerCamera.transform.position,
            transform.position
        );

        if (distance <= interactionDistance)
        {
            ShowPrompt();

            if (Input.GetKeyDown(interactKey))
            {
                TryOpenDoor();
            }
        }
        else
        {
            HidePrompt();
        }
    }

    // ============================================================
    //  TryOpenDoor — checks if player has the key
    // ============================================================
    void TryOpenDoor()
    {
        GameState gameState = FindObjectOfType<GameState>();

        if (gameState == null) return;

        // Check if player has the required clue (the key)
        if (gameState.HasClue(requiredClueID))
        {
            // Player has the key — open the door!
            Debug.Log("Door opened with " + keyName);
            isAnimating = true;

        if (InstructionPanel.instance != null)
{
    InstructionPanel.instance.SetInstruction(
        "You unlocked the secret room!\nSearch inside carefully."
    );
}
            HidePrompt();

        }
        else
        {
            // Player doesn't have the key — show locked message
            if (promptText != null)
            {
                promptText.text = "Locked. You need the " + keyName + ".";
            }
            Debug.Log("Door is locked. Need: " + keyName);
        }
    }

    // ============================================================
    //  AnimateDoor — smoothly rotates the door open
    // ============================================================
    void AnimateDoor()
    {
        // Smoothly rotate toward the open rotation
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            openRotation,
            openSpeed * Time.deltaTime
        );

        // Check if we are close enough to the target rotation
        if (Quaternion.Angle(transform.rotation, openRotation) < 0.5f)
        {
            // Snap to exact open position and stop animating
            transform.rotation = openRotation;
            isOpen = true;
            isAnimating = false;
        }
    }

    // ============================================================
    //  ShowPrompt and HidePrompt
    // ============================================================
    void ShowPrompt()
    {
        if (!promptShowing)
        {
            if (promptUI != null) promptUI.SetActive(true);

            if (promptText != null)
            {
                GameState gameState = FindObjectOfType<GameState>();
                if (gameState != null && gameState.HasClue(requiredClueID))
                    promptText.text = "Press E to unlock door";
                else
                    promptText.text = "Locked. Need the " + keyName + ".";
            }
            promptShowing = true;
        }
    }

    void HidePrompt()
    {
        if (promptShowing)
        {
            if (promptUI != null) promptUI.SetActive(false);
            promptShowing = false;
        }
    }
}