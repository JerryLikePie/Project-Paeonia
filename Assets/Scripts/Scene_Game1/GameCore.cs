using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// ��Ϸ�����߼�����
/// 
/// ͬʱ��һ������λ�������ã�������д��һ�����������з���� header
/// �����ű�ֻ��Ҫ����� header �Ϳ������õ����еķ��񣬶����� Find
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

	// ��Ϸ״̬����
	// Ĭ�������һ��������С��ѡ��
	private GameState gameState = GameState.GS_Squad_Selection;

	// �ؿ�����
	public LootManager lootManager;
	// ��ͼ����������
	public MapCreate mapCreator;
	// �÷ֹ���
	public ScoreManager scoreManager;

	// �糡�������ݵ� object
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

	// ѡ����Ͻ���ս��
	public void startBattle()
	{
		Debug.Assert(gameState == GameState.GS_Squad_Selection);

		gameState = GameState.GS_Game_Loading;	// �����ţ��Ժ������״̬��ʾ
		// ������Ϸ
		loadGame();

		gameState = GameState.GS_Gaming;
		// TODO bgm �ĵ���ĵط�ȥ
		scoreManager.startBGM();
	}


	// ������Ϸ����Ҫ���ص���Դ
	private void loadGame()
	{
		// ���ص�ͼ
		mapCreator.SpawnGame();
	}

	public void endBattle()
	{

	}

}
