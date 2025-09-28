using UnityEngine;
using System.Collections;

public class DiceRoller : MonoBehaviour
{
    public GameObject[] diceFaces;           // 6 dice face objects
    public DialogueManagerTMP dialogueManager; // Reference to DialogueManagerTMP
    public float rollDuration = 1f;          // Dice roll animation duration
    public float postRollDelay = 2f;         // Wait before showing guidance

    private bool diceRolled = false;

    void Start()
    {
        // Hide all dice faces at start
        foreach (var face in diceFaces)
            if (face != null) face.SetActive(false);
    }

    void Update()
    {
        // Press Enter to roll dice
        if (!diceRolled && Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(RollDiceRoutine());
        }
    }

    IEnumerator RollDiceRoutine()
    {
        diceRolled = true;

        // Visual roll animation
        float timer = 0f;
        int tempRoll = 0;
        while (timer < rollDuration)
        {
            tempRoll = Random.Range(1, diceFaces.Length + 1);
            for (int i = 0; i < diceFaces.Length; i++)
                if (diceFaces[i] != null)
                    diceFaces[i].SetActive(i == tempRoll - 1);

            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        // Final rolled number
        int rolledNumber = Random.Range(1, diceFaces.Length + 1);
        for (int i = 0; i < diceFaces.Length; i++)
            if (diceFaces[i] != null)
                diceFaces[i].SetActive(i == rolledNumber - 1);

        // Wait briefly before calling DialogueManager
        yield return new WaitForSeconds(postRollDelay);

        // Notify DialogueManager of result
        if (dialogueManager != null)
            dialogueManager.OnDiceRolled(rolledNumber);
    }
}
