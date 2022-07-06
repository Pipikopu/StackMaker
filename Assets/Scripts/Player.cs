using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform playerTransform;
    public Transform modelTransform;
    public Transform stack;

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


    //inside class
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;


    // Start is called before the first frame update
    void Start()
    {
        direction = Direction.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (direction == Direction.Idle)
        {
            Swipe();
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (GetTargetPosition(Vector3.forward)) {
                    direction = Direction.Forward;
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (GetTargetPosition(Vector3.back))
                {
                    direction = Direction.Backward;
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (GetTargetPosition(Vector3.right))
                {
                    direction = Direction.Right;
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (GetTargetPosition(-Vector3.right))
                {
                    direction = Direction.Left;
                }
            }
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
        if (Physics.Raycast(playerTransform.position - Vector3.up * 0.5f, playerTransform.TransformDirection(directionVec), out RaycastHit hit, Mathf.Infinity, layer_mask))
        {
            targetPosition = playerTransform.position + directionVec * (hit.distance - 0.5f);
            Debug.Log("Hit Wall");
            Debug.Log(targetPosition);
            startPosition = playerTransform.position;
            return true;
        }
        return false;
    }


    public void SetNextDirection(int dir)
    {
        if (dir == 0) //DownRight
        {
            if (direction == Direction.Forward)
            {
                nextDirection = Direction.Right;
            }
            else if (direction == Direction.Left)
            {
                nextDirection = Direction.Backward;
            }
        }
        else if (dir == 1) //UpRight
        {
            if (direction == Direction.Backward)
            {
                nextDirection = Direction.Right;
            }
            else if (direction == Direction.Left)
            {
                nextDirection = Direction.Forward;
            }
        }
        else if(dir == 2) //DownLeft
        {
            if (direction == Direction.Right)
            {
                nextDirection = Direction.Backward;
            }
            else if (direction == Direction.Forward)
            {
                nextDirection = Direction.Left;
            }
        }
        else if(dir == 3) //UpLeft
        {
            if (direction == Direction.Right)
            {
                nextDirection = Direction.Forward;
            }
            else if (direction == Direction.Backward)
            {
                nextDirection = Direction.Left;
            }
        }
    }

    void Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //save began touch 2d point
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //create vector from the two points
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            //normalize the 2d vector
            currentSwipe.Normalize();

            //swipe upwards
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
}
