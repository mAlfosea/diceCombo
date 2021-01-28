using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct SaveData  {

	public SummonerData[] summoners;
	public SupportData[] supports;
	public ActionsData[] actions;
	public PlayerTeam playerTeam;
	public EnemyTeam enemyTeam;
	public Fight fight;
}

#region Characters
[System.Serializable]
public struct SummonerData {

	public int id;
	public int idOriginal;
	public int tier;
	public int nbDiceMax;
	public int nbDice;
	public int nbSkillSlots;
	public int nbItemSlots;
	public string summonerName;
	public string idAvatar;
	public int pvMax;
	public int pv;
	public int force;
	public int armure;
	public List<int> L_skills;
	public List<int> L_talents;
	public List<EtatsData> L_etats;
	public int characterType;
}

[System.Serializable]
public struct SupportData {

	public int id;
	public int idOriginal;
	public int tier;
	public int nbDiceMax;
	public int nbDice;
	public int nbSkillSlots;
	public string supportName;
	public string idAvatar;
	public List<int> L_skills;
	public List<int> L_talents;
	public List<EtatsData> L_etats;
	public int characterType;
}
#endregion

#region Actions
[System.Serializable]
public class ActionsData {

	public int id;
	public string name;
	public string description;
	public int price;
	public int type;
	public int color;
	public int targetType;
	public CombatActionData[] combatActions;
	public UpgradeData[] upgradeRequirements;

	public ActionsData () {
	}
}

[System.Serializable]
public struct CombatActionData {

	public int target;
	public string modulateur;
	public int damage;
	public int typeDmg;
	public int colorDmg;
	public string idAnim;
	public CombatEtatData[] etats;
	public ActionCondition[] conditions;
}

[System.Serializable]
public struct ActionCondition {

	public int target;
	public int type;
	public int mod;
	public int value;
}

[System.Serializable]
public struct UpgradeData {

	public int idItem;
	public int itemAmount;
}
#endregion
	
[System.Serializable]
public class EtatsData {

	public int id;
	public string name;
	public string description;
	public int duration;
	public int value;
	public int turnStateApply;
	public bool isShowable;
	public bool isRemovable;

	public EtatsData () {
	}
}

[System.Serializable]
public struct CombatEtatData {

	public int target;
	public string modulateur;
	public int idEtat;
	public int duration;
	public int value;
}
	
[System.Serializable]
public struct Campaign {

	public int bossTeamId;
	public int[] monstersAvailableId;
	public int[] cardsAvailableId;
	public int day;
	public int gold;
	public int soul;
	public int goldReward;
	public int soulReward;
	public int daysToRemove;
	public bool actionDone;
	public bool combatEnded;
	public bool isBossFight;
	public bool isCampaignGenerated;
}

#region Data des Team + Fight
[System.Serializable]
public struct PlayerTeam {

	public SummonerData mainSummoner;
	public SupportData[] supports;
}

[System.Serializable]
public struct EnemyTeam {

	public SummonerData[] enemies;
}

[System.Serializable]
public struct BossTeam {

	public int[] group;
}

[System.Serializable]
public struct MonsterTeam {

	public int[] group;
	public int goldReward;
	public int soulReward;
}

[System.Serializable]
public struct Fight {

	public PlayerTeam playerTeam;
	public EnemyTeam enemyTeam;
}
#endregion