using UnityEngine;
using TMPro;

public class NotebookManager : MonoBehaviour
{
    public static NotebookManager instance;

    [Header("Notebook UI")]
    public GameObject notebookPanel;
    public TextMeshProUGUI cluesText;
    public KeyCode toggleKey = KeyCode.N;

    private bool isOpen = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (notebookPanel != null)
            notebookPanel.SetActive(false);
    }

    void Update()
    {
        // Don't open notebook during dialogue
        if (DialogueManager.instance != null &&
            DialogueManager.instance.IsInConversation) return;

        if (Input.GetKeyDown(toggleKey))
        {
            if (isOpen) CloseNotebook();
            else OpenNotebook();
        }
    }

    public void OpenNotebook()
    {
        isOpen = true;
        notebookPanel.SetActive(true);

        // Hide instruction panel while notebook is open
        if (InstructionPanel.instance != null)
            InstructionPanel.instance.instructionPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        RefreshClues();
    }

    public void CloseNotebook()
    {
        isOpen = false;
        notebookPanel.SetActive(false);

        // Show instruction panel again
        if (InstructionPanel.instance != null)
            InstructionPanel.instance.instructionPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void RefreshClues()
    {
        GameState gameState = FindObjectOfType<GameState>();
        if (gameState == null) return;

        string content = "=== CLUES FOUND ===\n\n";

        if (gameState.HasClue(1))
            content += "[1] USB Drive\nA scratched USB found on the desk.\n\n";
        if (gameState.HasClue(2))
            content += "[2] Deleted Email\nA printed email meant to be deleted.\n\n";
        if (gameState.HasClue(3))
            content += "[3] Rusty Key\nAn old key that opens a hidden room.\n\n";
        if (gameState.HasClue(4))
            content += "[4] Research Disk\nA disk containing stolen research data.\n\n";
        if (gameState.HasClue(5))
            content += "[5] Hidden Hard Drive\nFound behind a server rack.\n\n";

        if (gameState.CluesFound() == 0)
            content += "No clues found yet.\nExplore the lab!";

        cluesText.text = content;
    }
}