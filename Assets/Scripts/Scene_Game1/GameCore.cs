using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	private GameState gameState;

	// �ؿ�����
	public LootManager lootManager;
	// ��ͼ����������
	public MapCreate mapCreator;
	// �÷ֹ���
	public ScoreManager scoreManager;

	// �糡�������ݵ� object
	public SceneMessager sceneMessager;
	// �¼�ϵͳ
	public GameEventSystem eventSystem;

	// С��ѡ�����
	public SquadSelectionPage uiSquadSelectionPage;

	// ��¼�ؿ���ʼ��ʱ��
	[HideInInspector] public long timeStart; 


	// ������ ��Ҫ ������
	// ��ǰ�������� GameCore �������нű�����
	// ��֤���Ե��õ������ű� Start �м��ص�������Դ
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


		// ����ǽ̳̹ؿ�������С��ѡ��
		// ֱ�ӽ�����Ϸ
		if (mapCreator.IsTutorial())
		{
			gameState = GameState.GS_Game_Loading;
			LoadGame(withPreset: true);

			gameState = GameState.GS_Gaming;
			InitGame();
		}
		// ����С��ѡ��
		else
		{
			gameState = GameState.GS_Squad_Selection;
		}
	}

	// ������Ϸ״̬
	// �����Ϸ�Ƿ������������
	void Update()
	{
		// ռ��з�Ŀ��㣬��Ϸ����
		if (mapCreator.ObjectivePoint != null && mapCreator.RedPoint.haveUnit)
		{
			scoreManager.EnemyBaseCaptured();
			Invoke("EndGame", 3);
		}

		// �ҷ�Ŀ����⵽ռ�죬��Ϸ����
		if (mapCreator.HomePoint != null && mapCreator.BluePoint.haveEnemy)
		{
			scoreManager.FriendlyBaseLost();
			Invoke("EndGame", 3);
		}
	}

	// С��ѡ����Ͻ���ս��
	public void StartBattle()
	{
		Debug.Assert(gameState == GameState.GS_Squad_Selection);
		
		// (����)������Ϸ
		gameState = GameState.GS_Game_Loading;	// ���״̬�����ţ��Ժ������״̬��ʾ
		LoadGame();

		// ������ϣ���ʼ�ؿ�ս��
		gameState = GameState.GS_Gaming;
		InitGame();
	}


	// ������Ϸ����Ҫ����Դ
	private void LoadGame(bool withPreset = false)
	{
		// ���ص�ͼ
		if (withPreset)
		{
			mapCreator.SpawnGameWithPreset();
		}
		else
		{
			mapCreator.SpawnGame();
		}
	}

	// ������Ϸǰ��Ҫ��ʼ���ı���
	private void InitGame()
	{
		timeStart = System.DateTime.Now.Ticks;
		uiSquadSelectionPage.gameObject.SetActive(false);
		// TODO bgm �ĵ���ĵط�ȥ
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
