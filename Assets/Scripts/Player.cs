using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    //inside class
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    public Transform playerTransform;
    public Transform modelTransform;
    public GameObject yellowFlatPrefabs;

    private Vector3 targetPosition;
    private Vector3 startPosition;

    public float speed;

    public enum Direction
    {
        Idle,
        Forward,
        Backward,
        Left,
        Right
    }

    private Direction direction = Direction.Idle;
    private Direction nextDirection = Direction.Idle;

    private int numOfStacks = 0;
    public GameObject stackHolder;
    public float stackDistance;

    private int score;
    public Text scoreText;

    public GameObject closedArk;
    public GameObject openArk;

    private bool isWinning;
    public GameObject winParticlesObject;
    public GameObject completeMenu;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        isWinning = false;
        direction = Direction.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (direction == Direction.Idle && !isWinning)
        {
            Swipe();
        }
        else
        {
            playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(playerTransform.position, targetPosition) < 0.1f)
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
                        GetTargetPosition(-Vector3.forward);
                        break;
                    case Direction.Right:
                        GetTargetPosition(Vector3.right);
                        break;
                    case Direction.Left:
                        GetTargetPosition(-Vector3.right);
                        break;
                }
            }
        }
    }

    private bool GetTargetPosition(Vector3 directionVec)
    {
        int layer_mask = LayerMask.GetMask("Wall");
        if (Physics.Raycast(playerTransform.position - Vector3.up * 0.5f, directionVec, out RaycastHit hit, Mathf.Infinity, layer_mask))
        {
            targetPosition = playerTransform.position + directionVec * (hit.distance - 0.5f);
            Debug.Log("Hit Wall");
            Debug.Log(targetPosition);
            startPosition = playerTransform.position;
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

            if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            {
                Debug.Log("up swipe");
                if (GetTargetPosition(Vector3.forward))
                {
                    direction = Direction.Forward;
                }
            }

            //swipe down
            if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            {
                Debug.Log("down swipe");
                if (GetTargetPosition(Vector3.back))
                {
                    direction = Direction.Backward;
                }
            }
            //swipe left
            if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                Debug.Log("left swipe");
                if (GetTargetPosition(-Vector3.right))
                {
                    direction = Direction.Left;
                }
            }
            //swipe right
            if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                Debug.Log("right swipe");
                if (GetTargetPosition(Vector3.right))
                {
                    direction = Direction.Right;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject stack = other.gameObject;
        switch (stack.tag)
        {
            case "DownRight":
                if (direction == Direction.Forward)
                {
                    nextDirection = Direction.Right;
                }
                else if (direction == Direction.Left)
                {
                    nextDirection = Direction.Backward;
                }
                break;

            case "UpRight":
                if (direction == Direction.Backward)
                {
                    nextDirection = Direction.Right;
                }
                else if (direction == Direction.Left)
                {
                    nextDirection = Direction.Forward;
                }
                break;

            case "DownLeft":
                if (direction == Direction.Right)
                {
                    nextDirection = Direction.Backward;
                }
                else if (direction == Direction.Forward)
                {
                    nextDirection = Direction.Left;
                }
                break;

            case "UpLeft":
                if (direction == Direction.Right)
                {
                    nextDirection = Direction.Forward;
                }
                else if (direction == Direction.Backward)
                {
                    nextDirection = Direction.Left;
                }
                break;

            case "Stack":
                //CollectStack();
                if (numOfStacks != 0)
                {
                    modelTransform.position += Vector3.up * 0.4f;
                    stackHolder.transform.position += Vector3.up * 0.4f;
                }
                stack.transform.SetParent(stackHolder.transform);
                Debug.Log("Hit Stack");
                stack.transform.position = modelTransform.position + Vector3.up * (-0.5f + numOfStacks * -stackDistance);
                numOfStacks += 1;
                score += 1;
                scoreText.text = score.ToString();
                stack.tag = "Untagged";
                break;

            case "BridgeBlock":
                modelTransform.position -= Vector3.up * 0.4f;
                numOfStacks -= 1;
                Destroy(stackHolder.transform.GetChild(0).gameObject);
                GameObject newYellowFlat = Instantiate(yellowFlatPrefabs) as GameObject;
                stack.tag = "Untagged";
                newYellowFlat.transform.SetParent(stack.transform);
                newYellowFlat.transform.position = stack.transform.position + new Vector3(0, 0.02f, 0);
                break;
            case "WinBlock":
                Debug.Log("Win Block");
                stack.tag = "Untagged";
                isWinning = true;
                foreach (Transform child in stackHolder.transform)
                {
                    GameObject.Destroy(child.gameObject);
                    modelTransform.position -= Vector3.up * 0.4f;
                }
                modelTransform.position += Vector3.up * 0.4f;
                modelTransform.eulerAngles = Vector3.up * 0;
                foreach (Transform child in winParticlesObject.transform)
                {
                    child.GetComponent<ParticleSystem>().Play();
                }

                modelTransform.gameObject.GetComponent<Animator>().Play("Take 2");
                closedArk.SetActive(false);
                openArk.SetActive(true);
                StartCoroutine(completeGameDelay());
                break;
        }
    }

    IEnumerator completeGameDelay()
    {
        yield return new WaitForSeconds(2.5f);
        completeMenu.SetActive(true);
        Time.timeScale = 0;
    }
}
