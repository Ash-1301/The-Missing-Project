// ============================================================
//  OpeningCutscene.cs  —  The Missing Project
//  This plays an opening cutscene before the game starts.
//  It shows a series of text slides on a black screen
//  that fade in and out to tell the backstory.
//
//  HOW TO USE:
//  1. Create a NEW SCENE called "OpeningCutscene"
//  2. Set up the UI in that scene
//  3. Attach this script to an empty object
//  4. After the cutscene ends it loads your main game scene
// ============================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class OpeningCutscene : MonoBehaviour
{
    [Header("UI Elements")]
    // The black background panel — covers the whole screen
    public Image blackBackground;

    // The main text shown in the center of the screen
    public TextMeshProUGUI storyText;

    // A smaller subtitle text at the bottom (optional)
    public TextMeshProUGUI subtitleText;

    // "Press any key to skip" text
    public TextMeshProUGUI skipText;

    [Header("Settings")]
    // Name of the scene to load after cutscene ends
    // Make sure this matches your main game scene name EXACTLY
    public string gameSceneName = "SampleScene";

    // How long each slide stays on screen (fully visible)
    public float slideDisplayTime = 3f;

    // How fast text fades in and out (seconds)
    public float fadeDuration = 1.5f;

    // ============================================================
    //  The story slides — each string is one screen of text
    //  Edit these to tell YOUR backstory
    // ============================================================
    private string[] slides = new string[]
    {
        // Slide 1 — set the scene
        "Mayfield University has been developing a research project that could bring millions in funding and recognition to the department.",

        // Slide 2 — introduce the problem
        "The project was stored on the university's secure lab computers.",

        // Slide 3 — the incident
        "Then one night...\n\nThe project vanished.\n\nEvery file. Every backup.\nGone.",

        // Slide 4 — introduce suspects
        "Three people had access to the lab\nthat night.\n\nThree people with secrets.\nThree people with motives.",

        // Slide 5 — introduce the player
        "You have been called in\nto investigate.\n\nFind the clues.\nQuestion the suspects.\nUncover the truth.",

        // Slide 6 — final slide before game starts
        "The Missing Project\n\nWho took it?\nOnly you can find out."
    };

    // ============================================================
    //  START — begins the cutscene automatically
    // ============================================================
    void Start()
    {
        // Hide the cursor during cutscene
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Show skip text
        if (skipText != null)
            skipText.text = "Press any key to skip";

        // Start the cutscene coroutine
        // A coroutine lets us wait between slides
        StartCoroutine(PlayCutscene());
    }

    // ============================================================
    //  UPDATE — checks for skip input every frame
    // ============================================================
    void Update()
    {
        // Any key press skips the cutscene
        if (Input.anyKeyDown)
        {
            // Stop all running coroutines
            StopAllCoroutines();

            // Go straight to the game
            LoadGameScene();
        }
    }

    // ============================================================
    //  PlayCutscene — the main coroutine that runs all slides
    //  A coroutine is a function that can pause and wait
    // ============================================================
    IEnumerator PlayCutscene()
    {
        // Start with everything invisible
        SetTextAlpha(0f);

        // Small pause before first slide
        yield return new WaitForSeconds(0.5f);

        // Loop through each slide
        for (int i = 0; i < slides.Length; i++)
        {
            // Set the text for this slide
            if (storyText != null)
                storyText.text = slides[i];

            // FADE IN — gradually make text visible
            yield return StartCoroutine(FadeText(0f, 1f));

            // HOLD — keep text visible for a few seconds
            yield return new WaitForSeconds(slideDisplayTime);

            // FADE OUT — gradually hide the text
            yield return StartCoroutine(FadeText(1f, 0f));

            // Small pause between slides
            yield return new WaitForSeconds(0.3f);
        }

        // All slides done — load the game
        LoadGameScene();
    }

    // ============================================================
    //  FadeText — smoothly changes text transparency
    //  fromAlpha = starting transparency (0 = invisible, 1 = visible)
    //  toAlpha = ending transparency
    // ============================================================
    IEnumerator FadeText(float fromAlpha, float toAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            // Calculate how far through the fade we are (0 to 1)
            float progress = elapsed / fadeDuration;

            // Lerp smoothly between the two alpha values
            float currentAlpha = Mathf.Lerp(fromAlpha, toAlpha, progress);

            // Apply the alpha to the text
            SetTextAlpha(currentAlpha);

            // Wait one frame then continue
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to exact final value
        SetTextAlpha(toAlpha);
    }

    // ============================================================
    //  SetTextAlpha — sets the transparency of all text elements
    // ============================================================
    void SetTextAlpha(float alpha)
    {
        if (storyText != null)
        {
            Color c = storyText.color;
            c.a = alpha;
            storyText.color = c;
        }

        if (subtitleText != null)
        {
            Color c = subtitleText.color;
            c.a = alpha;
            subtitleText.color = c;
        }
    }

    // ============================================================
    //  LoadGameScene — loads the main game
    // ============================================================
    void LoadGameScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(gameSceneName);
    }
}