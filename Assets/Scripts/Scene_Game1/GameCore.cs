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

	// GUI-小队选择界面
	public SquadSelectionPage uiSquadSelectionPage;

	// BGMs
	public AudioSource bgmIntense, bgmNormal, bgmIntro;
	// 发现的敌人数量，用于控制音效音量
	private int enemyShown;


	// 记录关卡开始的时间
	[HideInInspector] public long timeStart; 


	// 【【【 重要 】】】
	// 当前在设置中 GameCore 晚于所有脚本加载
	// 保证可以调用到其他脚本 Start 中加载的所有资源
	// 同时这也意味着其他组件在 Start 的时候无法使用 GameCore 中的资源
    void Start()
	{
		Debug.Assert(lootManager != null);
		Debug.Assert(mapCreator != null);
		Debug.Assert(scoreManager != null);
		// SceneMessager 是跨场景对象，动态绑定
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
			LoadGame(BattleType.Tutorial);

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

		// 持续生成敌人


		// 根据敌人数量修改 bgm 音量
		if (bgmIntense.volume < enemyShown / 3.0f)
		{
			setBgmIntenseVolume(bgmIntense.volume + 0.003f);
		}
		else if (bgmIntense.volume > enemyShown / 3.0f)
		{
			setBgmIntenseVolume(bgmIntense.volume - 0.001f);
		}

	}

	// 小队选择完毕进入战斗
	public void StartBattle()
	{
		Debug.Assert(gameState == GameState.GS_Squad_Selection);
		
		// (阻塞)加载游戏
		gameState = GameState.GS_Game_Loading;  // 这个状态先留着，以后可以做状态显示
		if (mapCreator.IsOperation())
		{
			LoadGame(BattleType.Operation);
		}
		else
		{
			LoadGame(BattleType.Normal);
		}

		// 加载完毕，开始关卡战役
		gameState = GameState.GS_Gaming;
		InitGame();
	}

	public enum BattleType { 
		Tutorial,	// 教程关卡
		Normal,		// 普通战斗关卡
		Operation	// 远征关卡
	}


	// 加载游戏中需要的资源
	private void LoadGame(BattleType gameType)
	{
		switch (gameType)
		{
			case BattleType.Tutorial:
				mapCreator.SpawnGameWithPreset();
				break;
			case BattleType.Normal:
				mapCreator.SpawnGame();
				break;
			case BattleType.Operation:
				mapCreator.SpawnGame(generateRandomMap: true);
				break;
		}
	}

	// 进入游戏前需要初始化的变量
	private void InitGame()
	{
		uiSquadSelectionPage.gameObject.SetActive(false);

		timeStart = System.DateTime.Now.Ticks;
		StartBGM();
		enemyShown = 0;
		lootManager.startRecordLooting();
	}

	private void StartBGM()
	{
		if (bgmIntense != null && bgmNormal != null)
		{
			bgmIntro.Play();
			float introLength = bgmIntro.clip.length;
			if (!bgmIntense.isPlaying)
			{
				bgmIntense.PlayDelayed(introLength);
			}
			if (!bgmNormal.isPlaying)
			{
				bgmNormal.PlayDelayed(introLength);
			}
		}
	}

	public void EndGame()
	{
		gameState = GameState.GS_Game_End;
		scoreManager.OnGameEnd();
		lootManager.StopRecordLooting();
		// EndGameLoot
		SceneManager.LoadScene("GameEnd");
	}

	public bool IsGaming()
	{
		return gameState == GameState.GS_Gaming;
	}

	// 设置遇敌 BGM 的音量
	public void setBgmIntenseVolume(float volume)
	{
		bgmIntense.volume = volume;
	}

	public void foundEnemy(int num)
	{
		enemyShown += num;
	}

}
