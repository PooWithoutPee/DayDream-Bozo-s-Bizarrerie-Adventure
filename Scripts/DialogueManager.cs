using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DialogueManagerTMP : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;   // Main dialogue text
    public Button nextButton;               // Next button
    public GameObject dice;                 // Dice parent object
    public DiceRoller diceRoller;           // Reference to DiceRoller script

    [Header("Choice Settings")]
    public GameObject choice1Button;        // First choice button
    public GameObject choice2Button;        // Second choice button

    [Header("Dialogue Settings")]
    [TextArea(2, 5)]
    public List<string> dialogueLines = new List<string>()
    {
        "Ah¡K welcome, mortal. I have a proposition for you.",
        "You are now bound to a game of blood and fire, here in my realm of Hell.",
        "To survive, you must hunt demons and claim their power.",
        "Roll the dice to gain strength¡K but every roll demands a price.",
        "To grow stronger, you must sacrifice someone you love.",
        "Choose wisely¡K for each choice seals a fate you cannot undo. Press Enter to roll!"
    };

    [Header("Typewriter Settings")]
    public float lettersPerSecond = 30f;

    private int currentLine = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool waitingForChoice = false;
    private bool afterChoiceDialogue = false; // first click after choice shows fight text
    private bool fightTextShown = false;      // second click after choice loads scene

    void Start()
    {
        if (dice != null) dice.SetActive(false);
        if (choice1Button != null) choice1Button.SetActive(false);
        if (choice2Button != null) choice2Button.SetActive(false);

        if (dialogueLines.Count > 0)
            ShowLine(currentLine);

        nextButton.onClick.AddListener(OnNextClicked);

        // Add listeners to choice buttons
        if (choice1Button != null) choice1Button.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(1));
        if (choice2Button != null) choice2Button.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(2));
    }

    void ShowLine(int index)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(dialogueLines[index]));
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

        // Show dice after last dialogue line
        if (currentLine == dialogueLines.Count - 1 && dice != null)
            dice.SetActive(true);
    }

    void OnNextClicked()
    {
        if (waitingForChoice) return; // freeze until choice made

        if (isTyping)
        {
            // Finish line instantly
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            dialogueText.text = dialogueLines[currentLine];
            isTyping = false;

            if (currentLine == dialogueLines.Count - 1 && dice != null)
                dice.SetActive(true);
        }
        else
        {
            if (afterChoiceDialogue)
            {
                // First click after choice ¡÷ show fight text with typewriter
                StartCoroutine(TypeText("You will now be sent to fight the demon!"));
                afterChoiceDialogue = false;
                fightTextShown = true;
                return;
            }

            if (fightTextShown)
            {
                // Second click after fight text ¡÷ load next scene
                SceneManager.LoadScene("GamePlay"); // Replace with your Scene2 name
                return;
            }

            currentLine++;
            if (currentLine < dialogueLines.Count)
            {
                ShowLine(currentLine);
            }
            else
            {
                // Keep last line, show dice
                dialogueText.text = dialogueLines[dialogueLines.Count - 1];
                if (dice != null) dice.SetActive(true);

                // Disable Next button until dice roll
                if (nextButton != null) nextButton.interactable = false;
            }
        }
    }

    // Called by DiceRoller after dice roll
    public void OnDiceRolled(int rolledNumber)
    {
        StartCoroutine(TypeText("You rolled a " + rolledNumber + "!"));
        StartCoroutine(ShowSacrificeChoices(rolledNumber));
    }

    private IEnumerator ShowSacrificeChoices(int rolledNumber)
    {
        yield return new WaitForSeconds(2f);

        // Show guidance message with typewriter
        yield return StartCoroutine(TypeText("This means you can sacrifice your family to get a buff in the following battle."));

        yield return new WaitForSeconds(2f);

        // Freeze dialogue and show choices
        waitingForChoice = true;
        if (choice1Button != null) choice1Button.SetActive(true);
        if (choice2Button != null) choice2Button.SetActive(true);
    }

    private void OnChoiceSelected(int choiceNumber)
    {
        waitingForChoice = false;

        string resultText = "";
        if (choiceNumber == 1)
        {
            resultText = "Congratulations¡K your sister is dead, but you get more HP!";
        }
        else if (choiceNumber == 2)
        {
            resultText = "Congratulations¡K your monther is dead, but you deal higher damage!";
        }

        // Hide choice buttons
        if (choice1Button != null) choice1Button.SetActive(false);
        if (choice2Button != null) choice2Button.SetActive(false);

        // Enable Next button to continue
        if (nextButton != null)
            nextButton.interactable = true;

        // Set flag to show next dialogue on next click
        afterChoiceDialogue = true;

        // Show choice result with typewriter
        StartCoroutine(TypeText(resultText));
    }
}
