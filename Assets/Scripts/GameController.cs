using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI username;
    public TextMeshProUGUI startMessage;
    public TextMeshProUGUI warningMessage;
    public TextMeshProUGUI gameOverMessage;

    public Button quitButton;

    public Transform[] waypoints;
    public int currentPlayerIndex;
    public int currentWaypointIndex = 0;
    public bool isPlayermoving = false;
    public bool reachedEndPoint = false;
    public bool diceRolled = false;

    public int diceRoll;

    public GameObject diceObject;

    public Sprite[] diceSprites;
    public SpriteRenderer diceRenderer;

    [SerializeField] private AudioClip walking;
    [SerializeField] private AudioClip success;
    [SerializeField] private AudioClip diceAuido;
    private AudioSource audioSource;

    private Dictionary<int, int> specialWaypoints = new Dictionary<int, int>()
    {
        {18, 62}, {14, 46}, {37, 5}, {71, 33}, {68, 89}, {94, 64}, {78, 97}, {81, 60}
    };


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        currentPlayerIndex = currentWaypointIndex;

        username.text = UIManagement.instance.uname;
        startMessage.text = "Start the game!!";
        warningMessage.text = "Roll the dice!!";

        Button diceButton = diceObject.GetComponent<Button>();
        if(diceButton != null)
        {
            diceButton.onClick.AddListener(RollDice);
            //warningMessage.text = "Roll the dice!!";
        }
        else
        {
            Debug.LogError("Dice Button is not Assigned");
        }

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        if (diceRenderer == null)
        {
            Debug.LogError("Dice SpriteRenderer is not assigned!");
        }

    }

    void RollDice()
    {
        diceRoll = Random.Range(1, 7);
        Debug.Log("dice roll is: " + diceRoll);

        diceRenderer.sprite = diceSprites[diceRoll - 1];

        StartCoroutine(MovePlayer(diceRoll));
        audioSource.PlayOneShot(diceAuido);

        diceRolled = true;
    }

    IEnumerator MovePlayer(int steps)
    {
        isPlayermoving = true;

        audioSource.PlayOneShot(diceAuido);
        yield return new WaitForSecondsRealtime(0.5f);


        int remainingSteps = waypoints.Length - 1 - currentWaypointIndex;

        // Check if the dice roll is greater than the remaining steps
        if (steps > remainingSteps)
        {
            Debug.Log("Dice roll is greater than remaining steps. Roll again!");
            isPlayermoving = false;
            yield break; // Exit the coroutine, and the player will roll again
        }

        for (int i =0; i< steps; i++)
        {
            //yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(0.25f);

            currentWaypointIndex = Mathf.Clamp(currentWaypointIndex+1, 0, waypoints.Length -1);
            
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            //Vector2 targetPosition = waypoints[currentWaypointIndex].position;
            StartCoroutine(smoothMove(transform.position, targetPosition, 0.5f));

            if (isPlayermoving)
            {
                audioSource.PlayOneShot(walking);
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }
        isPlayermoving = false;
        checkSpecialTiles();

        Debug.Log("Player is at position: " + currentWaypointIndex);

        if (!reachedEndPoint && currentPlayerIndex == waypoints.Length - 1)
        {
            Debug.Log("Reached End!! Game Over");
            audioSource.PlayOneShot(success);

            gameOverMessage.text = "Game Over";

            isPlayermoving = false;
            diceObject.SetActive(false);
            yield break;
        }
    }

    void checkSpecialTiles()
    {
        if (specialWaypoints.ContainsKey(currentWaypointIndex))
        {
            currentWaypointIndex = specialWaypoints[currentWaypointIndex];
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            StartCoroutine(smoothMove(transform.position, targetPosition, 0.5f));
        }
    }

    IEnumerator smoothMove(Vector3 start, Vector3 end, float duration)
    {
        float elapsedTime = 0f;
        if(elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(start, end, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.position = end;
    }

    private void Update()
    {
        if (diceRolled)
        {
            warningMessage.text = "DiceRoll: " + diceRoll;
        }
        currentPlayerIndex = currentWaypointIndex;
    }

}
