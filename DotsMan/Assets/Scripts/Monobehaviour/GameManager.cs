using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

	public static GameManager instance;

    public int score;
    public TextMeshProUGUI pelletsUI;
    public GameObject titleUI, gameUI, winUI, looseUI;
	public void Awake()
	{
		instance = this;
	}

	//// Start is called before the first frame update
	//void Start()
	//{

	//}

	//// Update is called once per frame
	//void Update()
	//{

	//}

	public void Reset()
	{
		score = 0;
    }
    public void InGame()
    {

    }
    public void Win()
    {

    }
    public void Loose()
    {

    }
}
