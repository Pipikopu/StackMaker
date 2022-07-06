using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StackPlayer : MonoBehaviour
{
    public Transform modelTransform;
    public float stackDistance;
    public GameObject yellowFlatPrefabs;
    public GameObject stacks;
    private int score;
    public Text scoreText;

    public GameObject closedArk;
    public GameObject openArk;

    public GameObject winParticlesObject;

    private int numOfStacks = 0;

    private GameObject playerModel;

    public GameObject completeMenu;

    private void Start()
    {
        score = 0;
        playerModel = modelTransform.gameObject;
        //player = GameObject.Find("PlayerModel");
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject stack = other.gameObject;
        switch (stack.tag)
        {
            case "DownRight":
                //nextDirection = Direction.Forward;
                playerModel.GetComponent<Player>().SetNextDirection(0);
                stack.GetComponent<Animator>().Play("Take 002");
                break;
            case "UpRight":
                //nextDirection = Direction.Backward;
                playerModel.GetComponent<Player>().SetNextDirection(1);
                stack.GetComponent<Animator>().Play("Take 002");
                break;
            case "DownLeft":
                //nextDirection = Direction.Right;
                playerModel.GetComponent<Player>().SetNextDirection(2);
                stack.GetComponent<Animator>().Play("Take 002");
                break;
            case "UpLeft":
                //nextDirection = Direction.Left;
                playerModel.GetComponent<Player>().SetNextDirection(3);
                stack.GetComponent<Animator>().Play("Take 002");
                break;
            case "Stack":
                //CollectStack();
                if (numOfStacks != 0)
                {
                    modelTransform.position += Vector3.up * 0.4f;
                    stacks.transform.position += Vector3.up * 0.4f;
                }
                stack.transform.SetParent(stacks.transform);
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
                Destroy(stacks.transform.GetChild(0).gameObject);
                GameObject newYellowFlat = Instantiate(yellowFlatPrefabs) as GameObject;
                stack.tag = "Untagged";
                newYellowFlat.transform.SetParent(stack.transform);
                newYellowFlat.transform.position = stack.transform.position + new Vector3(0, 0.02f, 0);
                break;
            case "WinBlock":
                Debug.Log("Win Block");
                stack.tag = "Untagged";
                foreach (Transform child in stacks.transform)
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
