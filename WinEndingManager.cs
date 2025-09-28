using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class WinEndingManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public Button nextButton;
    public GameObject respectImage; // Image to show after pressing F

    [Header("Typewriter Settings")]
    public float lettersPerSecond = 30f;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private int stage = 0;
    private bool canPressF = false;

    void Start()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextClicked);

        if (respectImage != null)
            respectImage.SetActive(false);

        ShowNextStage();
    }

    void ShowNextStage()
    {
        stage++;
        switch (stage)
        {
            case 1:
                StartCoroutine(TypeText("Congratulations! You have defeated all challenges and emerged victorious."));
                break;
            case 2:
                StartCoroutine(TypeText("You are now free¡K and you marry the devil girl, hand in hand, embracing your fate."));
                break;
            case 3:
                StartCoroutine(TypeText("But¡K was all the sacrifice truly worth it?"));
                break;
            case 4:
                // Show text and activate image immediately
                StartCoroutine(TypeText("Press F to pay your respects..."));
                if (respectImage != null)
                    respectImage.SetActive(true); // show image as soon as text appears
                canPressF = true; // still wait for player to press F
                if (nextButton != null)
                    nextButton.gameObject.SetActive(false); // hide Next button during F input
                break;

            case 5:
                dialogueText.text = "The End.";
                break;
        }
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }

    void OnNextClicked()
    {
        if (isTyping) return; // wait for typewriter to finish
        ShowNextStage();
    }

    void Update()
    {
        if (canPressF && Input.GetKeyDown(KeyCode.F))
        {
            canPressF = false;
            if (respectImage != null)
                respectImage.SetActive(true);

            // Optional: show ending text again
            dialogueText.text = "You have paid your respects.";
        }
    }
}
