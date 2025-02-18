using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    public Transform[] tiles;
    public GameObject spawnPoint;
    public DiceRollScript diceScript;
    int characterIndex;
    int index;

    private GameObject mainCharacter;
    private int currentTileIndex = -1;
    private bool isMoving = false;
    int[] otherPlayers;

    private const string textFileName = "playerNames";

    void Start()
    {
        characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        mainCharacter = Instantiate(playerPrefabs[characterIndex],
            spawnPoint.transform.position, Quaternion.identity);
        mainCharacter.GetComponent<NameScript>().SetPlayerName(
            PlayerPrefs.GetString("PlayerName"));

        otherPlayers = new int[PlayerPrefs.GetInt("PlayerCount")];
        string[] nameArray = ReadLinesFromFile(textFileName);
        Debug.Log(otherPlayers.Length + " " + nameArray.Length);

        for (int i = 0; i < otherPlayers.Length; i++)
        {
            spawnPoint.transform.position += new Vector3(0.2f, 0, 0.08f);
            index = Random.Range(0, playerPrefabs.Length);
            GameObject character = Instantiate(playerPrefabs[index],
                spawnPoint.transform.position, Quaternion.identity);
            character.GetComponent<NameScript>().SetPlayerName(
                nameArray[Random.Range(0, nameArray.Length)]);
        }
    }

    void Update()
    {
        // Only trigger movement if the dice has landed, its result hasn't been used yet,
        // and the player is not already moving.
        if (diceScript.isLanded && !diceScript.hasBeenUsed && !isMoving)
        {
            int diceResult;
            if (int.TryParse(diceScript.diceFaceNum, out diceResult))
            {
                // Mark the dice result as used so it isn’t consumed again.
                diceScript.hasBeenUsed = true;
                StartCoroutine(MovePlayer(diceResult));
            }
        }
    }

    IEnumerator MovePlayer(int steps)
    {
        isMoving = true;
        float elevation = 0.7f; // Augstuma nobīde, lai karakteri būtu redzami

        // Aprēķina, cik soļu ir līdz finiša (pēdējam tile)
        int remaining = (tiles.Length - 1) - currentTileIndex;
        int totalSteps = steps;

        if (totalSteps <= remaining)
        {
            // Ja soļu skaits ir mazāks vai vienāds ar atlikušajiem, pārvietojas vienkārši uz priekšu.
            for (int i = 0; i < totalSteps; i++)
            {
                int nextTileIndex = (currentTileIndex == -1) ? 0 : currentTileIndex + 1;
                currentTileIndex = nextTileIndex;

                Vector3 startPos = mainCharacter.transform.position;
                Vector3 endPos = tiles[currentTileIndex].position + new Vector3(0, elevation, 0);
                float duration = 0.5f;
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    mainCharacter.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                mainCharacter.transform.position = endPos;
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            // Ja izkritušais skaitlis pārsniedz atlikušos soļus,
            // vispirms iet uz finišu, tad atlikušos soļus atpakaļ.
            int forwardSteps = remaining;
            int backwardSteps = totalSteps - remaining;

            // Pārvietojas uz finiša tile
            for (int i = 0; i < forwardSteps; i++)
            {
                int nextTileIndex = (currentTileIndex == -1) ? 0 : currentTileIndex + 1;
                currentTileIndex = nextTileIndex;

                Vector3 startPos = mainCharacter.transform.position;
                Vector3 endPos = tiles[currentTileIndex].position + new Vector3(0, elevation, 0);
                float duration = 0.5f;
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    mainCharacter.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                mainCharacter.transform.position = endPos;
                yield return new WaitForSeconds(0.1f);
            }

            // Pārvietojas atpakaļ par atlikušajiem soļiem.
            for (int i = 0; i < backwardSteps; i++)
            {
                int nextTileIndex = currentTileIndex - 1;
                // Ja noiet pāri sākuma tile, saglabā pozīciju 0.
                if (nextTileIndex < 0)
                    nextTileIndex = 0;

                currentTileIndex = nextTileIndex;

                Vector3 startPos = mainCharacter.transform.position;
                Vector3 endPos = tiles[currentTileIndex].position + new Vector3(0, elevation, 0);
                float duration = 0.5f;
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    mainCharacter.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                mainCharacter.transform.position = endPos;
                yield return new WaitForSeconds(0.1f);
            }
        }

        isMoving = false;
    }

    string[] ReadLinesFromFile(string fName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fName);
        if (textAsset != null)
            return textAsset.text.Split(new[] { '\r', '\n' },
                System.StringSplitOptions.RemoveEmptyEntries);
        else
        {
            Debug.LogError("File not found " + fName);
            return new string[0];
        }
    }
}