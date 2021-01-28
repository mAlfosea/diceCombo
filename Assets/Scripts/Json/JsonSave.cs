using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonSave {
	
	public static SaveData saveData;

	public static string jsonPath = "/data.json";

	public static string summonersJsonPath = "/Json/summoners.json";
	public static string supportsJsonPath = "/Json/supports.json";
	public static string enemiesJsonPath = "/Json/enemies.json";

	public static string actionsJsonPath = "/Json/actions.json";
	public static string etatsJsonPath = "/Json/etats.json";

	public static string playerTeamJsonPath = "/Json/playerTeam.json";
	public static string enemyTeamJsonPath = "/Json/enemyTeam.json";

	public static string campaignJsonPath = "/Json/campaign.json";
	public static string bossTeamJsonPath = "/Json/bossTeam.json";
	public static string monsterTeamJsonPath = "/Json/monsterTeam.json";

	#region Save Summoner / Enemies
	public static void SaveSummoners(List<GameObject> newSummoner)
	{
		string jsonContent = CharactersToJson (newSummoner);

		string filePath = Application.dataPath + summonersJsonPath;
		File.WriteAllText (filePath, jsonContent);		
	}
	public static void SaveEnemies(List<GameObject> newSummoner)
	{
		string jsonContent = CharactersToJson (newSummoner);

		string filePath = Application.dataPath + enemiesJsonPath;
		File.WriteAllText (filePath, jsonContent);		
	}

	public static void SaveSummonersFromEditor(List<SummonerData> newSummonerList) {
		SummonerData[] summonersArray = newSummonerList.ToArray ();
		string jsonContent = JsonHelper.ToJson (summonersArray, true);

		string filePath = Application.dataPath + summonersJsonPath;
		File.WriteAllText (filePath, jsonContent);		
	}

	public static void SaveEnemiesFromEditor(List<SummonerData> newSummonerList) {
		SummonerData[] summonersArray = newSummonerList.ToArray ();
		string jsonContent = JsonHelper.ToJson (summonersArray, true);

		string filePath = Application.dataPath + enemiesJsonPath;
		File.WriteAllText (filePath, jsonContent);		
	}

	public static void SaveSupportsFromEditor(List<SupportData> newSupportsList) {
		SupportData[] supportsArray = newSupportsList.ToArray ();
		string jsonContent = JsonHelper.ToJson (supportsArray, true);

		string filePath = Application.dataPath + supportsJsonPath;
		File.WriteAllText (filePath, jsonContent);		
	}

	public static void SavePlayerTeam (PlayerTeam newPlayerTeam) {
		PlayerTeam[] playerTeam = new PlayerTeam[1];
		playerTeam [0] = newPlayerTeam;

		string jsonContent = JsonHelper.ToJson (playerTeam, true);

		string filePath = Application.dataPath + playerTeamJsonPath;
		File.WriteAllText (filePath, jsonContent);	
	}

	public static void SaveEnemyTeam (EnemyTeam newEnemyTeam) {
		EnemyTeam[] enemyTeam = new EnemyTeam[1];
		enemyTeam [0] = newEnemyTeam;

		string jsonContent = JsonHelper.ToJson (enemyTeam, true);

		string filePath = Application.dataPath + enemyTeamJsonPath;
		File.WriteAllText (filePath, jsonContent);	
	}
	#endregion

	#region Load Summoner / Supports / Enemies
	public static SummonerData[] LoadSummoners () {
		string filePath = Application.dataPath + summonersJsonPath;
		string jsonForm = File.ReadAllText(filePath); 

		SummonerData[] summoners = JsonHelper.FromJson<SummonerData> (jsonForm);
		return summoners;
	}
	public static SupportData[] LoadSupports () {
		string filePath = Application.dataPath + supportsJsonPath;
		string jsonForm = File.ReadAllText(filePath); 

		SupportData[] supports = JsonHelper.FromJson<SupportData> (jsonForm);
		return supports;
	}
	public static SummonerData[] LoadEnemies () {
		string filePath = Application.dataPath + enemiesJsonPath;
		string jsonForm = File.ReadAllText(filePath); 

		SummonerData[] summoners = JsonHelper.FromJson<SummonerData> (jsonForm);
		return summoners;
	}
	#endregion

	#region Character To Json
	private static string CharactersToJson (List<GameObject> newSummoner) {
		saveData.summoners = new SummonerData[newSummoner.Count];

		for (int i = 0; i < newSummoner.Count; i++) {
			Summoner summonerScript = newSummoner[i].GetComponent<Summoner>();
			saveData.summoners[i].id = summonerScript.id;
			saveData.summoners[i].idOriginal = summonerScript.idOriginal;
			saveData.summoners[i].tier = summonerScript.tier;
			saveData.summoners[i].nbDiceMax = summonerScript.nbDiceMax;
			saveData.summoners[i].nbDice = summonerScript.nbDice;
			saveData.summoners[i].nbSkillSlots = summonerScript.nbSkillSlots;
			saveData.summoners[i].nbItemSlots = summonerScript.nbItemSlots;
			saveData.summoners[i].summonerName = summonerScript.summonerName;
			saveData.summoners[i].idAvatar = summonerScript.idAvatar;
			saveData.summoners[i].pvMax = summonerScript.pvMax;
			saveData.summoners[i].L_skills = summonerScript.L_skills;
			saveData.summoners[i].L_talents = summonerScript.L_talents;
			saveData.summoners[i].L_etats = summonerScript.L_etats;
			saveData.summoners[i].characterType = summonerScript.characterType;
		}

		string dataAsJson = JsonHelper.ToJson (saveData.summoners, true);
		return dataAsJson;
	}
	#endregion

	#region Load Actions / Etats
	public static ActionsData[] LoadActions () {
		string filePath = Application.dataPath + actionsJsonPath;
		string jsonForm = File.ReadAllText(filePath); 

		ActionsData[] actions = JsonHelper.FromJson<ActionsData> (jsonForm);
		return actions;
	}

	public static EtatsData[] LoadEtats () {
		string filePath = Application.dataPath + etatsJsonPath;
		string jsonForm = File.ReadAllText(filePath); 

		EtatsData[] etats = JsonHelper.FromJson<EtatsData> (jsonForm);
		return etats;
	}
	#endregion

	#region Load Teams
	public static PlayerTeam[] LoadPlayerTeam () {
		string filePath = Application.dataPath + playerTeamJsonPath;
		string jsonForm = File.ReadAllText(filePath); 

		PlayerTeam[] playerTeam = JsonHelper.FromJson<PlayerTeam> (jsonForm);
		return playerTeam;
	}

	public static EnemyTeam[] LoadEnemyTeam () {
		string filePath = Application.dataPath + enemyTeamJsonPath;
		string jsonForm = File.ReadAllText(filePath); 

		EnemyTeam[] enemyTeam = JsonHelper.FromJson<EnemyTeam> (jsonForm);
		return enemyTeam;
	}

	public static BossTeam[] LoadBossTeam () {
		string filePath = Application.dataPath + bossTeamJsonPath;
		string jsonForm = File.ReadAllText(filePath); 

		BossTeam[] bossTeam = JsonHelper.FromJson<BossTeam> (jsonForm);
		return bossTeam;
	}

	public static MonsterTeam[] LoadMonsterTeam () {
		string filePath = Application.dataPath + monsterTeamJsonPath;
		string jsonForm = File.ReadAllText(filePath); 

		MonsterTeam[] monsterTeam = JsonHelper.FromJson<MonsterTeam> (jsonForm);
		return monsterTeam;
	}
	#endregion

	#region Save / Load Campaign 
	public static void SaveCampaign () {
		Campaign[] campaign = new Campaign[1];
		Campaign campaignToSave = new Campaign ();
		campaignToSave.bossTeamId = CampaignManager.bossTeamId;
		campaignToSave.monstersAvailableId = CampaignManager.monstersAvailableId;
		campaignToSave.cardsAvailableId = CampaignManager.cardsAvailableId;
		campaignToSave.day = CampaignManager.day;
		campaignToSave.gold = CampaignManager.gold;
		campaignToSave.soul = CampaignManager.soul;
		campaignToSave.goldReward = CampaignManager.goldReward;
		campaignToSave.soulReward = CampaignManager.soulReward;
		campaignToSave.daysToRemove = CampaignManager.daysToRemove;
		campaignToSave.actionDone = CampaignManager.actionDone;
		campaignToSave.combatEnded = CampaignManager.combatEnded;
		campaignToSave.isBossFight = CampaignManager.isBossFight;
		campaignToSave.isCampaignGenerated = CampaignManager.isCampaignGenerated;

		campaign [0] = campaignToSave;

		string jsonContent = JsonHelper.ToJson (campaign, true);

		string filePath = Application.dataPath + campaignJsonPath;
		File.WriteAllText (filePath, jsonContent);	
	}

	public static Campaign LoadCampaign () {
		string filePath = Application.dataPath + campaignJsonPath;
		string jsonForm = File.ReadAllText(filePath); 

		Campaign[] campaign = JsonHelper.FromJson<Campaign> (jsonForm);
		return campaign[0];
	}
	#endregion
}