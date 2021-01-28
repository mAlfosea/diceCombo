using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MonsterPanel : MonoBehaviour {

	public GameObject[] monstersGO;
	public Text rewardText;
	public Text soulRewardText;

	private MonsterTeam monsterTeam = new MonsterTeam();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init (MonsterTeam newTeamId) {
		//MonsterTeam[] T_enemies = JsonSave.LoadMonsterTeam ();
		//monsterTeam = T_enemies [newTeamId];
		for (int i = 0; i < this.monstersGO.Length; i++) {
			this.monstersGO [i].SetActive (false);
		}

		monsterTeam = newTeamId;

		for (int i = 0; i < monsterTeam.group.Length; i++) {
			this.monstersGO [i].SetActive (true);

			SummonerData enemyTemp = new SummonerData();

			for (int j = 0; j < CampaignManager.L_Enemies.Count; j++) {
				if (CampaignManager.L_Enemies [j].id == monsterTeam.group [i]) {
					enemyTemp = CampaignManager.L_Enemies [j];
				}
			}

			this.monstersGO [i].transform.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("character_avatars/" + enemyTemp.idAvatar);
		}

		this.rewardText.text = monsterTeam.goldReward.ToString ();
		this.soulRewardText.text = monsterTeam.soulReward.ToString ();
	}

	public void LaunchFight () {
		CampaignManager.daysToRemove = 1;
		CampaignManager.goldReward = this.monsterTeam.goldReward;
		CampaignManager.soulReward = this.monsterTeam.soulReward;
		CampaignManager.combatEnded = false;
		CampaignManager.SaveCampaign ();

		EnemyTeam monsterTeamToFight = new EnemyTeam ();
		monsterTeamToFight.enemies = new SummonerData[this.monsterTeam.group.Length];

		for (int i = 0; i < this.monsterTeam.group.Length; i++) {
			SummonerData enemyTemp = new SummonerData();

			for (int j = 0; j < CampaignManager.L_Enemies.Count; j++) {
				if (CampaignManager.L_Enemies [j].id == monsterTeam.group [i]) {
					enemyTemp = CampaignManager.L_Enemies [j];
				}
			}

			monsterTeamToFight.enemies [i] = enemyTemp;
		}

		JsonSave.SaveEnemyTeam (monsterTeamToFight);

		SceneManager.LoadScene ("combat", LoadSceneMode.Single);
	}
}
