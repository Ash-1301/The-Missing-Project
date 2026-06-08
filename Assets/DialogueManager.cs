using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("Main Dialogue Panel")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI suspectNameText;
    public TextMeshProUGUI dialogueText;
    public Image suspectPortrait;

    [Header("Question Buttons")]
    public Button questionButton1;
    public Button questionButton2;
    public Button questionButton3;
    public Button questionButton4;
    public TextMeshProUGUI questionText1;
    public TextMeshProUGUI questionText2;
    public TextMeshProUGUI questionText3;
    public TextMeshProUGUI questionText4;

    [Header("Close Button")]
    public Button closeButton;

    private SuspectDialogue currentSuspect;
    private bool isInConversation = false;
    public bool IsInConversation { get { return isInConversation; } }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseDialogue);
    }

    public void StartConversation(SuspectDialogue suspect)
    {
        currentSuspect = suspect;
        isInConversation = true;

        // Show dialogue panel
        dialoguePanel.SetActive(true);

        // Hide instruction panel so it doesn't overlap
        if (InstructionPanel.instance != null)
            InstructionPanel.instance.instructionPanel.SetActive(false);

        // Show suspect name
        if (suspectNameText != null)
            suspectNameText.text = suspect.suspectName;

        // Show portrait if assigned
        if (suspectPortrait != null && suspect.portrait != null)
        {
            suspectPortrait.sprite = suspect.portrait;
            suspectPortrait.gameObject.SetActive(true);
        }

        // Show greeting
        ShowText(suspect.greetingText);

        // Unlock cursor for clicking buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Load question buttons
        LoadQuestions();
    }

    void LoadQuestions()
    {
        GameState gameState = FindObjectOfType<GameState>();

        List<SuspectDialogue.DialogueQuestion> available
            = currentSuspect.GetAvailableQuestions(gameState);

        HideAllButtons();

        Button[] buttons = { questionButton1, questionButton2, questionButton3, questionButton4 };
        TextMeshProUGUI[] texts = { questionText1, questionText2, questionText3, questionText4 };

        for (int i = 0; i < available.Count && i < 4; i++)
        {
            buttons[i].gameObject.SetActive(true);
            texts[i].text = available[i].questionText;

            int index = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => AskQuestion(available[index]));
        }
    }

    void AskQuestion(SuspectDialogue.DialogueQuestion question)
    {
        ShowText(question.answerText);

        GameState gameState = FindObjectOfType<GameState>();
        if (gameState != null && question.flagToSet != "")
            gameState.SetFlag(question.flagToSet);

        question.hasBeenAsked = true;
        LoadQuestions();
    }

    void ShowText(string text)
    {
        if (dialogueText != null)
            dialogueText.text = text;
    }

    void HideAllButtons()
    {
        if (questionButton1 != null) questionButton1.gameObject.SetActive(false);
        if (questionButton2 != null) questionButton2.gameObject.SetActive(false);
        if (questionButton3 != null) questionButton3.gameObject.SetActive(false);
        if (questionButton4 != null) questionButton4.gameObject.SetActive(false);
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        isInConversation = false;
        currentSuspect = null;

        // Show instruction panel again
        if (InstructionPanel.instance != null)
            InstructionPanel.instance.instructionPanel.SetActive(true);

        // Lock cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}