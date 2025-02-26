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

	// GUI-С��ѡ�����
	public SquadSelectionPage uiSquadSelectionPage;

	// BGMs
	public AudioSource bgmIntense, bgmNormal, bgmIntro;
	// ���ֵĵ������������ڿ�����Ч����
	private int enemyShown;


	// ��¼�ؿ���ʼ��ʱ��
	[HideInInspector] public long timeStart; 


	// ������ ��Ҫ ������
	// ��ǰ�������� GameCore �������нű�����
	// ��֤���Ե��õ������ű� Start �м��ص�������Դ
	// ͬʱ��Ҳ��ζ����������� Start ��ʱ���޷�ʹ�� GameCore �е���Դ
    void Start()
	{
		Debug.Assert(lootManager != null);
		Debug.Assert(mapCreator != null);
		Debug.Assert(scoreManager != null);
		// SceneMessager �ǿ糡�����󣬶�̬��
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
			LoadGame(BattleType.Tutorial);

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

		// �������ɵ���


		// ���ݵ��������޸� bgm ����
		if (bgmIntense.volume < enemyShown / 3.0f)
		{
			setBgmIntenseVolume(bgmIntense.volume + 0.003f);
		}
		else if (bgmIntense.volume > enemyShown / 3.0f)
		{
			setBgmIntenseVolume(bgmIntense.volume - 0.001f);
		}

	}

	// С��ѡ����Ͻ���ս��
	public void StartBattle()
	{
		Debug.Assert(gameState == GameState.GS_Squad_Selection);
		
		// (����)������Ϸ
		gameState = GameState.GS_Game_Loading;  // ���״̬�����ţ��Ժ������״̬��ʾ
		if (mapCreator.IsOperation())
		{
			LoadGame(BattleType.Operation);
		}
		else
		{
			LoadGame(BattleType.Normal);
		}

		// ������ϣ���ʼ�ؿ�ս��
		gameState = GameState.GS_Gaming;
		InitGame();
	}

	public enum BattleType { 
		Tutorial,	// �̳̹ؿ�
		Normal,		// ��ͨս���ؿ�
		Operation	// Զ���ؿ�
	}


	// ������Ϸ����Ҫ����Դ
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

	// ������Ϸǰ��Ҫ��ʼ���ı���
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

	// �������� BGM ������
	public void setBgmIntenseVolume(float volume)
	{
		bgmIntense.volume = volume;
	}

	public void foundEnemy(int num)
	{
		enemyShown += num;
	}

}
