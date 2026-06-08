using UnityEngine;
using TMPro;

public class InstructionPanel : MonoBehaviour
{
    public static InstructionPanel instance;

    [Header("UI Elements")]
    public GameObject instructionPanel;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI titleText;

    // All instructions in order of game progress
    private string[] instructions = new string[]
    {
        // 0 clues found
        "A research project has gone missing.\nExplore the lab and find clues.\nPress E to examine objects.",
        // 1 clue found
        "Good find! Keep searching.\nThere are 5 clues hidden around.\nPress N to check your notebook.",
        // 2 clues found
        "You are making progress.\nTry talking to the suspects.\nWalk up to them and press E.",
        // 3 clues found - rusty key
        "You found the Rusty Key!\nFind the locked door and use it.\nThe truth may be inside.",
        // 4 clues found
        "Almost there!\nSearch the secret room carefully.\nThen talk to suspects again.",
        // 5 clues found
        "You have all the clues!\nTalk to suspects one more time.\nThen make your accusation."
    };

    private int currentStage = 0;

    void Awake() { instance = this; }

    void Start()
    {
        if (titleText != null)
            titleText.text = "OBJECTIVE";

        // Show first instruction immediately
        SetInstruction(instructions[0]);
    }

    void Update()
    {
        // Automatically update based on clues found
        GameState gameState = FindObjectOfType<GameState>();
        if (gameState == null) return;

        int cluesFound = gameState.CluesFound();

        // Move forward through stages based on clues
        if (cluesFound >= 5 && currentStage < 5)
            GoToStage(5);
        else if (cluesFound >= 4 && currentStage < 4)
            GoToStage(4);
        else if (gameState.HasClue(3) && currentStage < 3)
            GoToStage(3);
        else if (cluesFound >= 2 && currentStage < 2)
            GoToStage(2);
        else if (cluesFound >= 1 && currentStage < 1)
            GoToStage(1);
    }

    void GoToStage(int stage)
    {
        currentStage = stage;
        SetInstruction(instructions[stage]);
    }

    public void SetInstruction(string text)
    {
        if (instructionText != null)
            instructionText.text = text;

        if (instructionPanel != null)
            instructionPanel.SetActive(true);
    }
}