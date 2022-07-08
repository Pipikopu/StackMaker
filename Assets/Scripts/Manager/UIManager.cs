using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public GameObject completeMenu;
    public Text scoreValue;

    public void ActivateCompleteMenu()
    {
        completeMenu.SetActive(true);
    }

    public void DeactivateCompleteMenu()
    {
        completeMenu.SetActive(true);
    }

    public void SetScoreValue(int score)
    {
        scoreValue.text = score.ToString();
    }
}
