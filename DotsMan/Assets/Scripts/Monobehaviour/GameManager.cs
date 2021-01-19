using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Entities;

public class GameManager : MonoBehaviour
{

	public static GameManager instance;

    public int score, level;
	public TextMeshProUGUI pelletsUI;
	public TextMeshProUGUI scoreTextUI;
	public GameObject titleUI, gameUI, winUI, loseUI;
	public void Awake()
	{
		instance = this;
		Reset();
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
		SwitchUI(titleUI);
		level = 0;
		LoadLevel(0);
    }
    public void InGame()
    {
		SwitchUI(gameUI);
	}
    public void Win()
    {

		SwitchUI(winUI);
	}
    public void Loose()
    {

		SwitchUI(loseUI);
	}

	public void SwitchUI(GameObject newUI)
	{
		titleUI.SetActive(false);
		gameUI.SetActive(false);
		winUI.SetActive(false);
		loseUI.SetActive(false);

		newUI.SetActive(true);
	}

	public void AddPoints(int points)
	{
		score += points;
		scoreTextUI.text = "Score : "+score.ToString();
	}

	public void LoadLevel(int newLevel)
	{
		UnloadLevel();
		SceneManager.LoadScene($"jlevel{newLevel}",LoadSceneMode.Additive);//keep the ui scene
		level = newLevel;

	}
	public AsyncOperation UnloadLevel()
	{

		//cleanup entities as they don't get deleted with a scene unload
		var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		foreach( var entity in entityManager.GetAllEntities())
		{
			entityManager.DestroyEntity(entity);
		}

		if (SceneManager.GetSceneByName($"jlevel{level}").isLoaded)
		{
			Debug.Log($"scene properly unloaded loaded {level}");
			var ao = SceneManager.UnloadSceneAsync($"jlevel{level}");
			return ao;
		}
		else
		{
			Debug.LogWarning($"scene not properly unloaded {level}");
		}
		return null;
	}

	public void NextLevel()
	{
		InGame();
		
		LoadLevel(level + 1);
	}
}
