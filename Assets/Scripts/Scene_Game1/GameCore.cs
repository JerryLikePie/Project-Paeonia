using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	private GameState gameState = GameState.GS_Squad_Selection;

	// 关卡掉落
	public LootManager lootManager;
	// 地图加载与生成
	public MapCreate mapCreator;
	// 得分管理
	public ScoreManager scoreManager;

	// 跨场景传数据的 object
	public SceneMgr sceneManager;


    void Start()
	{
		Debug.Assert(lootManager != null);
		Debug.Assert(mapCreator != null);
		Debug.Assert(scoreManager != null);

		sceneManager = GameObject.Find("SceneMgr").GetComponent<SceneMgr>();
		Debug.Assert(sceneManager != null);
	}

	// 
	void Update()
	{

	}

	// 选择完毕进入战斗
	public void startBattle()
	{
		Debug.Assert(gameState == GameState.GS_Squad_Selection);

		gameState = GameState.GS_Game_Loading;	// 先留着，以后可以做状态显示
		// 加载游戏
		loadGame();

		gameState = GameState.GS_Gaming;
		// TODO bgm 改到别的地方去
		scoreManager.startBGM();
	}


	// 加载游戏中需要加载的资源
	private void loadGame()
	{
		// 加载地图
		mapCreator.SpawnGame();
	}

	public void endBattle()
	{

	}

}
