using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager {

	public static PlayerTeam playerTeam;

	public static int bossTeamId;
	public static BossTeam finalBossTeam;

	public static int[] monstersAvailableId = new int[3];
	public static MonsterTeam[] monstersAvailable = new MonsterTeam[3];

	public static int[] cardsAvailableId = new int[5];
	public static ActionsData[] cardAvailable = new ActionsData[5];

	public static int gold;
	public static int soul;
	public static int day;
	public static bool actionDone;
	public static bool combatEnded;
	public static bool isBossFight;

	public static int goldReward;
	public static int soulReward;
	public static int daysToRemove;

	public static bool isEditor;
	public static bool isCampaignGenerated;

	public static int itemToBuy;
	public static int itemPrice;

	public static List<SummonerData> L_Summoners = new List<SummonerData> ();
	public static List<SupportData> L_Supports = new List<SupportData> ();
	public static List<SummonerData> L_Enemies = new List<SummonerData> ();
	public static BossTeam[] L_bossTeams;
	public static MonsterTeam[] L_campaignTeams;



	public static void Init () {
		LoadData ();
		LoadPlayerTeam ();

		LoadCampaign ();

		/*if (isCampaignGenerated == false) {
			GenerateCampaign ();
		}*/
	}

	#region Load DATA
	public static void LoadPlayerTeam () {
		PlayerTeam[] teams = JsonSave.LoadPlayerTeam ();
		playerTeam = teams [0];
	}

	public static void LoadData () {
		if (DictionaryManager.isActionLoaded == false) {
			DictionaryManager.LoadActions ();
		}

		if (DictionaryManager.isEtatsLoaded == false) {
			DictionaryManager.LoadEtats ();
		}

		SummonerData[] summoners = JsonSave.LoadSummoners ();
		for (int i = 0; i < summoners.Length; i++) {
			L_Summoners.Add(summoners[i]);
		}

		SummonerData[] enemies = JsonSave.LoadEnemies ();
		for (int i = 0; i < enemies.Length; i++) {
			L_Enemies.Add(enemies[i]);
			//print ("le name de lenemy est : " + L_Enemies[i].summonerName);
		}

		SupportData[] supports = JsonSave.LoadSupports ();
		for (int i = 0; i < supports.Length; i++) {
			L_Supports.Add(supports[i]);
			//print ("le name du support est : " + L_Supports[i].supportName);
		}

		L_bossTeams = JsonSave.LoadBossTeam ();

		L_campaignTeams = JsonSave.LoadMonsterTeam ();
	}
	#endregion

	#region Generate / Load Campaign
	public static void GenerateCampaign () {
		day = 50;
		gold = 0;
		soul = 0;
		actionDone = false;
		isEditor = false;
		combatEnded = false;
		isBossFight = false;

		GeneratePlayerTeam ();
		GenerateBossTeam ();
		GenerateAvailableMonsters ();
		GenerateAvailableSkills ();

		isCampaignGenerated = true;
	}

	public static void LoadCampaign() {
		Campaign campaignTemp = JsonSave.LoadCampaign ();

		gold = campaignTemp.gold;
		soul = campaignTemp.soul;
		day = campaignTemp.day;
		actionDone = campaignTemp.actionDone;
		combatEnded = campaignTemp.combatEnded;
		isBossFight = campaignTemp.isBossFight;
		isEditor = false;
		isCampaignGenerated = campaignTemp.isCampaignGenerated;

		goldReward = campaignTemp.goldReward;
		soulReward = campaignTemp.soulReward;
		daysToRemove = campaignTemp.daysToRemove;

		bossTeamId = campaignTemp.bossTeamId;
		monstersAvailableId = campaignTemp.monstersAvailableId;
		cardsAvailableId = campaignTemp.cardsAvailableId;

		finalBossTeam = L_bossTeams [bossTeamId];
		for (int i = 0; i < monstersAvailableId.Length; i++) {
			monstersAvailable [i] = L_campaignTeams [monstersAvailableId[i]];
		}

		for (int i = 0; i < cardsAvailableId.Length; i++) {
			ActionsData actionTemp = DictionaryManager.GetAction (cardsAvailableId[i]);
			int priceMin = actionTemp.price - ((actionTemp.price * 20) / 100);
			int priceMax = actionTemp.price + ((actionTemp.price * 20) / 100);
			actionTemp.price = Random.Range(priceMin, priceMax);
			cardAvailable [i] = actionTemp;
		}
	}

	public static void SaveCampaign () {
		JsonSave.SavePlayerTeam (playerTeam);
		JsonSave.SaveCampaign ();
	}
	#endregion

	#region Generate Data
	public static void GenerateBossTeam () {
		bossTeamId = Random.Range (0, L_bossTeams.Length);		
	}

	public static void GenerateAvailableMonsters () {
		for (int i = 0; i < monstersAvailableId.Length; i++) {
			monstersAvailableId [i] = Random.Range (0, L_campaignTeams.Length);
		}

		for (int i = 0; i < monstersAvailableId.Length; i++) {
			monstersAvailable [i] = L_campaignTeams [monstersAvailableId[i]];
		}
	}

	public static void GenerateAvailableSkills () {
		for (int i = 0; i < cardsAvailableId.Length; i++) {
			cardsAvailableId [i] = Random.Range (0, DictionaryManager.actionsDictionary.Count);
		}

		for (int i = 0; i < cardsAvailableId.Length; i++) {
			ActionsData actionTemp = DictionaryManager.GetAction (cardsAvailableId[i]);
			int priceMin = actionTemp.price - ((actionTemp.price * 20) / 100);
			int priceMax = actionTemp.price + ((actionTemp.price * 20) / 100);
			actionTemp.price = Random.Range(priceMin, priceMax);
			cardAvailable [i] = actionTemp;
		}
	}

	public static void GeneratePlayerTeam () {
		playerTeam.mainSummoner = L_Summoners [0];
		playerTeam.mainSummoner.nbDiceMax = 1;

		playerTeam.supports [0] = L_Supports [4];
		playerTeam.supports [0].nbDiceMax = 1;

		playerTeam.supports [1] = L_Supports [5];
		playerTeam.supports [1].nbDiceMax = 1;
	}
	#endregion

	public static void AddDay () {
		daysToRemove = 0;
		combatEnded = false;

		ChangeDay ();
	}

	public static void ChangeDay () {
		GenerateAvailableMonsters ();
		GenerateAvailableSkills ();

		JsonSave.SaveCampaign ();
	}

	public static void AddGoldReward () {
		gold += goldReward;
		goldReward = 0;

		JsonSave.SaveCampaign ();
	}

	public static void AddSoulReward () {
		soul += soulReward;
		soulReward = 0;

		JsonSave.SaveCampaign ();
	}

	public static void Rest () {
		daysToRemove = 1;
		int hpRestored = (playerTeam.mainSummoner.pvMax * Random.Range (20, 40)) / 100;
		playerTeam.mainSummoner.pv += hpRestored;
		if (playerTeam.mainSummoner.pv > playerTeam.mainSummoner.pvMax) {
			playerTeam.mainSummoner.pv = playerTeam.mainSummoner.pvMax;
		}

		SaveCampaign ();
	}

	public static void ChangePlayerAction (int newCharacterType, int newCharacterId, int newSlotId) {
		if (newCharacterType == 1) {
			playerTeam.mainSummoner.L_skills [newSlotId] = cardsAvailableId [itemToBuy];
		} else {
				playerTeam.supports [newCharacterId].L_skills [newSlotId] = cardsAvailableId[itemToBuy];
		}
	}

	public static void BuyItem (int newCharacterType, int newCharacterId, int newSlotId) {
		if (newCharacterType == 1) {
			if (itemToBuy == 0) {
				playerTeam.mainSummoner.nbDice += 1;
			}
		} else {
			if (itemToBuy == 0) {
				playerTeam.supports [newCharacterId].nbDice += 1;
			}
		}
	}

}
