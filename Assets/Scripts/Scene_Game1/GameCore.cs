using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// 游戏核心逻辑控制
/// 
/// 同时起到一个服务定位器的作用，等于是写了一个包含了所有服务的 header
/// 其他脚本只需要绑定这个 header 就可以引用到所有的服务，而无需 Find
/// 
/// </summary>
public class GameCore : MonoBehaviour
{
	public enum GameState
	{
		GS_Squad_Selection,
		GS_Game_Loading,
		GS_Gaming,
		GS_Game_End
	}

	// 游戏状态变量
	// 默认情况下一进来就是小队选择
	private GameState gameState;

	// 关卡掉落
	public LootManager lootManager;
	// 地图加载与生成
	public MapCreate mapCreator;
	// 得分管理
	public ScoreManager scoreManager;

	// 跨场景传数据的 object
	public SceneMessager sceneMessager;
	// 事件系统
	public GameEventSystem eventSystem;

	// 小队选择界面
	public SquadSelectionPage uiSquadSelectionPage;

	// 记录关卡开始的时间
	[HideInInspector] public long timeStart; 


	// 【【【 重要 】】】
	// 当前在设置中 GameCore 晚于所有脚本加载
	// 保证可以调用到其他脚本 Start 中加载的所有资源
    void Start()
	{
		Debug.Assert(lootManager != null);
		Debug.Assert(mapCreator != null);
		Debug.Assert(scoreManager != null);

		sceneMessager = GameObject.Find("SceneMessager").GetComponent<SceneMessager>();
		Debug.Assert(sceneMessager != null);
		Debug.Assert(eventSystem != null);

		// 
		scoreManager.Initialize();
		scoreManager.stageName = mapCreator.mapToLoad;


		// 如果是教程关卡则跳过小队选择
		// 直接进入游戏
		if (mapCreator.IsTutorial())
		{
			gameState = GameState.GS_Game_Loading;
			LoadGame(withPreset: true);

			gameState = GameState.GS_Gaming;
			InitGame();
		}
		// 否则小队选择
		else
		{
			gameState = GameState.GS_Squad_Selection;
		}
	}

	// 更新游戏状态
	// 检测游戏是否满足结束条件
	void Update()
	{
		// 占领敌方目标点，游戏结束
		if (mapCreator.ObjectivePoint != null && mapCreator.RedPoint.haveUnit)
		{
			scoreManager.EnemyBaseCaptured();
			Invoke("EndGame", 3);
		}

		// 我方目标点遭到占领，游戏结束
		if (mapCreator.HomePoint != null && mapCreator.BluePoint.haveEnemy)
		{
			scoreManager.FriendlyBaseLost();
			Invoke("EndGame", 3);
		}
	}

	// 小队选择完毕进入战斗
	public void StartBattle()
	{
		Debug.Assert(gameState == GameState.GS_Squad_Selection);
		
		// (阻塞)加载游戏
		gameState = GameState.GS_Game_Loading;	// 这个状态先留着，以后可以做状态显示
		LoadGame();

		// 加载完毕，开始关卡战役
		gameState = GameState.GS_Gaming;
		InitGame();
	}


	// 加载游戏中需要的资源
	private void LoadGame(bool withPreset = false)
	{
		// 加载地图
		if (withPreset)
		{
			mapCreator.SpawnGameWithPreset();
		}
		else
		{
			mapCreator.SpawnGame();
		}
	}

	// 进入游戏前需要初始化的变量
	private void InitGame()
	{
		timeStart = System.DateTime.Now.Ticks;
		uiSquadSelectionPage.gameObject.SetActive(false);
		// TODO bgm 改到别的地方去
		scoreManager.startBGM();
	}


	public void EndGame()
	{
		gameState = GameState.GS_Game_End;
		scoreManager.OnGameEnd();
		lootManager.stopRecordLooting();
		// EndGameLoot
		SceneManager.LoadScene("GameEnd");
	}

	public bool IsGaming()
	{
		return gameState == GameState.GS_Gaming;
	}

}
