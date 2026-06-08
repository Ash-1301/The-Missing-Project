using UnityEngine;
using TMPro;

public class SimpleDoor : MonoBehaviour
{
    public float swingAngle = 90f;
    public float openSpeed = 2f;
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Camera playerCamera;
    private bool promptShowing = false;

    void Start()
    {
        playerCamera = Camera.main;
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y + swingAngle,
            transform.eulerAngles.z
        );
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        if (isAnimating)
        {
            AnimateDoor();
            return;
        }

        if (isOpen) return;

        float distance = Vector3.Distance(
            playerCamera.transform.position,
            transform.position
        );

        if (distance <= interactionDistance)
        {
            ShowPrompt();
            if (Input.GetKeyDown(interactKey))
            {
                isAnimating = true;
                HidePrompt();
            }
        }
        else
        {
            HidePrompt();
        }
    }

    void AnimateDoor()
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            openRotation,
            openSpeed * Time.deltaTime
        );

        if (Quaternion.Angle(transform.rotation, openRotation) < 0.5f)
        {
            transform.rotation = openRotation;
            isOpen = true;
            isAnimating = false;

            // Make collider trigger so player can walk through
            Collider col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }
    }

    void ShowPrompt()
    {
        if (!promptShowing)
        {
            if (promptUI != null) promptUI.SetActive(true);
            if (promptText != null) promptText.text = "Press E to open door";
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