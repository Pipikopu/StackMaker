using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Swipe variables
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    // Player Transform and Player Model Transform
    public Transform playerTransform;
    public Transform modelTransform;

    // Moving variables
    public float speed;
    public enum Direction
    {
        Idle,
        Forward,
        Backward,
        Left,
        Right
    }
    private Direction direction;
    private Direction nextDirection;
    private Vector3 targetPosition;

    // Stack variables
    private int numOfStacks;
    public GameObject stackHolder;
    public float stackDistance;
    public GameObject yellowFlatPrefabs;

    // Score and winning variable
    private int score;
    private bool isWinning;
    public GameObject winParticlesObject;
    public GameObject closedArk;
    public GameObject openArk;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        score = 0;
        numOfStacks = 0;
        isWinning = false;
        direction = Direction.Idle;
    }

    void Update()
    {
        if (direction == Direction.Idle && !isWinning)
        {
            Swipe();
        }
        else
        {
            Move();
            if (Vector3.Distance(playerTransform.position, targetPosition) < 0.1f)
            {
                CheckNextDirection();
            }
        }
    }

    private void Move()
    {
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetPosition, speed * Time.deltaTime);
    }

    private void CheckNextDirection()
    {
        playerTransform.position = targetPosition;
        direction = nextDirection;
        nextDirection = Direction.Idle;

        switch (direction)
        {
            case Direction.Forward:
                GetTargetPosition(Vector3.forward);
                break;
            case Direction.Backward:
                GetTargetPosition(Vector3.back);
                break;
            case Direction.Right:
                GetTargetPosition(Vector3.right);
                break;
            case Direction.Left:
                GetTargetPosition(Vector3.left);
                break;
        }
    }

    private bool GetTargetPosition(Vector3 directionVec)
    {
        int layer_mask = LayerMask.GetMask("Wall");
        if (Physics.Raycast(playerTransform.position - Vector3.up * 0.5f, directionVec, out RaycastHit hit, Mathf.Infinity, layer_mask))
        {
            targetPosition = playerTransform.position + directionVec * (hit.distance - 0.5f);
            return true;
        }
        return false;
    }

    void Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            currentSwipe.Normalize();

            // Swipe up
            if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            {
                if (GetTargetPosition(Vector3.forward))
                {
                    direction = Direction.Forward;
                }
            }

            //swipe down
            if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            {
                if (GetTargetPosition(Vector3.back))
                {
                    direction = Direction.Backward;
                }
                //GetTargetPosition(Vector3.back);
                //direction = Direction.Backward;
            }
            //swipe left
            if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                if (GetTargetPosition(Vector3.left))
                {
                    direction = Direction.Left;
                }
                //GetTargetPosition(Vector3.left);
                //direction = Direction.Left;
            }
            //swipe right
            if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                if (GetTargetPosition(Vector3.right))
                {
                    direction = Direction.Right;
                }
                //GetTargetPosition(Vector3.right);
                //direction = Direction.Right;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject colliderObject = other.gameObject;
        switch (colliderObject.tag)
        {
            case "DownRight":
                nextDirection = direction == Direction.Forward ? Direction.Right : direction == Direction.Left ? Direction.Backward : nextDirection;
                break;

            case "UpRight":
                nextDirection = direction == Direction.Backward ? Direction.Right : direction == Direction.Left ? Direction.Forward : nextDirection;
                break;

            case "DownLeft":
                nextDirection = direction == Direction.Right ? Direction.Backward : direction == Direction.Forward ? Direction.Left : nextDirection;
                break;

            case "UpLeft":
                nextDirection = direction == Direction.Right ? Direction.Forward : direction == Direction.Backward ? Direction.Left : nextDirection;
                break;

            case "Stack":
                Stack(colliderObject);
                break;

            case "BridgeBlock":
                Unstack(colliderObject);
                break;

            case "WinBlock":
                Win(colliderObject);
                
                break;
        }
    }

    private void Stack(GameObject newStack)
    {
        if (numOfStacks != 0)
        {
            // Move model higher
            modelTransform.position += Vector3.up * 0.4f;
            // Move stack holder higher
            stackHolder.transform.position += Vector3.up * 0.4f;
        }

        // Add new stack to stack holder
        numOfStacks += 1;
        newStack.tag = "Untagged";
        newStack.transform.SetParent(stackHolder.transform);
        newStack.transform.position = modelTransform.position + Vector3.up * (-0.1f + numOfStacks * -stackDistance);

        // Increase score
        score += 1;
    }

    private void Unstack(GameObject bridgeObject)
    {
        // Move model lower
        modelTransform.position -= Vector3.up * 0.4f;

        // Destroy a stack in stack holder
        numOfStacks -= 1;
        Destroy(stackHolder.transform.GetChild(0).gameObject);

        // Create new flat over the brigde
        GameObject newYellowFlat = Instantiate(yellowFlatPrefabs) as GameObject;
        newYellowFlat.transform.SetParent(bridgeObject.transform);
        newYellowFlat.transform.position = bridgeObject.transform.position + new Vector3(0, 0.02f, 0);
        bridgeObject.tag = "Untagged";
    }

    private void Win(GameObject winObject)
    {
        // Set winning condition
        isWinning = true;
        winObject.tag = "Untagged";
        UIManager.Ins.SetScoreValue(score);

        // Destroy all stacks in stack holder
        foreach (Transform child in stackHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Change player model transform
        modelTransform.position -= Vector3.up * 0.4f * numOfStacks;
        modelTransform.eulerAngles = Vector3.up * 0;

        // Activate winning effects
        closedArk.SetActive(false);
        openArk.SetActive(true);
        foreach (Transform child in winParticlesObject.transform)
        {
            child.GetComponent<ParticleSystem>().Play();
        }

        // Play winning animation
        modelTransform.gameObject.GetComponent<Animator>().Play("Take 2");
        Invoke("CompleteGame", 2.5f);
    }

    private void CompleteGame()
    {
        Time.timeScale = 0;
        UIManager.Ins.ActivateCompleteMenu();

    }
}
