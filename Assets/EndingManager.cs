// ============================================================
//  EndingManager.cs  —  The Missing Project
//  This script handles the final accusation screen.
//  The player picks who they think is guilty and gets
//  one of 3 different endings based on their choice
//  and how many clues they found.
//
//  HOW TO USE:
//  1. Create a new empty object in Hierarchy
//  2. Rename it "EndingManager"
//  3. Drag this script onto it
//  4. Build the accusation UI (see setup guide)
//  5. Connect everything in the Inspector
// ============================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingManager : MonoBehaviour
{
    // Global access from any script
    public static EndingManager instance;

    [Header("Accusation Screen UI")]
    // The big panel that covers the whole screen for the accusation
    public GameObject accusationPanel;

    // The title text at the top e.g. "Who stole the project?"
    public TextMeshProUGUI accusationTitleText;

    // The 3 suspect buttons the player clicks to accuse
    public Button accuseReidButton;
    public Button accuseMayaButton;
    public Button accuseHaleButton;

    // A warning shown if the player tries to accuse without enough clues
    public TextMeshProUGUI warningText;

    [Header("Ending Screen UI")]
    // The panel shown after accusation with the ending text
    public GameObject endingPanel;

    // The big ending title e.g. "CASE CLOSED" or "WRONG SUSPECT"
    public TextMeshProUGUI endingTitleText;

    // The detailed ending description
    public TextMeshProUGUI endingDescText;

    // A button to restart or quit after seeing the ending
    public Button playAgainButton;
    public Button quitButton;

    [Header("Accusation Trigger")]
    // How close the player must be to the accusation board
    public float triggerDistance = 4f;

    // The object in the scene the player walks up to (accusation board)
    public GameObject accusationBoard;

    // Prompt UI reused from clues
    public GameObject promptUI;
    public TextMeshProUGUI promptText;

    // ============================================================
    //  PRIVATE VARIABLES
    // ============================================================

    // Minimum clues needed to make an accusation
    // If player has fewer than this, they get the "cold case" ending
    private int minimumCluesRequired = 3;

    // Is the accusation screen currently open?
    private bool accusationOpen = false;

    // Player camera for distance checking
    private Camera playerCamera;

    // Is the prompt showing?
    private bool promptShowing = false;

    // ============================================================
    //  AWAKE
    // ============================================================
    void Awake()
    {
        instance = this;
    }

    // ============================================================
    //  START
    // ============================================================
    void Start()
    {
        playerCamera = Camera.main;

        // Hide both panels at start
        if (accusationPanel != null)
            accusationPanel.SetActive(false);

        if (endingPanel != null)
            endingPanel.SetActive(false);

        if (promptUI != null)
            promptUI.SetActive(false);

        if (warningText != null)
            warningText.gameObject.SetActive(false);

        // Connect buttons to their functions
        if (accuseReidButton != null)
            accuseReidButton.onClick.AddListener(() => MakeAccusation("DrReid", "Dr. Reid"));

        if (accuseMayaButton != null)
            accuseMayaButton.onClick.AddListener(() => MakeAccusation("Maya", "Maya Santos"));

        if (accuseHaleButton != null)
            accuseHaleButton.onClick.AddListener(() => MakeAccusation("ProfHale", "Professor Hale"));

        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    // ============================================================
    //  UPDATE — checks if player is near the accusation board
    // ============================================================
    void Update()
    {
        // Don't check if accusation screen is already open
        if (accusationOpen) return;

        // Don't check during dialogue or notebook
        if (DialogueManager.instance != null &&
            DialogueManager.instance.IsInConversation) return;

        if (accusationBoard == null) return;

        // Measure distance to accusation board
        float distance = Vector3.Distance(
            playerCamera.transform.position,
            accusationBoard.transform.position
        );

        if (distance <= triggerDistance)
        {
            ShowPrompt();

            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenAccusationScreen();
            }
        }
        else
        {
            HidePrompt();
        }
    }

    // ============================================================
    //  OpenAccusationScreen — shows the accusation UI
    // ============================================================
    void OpenAccusationScreen()
    {
        accusationOpen = true;
        HidePrompt();

        // Show the accusation panel
        accusationPanel.SetActive(true);

        // Hide instruction panel
        if (InstructionPanel.instance != null)
            InstructionPanel.instance.instructionPanel.SetActive(false);

        // Set the title
        if (accusationTitleText != null)
            accusationTitleText.text = "WHO STOLE THE MISSING PROJECT?";

        // Unlock cursor for clicking buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Show clue count as a hint
        GameState gameState = FindObjectOfType<GameState>();
        if (gameState != null && warningText != null)
        {
            int cluesFound = gameState.CluesFound();
            if (cluesFound < minimumCluesRequired)
            {
                warningText.gameObject.SetActive(true);
                warningText.text = "Warning: You have only found " + cluesFound +
                    " out of 5 clues.\nAre you sure you want to accuse someone?";
            }
        }
    }

    // ============================================================
    //  MakeAccusation — called when player clicks a suspect button
    //  suspectID = internal name e.g. "ProfHale"
    //  suspectDisplayName = shown name e.g. "Professor Hale"
    // ============================================================
    void MakeAccusation(string suspectID, string suspectDisplayName)
    {
        // Save accusation in GameState
        GameState gameState = FindObjectOfType<GameState>();
        if (gameState == null) return;

        gameState.Accuse(suspectID);

        int cluesFound = gameState.CluesFound();

        // Hide accusation panel
        accusationPanel.SetActive(false);

        // Decide which ending to show
        // ENDING C — not enough clues
        if (cluesFound < minimumCluesRequired)
        {
            ShowEnding(
                "CASE GOES COLD",
                "Without enough evidence, your accusation against " +
                suspectDisplayName + " is dismissed.\n\n" +
                "The stolen project was never recovered.\n" +
                "The real thief walks free.\n\n" +
                "You found " + cluesFound + " out of 5 clues.\n" +
                "Try again and search more carefully.",
                false
            );
        }
        // ENDING A — correct accusation with enough clues
        else if (suspectID == gameState.trueCulprit)
        {
            ShowEnding(
                "CASE CLOSED!",
                "You accused " + suspectDisplayName + " — and you were RIGHT!\n\n" +
                "Confronted with the evidence, " + suspectDisplayName +
                " confessed to stealing the research project.\n\n" +
                "The stolen data was recovered from the hidden hard drive.\n" +
                "The university reopened the investigation.\n\n" +
                "You found " + cluesFound + " out of 5 clues.\n" +
                "Excellent detective work!",
                true
            );
        }
        // ENDING B — wrong accusation
        else
        {
            ShowEnding(
                "WRONG SUSPECT",
                "You accused " + suspectDisplayName + " — but you were WRONG.\n\n" +
                "An innocent person's reputation was destroyed.\n" +
                "The real thief used the confusion to escape.\n\n" +
                "The stolen project was never found.\n\n" +
                "You found " + cluesFound + " out of 5 clues.\n" +
                "Look more carefully at the evidence next time.",
                false
            );
        }
    }

    // ============================================================
    //  ShowEnding — displays the ending screen
    //  isGoodEnding changes the title color
    // ============================================================
    void ShowEnding(string title, string description, bool isGoodEnding)
    {
        // Show the ending panel
        endingPanel.SetActive(true);

        // Set the title
        if (endingTitleText != null)
        {
            endingTitleText.text = title;
            // Green for good ending, red for bad ending
            endingTitleText.color = isGoodEnding ?
                new Color(0.1f, 0.8f, 0.1f) :  // green
                new Color(0.9f, 0.1f, 0.1f);    // red
        }

        // Set the description
        if (endingDescText != null)
            endingDescText.text = description;
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
                promptText.text = "Press E to make your accusation";
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

    // ============================================================
    //  PlayAgain — reloads the scene to restart
    // ============================================================
    void PlayAgain()
    {
        // Reload the current scene — resets everything
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    // ============================================================
    //  QuitGame — closes the game
    // ============================================================
    void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}