using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManagerEditor : MonoBehaviour {

	public PlayerTeam T_Player;
	public List<SummonerData> L_EnemiesInTeam = new List<SummonerData> ();
	public List<SupportData> L_SupportsInTeam = new List<SupportData> ();
	public EnemyTeam T_Enemy;

	public List<SummonerData> L_Summoners = new List<SummonerData> ();
	public List<SummonerData> L_Enemies = new List<SummonerData> ();
	public List<SupportData> L_Supports = new List<SupportData> ();

	public GameObject playerTeamContainer;
	public GameObject supportTeamContainer;
	public GameObject enemyTeamContainer;

	public GameObject characterInfosPanelPrefab;
	public GameObject supportInfosPanelPrefab;
	public GameObject emptyInfosPanelPrefab;
	public GameObject monsterEmptyInfosPanelPrefab;

	void OnEnable()
	{
		CharacterPanelEditor.E_SummonerUpdated += this.UpdateSummoner;
		CharacterPanelEditor.E_SupportUpdated += this.UpdateSupport;
		CharacterPanelEditor.E_RemoveCharacter += this.RemoveCharacter;
		PopupManager.E_ChangeSkill += this.ChangeAction;
		PopupElement.E_RemoveSkill += this.RemoveAction;
		PopupManager.E_ChangeTalent += this.ChangeTalent;
		PopupElement.E_RemoveTalent += this.RemoveTalent;
		PopupManager.E_ChangeCharacter += this.ChangeCharacter;
	}


	void OnDisable()
	{
		CharacterPanelEditor.E_SummonerUpdated -= this.UpdateSummoner;
		CharacterPanelEditor.E_SupportUpdated -= this.UpdateSupport;
		CharacterPanelEditor.E_RemoveCharacter -= this.RemoveCharacter;
		PopupManager.E_ChangeSkill -= this.ChangeAction;
		PopupElement.E_RemoveSkill -= this.RemoveAction;
		PopupManager.E_ChangeTalent -= this.ChangeTalent;
		PopupElement.E_RemoveTalent -= this.RemoveTalent;
		PopupManager.E_ChangeCharacter -= this.ChangeCharacter;
	}

	// Use this for initialization
	void Start () {
		if (DictionaryManager.isActionLoaded == false) {
			DictionaryManager.LoadActions ();
		}

		if (DictionaryManager.isEtatsLoaded == false) {
			DictionaryManager.LoadEtats ();
		}

		LoadSummoners ();
		LoadSupports ();
		LoadEnemies ();

		LoadPlayerTeam ();
		LoadEnemyTeam ();

		this.GeneratePlayerTeamPanelInfos ();
		this.GenerateEnemyTeamPanelInfos ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LaunchFight () {
		this.SaveSummoners ();
		SceneManager.LoadScene ("combat", LoadSceneMode.Single);
	}

	public void Retour () {
		SceneManager.LoadScene ("mainMenu", LoadSceneMode.Single);
	}

	public void SaveSummoners () {
		JsonSave.SavePlayerTeam (T_Player);
		JsonSave.SaveEnemyTeam (T_Enemy);
	}

	#region Load des Data
	public void LoadPlayerTeam () {
		PlayerTeam[] playerTeam = JsonSave.LoadPlayerTeam();
		for (int i = 0; i < playerTeam.Length; i++) {
			T_Player = playerTeam [i];
			//print ("l'ID du main summoner est : " + T_Player.mainSummoner);
		}
	}

	public void LoadEnemyTeam () {
		EnemyTeam[] enemyTeam = JsonSave.LoadEnemyTeam();
		T_Enemy = enemyTeam [0];
		//print ("l'ID du monstre est : " + T_Enemy.enemies[0]);
	}

	public void LoadSummoners () {
		SummonerData[] summoners = JsonSave.LoadSummoners ();
		for (int i = 0; i < summoners.Length; i++) {
			L_Summoners.Add(summoners[i]);
			//print ("le name du summoner est : " + L_Summoners[i].summonerName);
		}
	}

	public void LoadEnemies () {
		SummonerData[] enemies = JsonSave.LoadEnemies ();
		for (int i = 0; i < enemies.Length; i++) {
			L_Enemies.Add(enemies[i]);
			//print ("le name de lenemy est : " + L_Enemies[i].summonerName);
		}
	}

	public void LoadSupports () {
		SupportData[] supports = JsonSave.LoadSupports ();
		for (int i = 0; i < supports.Length; i++) {
			L_Supports.Add(supports[i]);
			//print ("le name du support est : " + L_Supports[i].supportName);
		}
	}
	#endregion

	#region GeneratePanelInfos
	private void GeneratePlayerTeamPanelInfos () {

		GameObject characterInfosTemp = Instantiate (this.characterInfosPanelPrefab, this.playerTeamContainer.transform.position, this.transform.rotation, this.playerTeamContainer.transform) as GameObject;
		CharacterPanelEditor characterInfosScript = characterInfosTemp.GetComponent<CharacterPanelEditor> ();
		SummonerData summonerScript = T_Player.mainSummoner;

		characterInfosTemp.name = summonerScript.summonerName;
		characterInfosScript.summonerScript = summonerScript;
		characterInfosScript.teamId = 1;
		characterInfosScript.GenerateSummoner ();

		for (int i = 0; i < T_Player.supports.Length; i++) {
					//print (L_Supports[j].id + " : " + L_Supports[j].supportName + " / " + T_Player.supports [i]);
			L_SupportsInTeam.Add (T_Player.supports [i]);
			GameObject supportInfosTemp = Instantiate (this.supportInfosPanelPrefab, this.supportTeamContainer.transform.position, this.transform.rotation, this.supportTeamContainer.transform) as GameObject;
			CharacterPanelEditor supportInfosScript = supportInfosTemp.GetComponent<CharacterPanelEditor> ();
			SupportData supportScript = T_Player.supports [i];

			characterInfosTemp.name = supportScript.supportName;
			supportInfosScript.supportScript = supportScript;
			supportInfosScript.teamId = 1;
			supportInfosScript.GenerateSupport ();
		}

		if (T_Player.supports.Length < 2) {
			for (int i = T_Player.supports.Length; i < 2; i++) {
				this.AddEmptyPanel (1, 2);
			}
		}
	}

	private void GenerateEnemyTeamPanelInfos () {

		for (int i = 0; i < T_Enemy.enemies.Length; i++) {
			L_EnemiesInTeam.Add (T_Enemy.enemies [i]);
			GameObject characterInfosTemp = Instantiate (this.characterInfosPanelPrefab, this.enemyTeamContainer.transform.position, this.transform.rotation, this.enemyTeamContainer.transform) as GameObject;
			CharacterPanelEditor characterInfosScript = characterInfosTemp.GetComponent<CharacterPanelEditor> ();
			SummonerData monsterScript = T_Enemy.enemies [i];

			characterInfosTemp.name = monsterScript.summonerName;
			characterInfosScript.summonerScript = monsterScript;
			characterInfosScript.teamId = 2;
			characterInfosScript.GenerateSummoner ();
		}
		if (T_Enemy.enemies.Length < 3) {
			for (int i = T_Enemy.enemies.Length; i < 3; i++) {
				this.AddEmptyPanel (2, 3);
			}
		}
	}
	#endregion

	private void UpdateSummoner (SummonerData newSummoner) {
		if (newSummoner.characterType == 1) {

			T_Player.mainSummoner = newSummoner;

			/*for (int i = 0; i < L_Summoners.Count; i++) {
				if (L_Summoners [i].id == newSummoner.id) {
					L_Summoners [i] = newSummoner;
				}
			}*/
		} else {
			for (int i = 0; i < L_EnemiesInTeam.Count; i++) {
				if (L_EnemiesInTeam [i].id == newSummoner.id) {
					L_EnemiesInTeam [i] = newSummoner;
					T_Enemy.enemies = L_EnemiesInTeam.ToArray ();
				}
			}
		}
	}

	private void UpdateSupport (SupportData newSupport) {
		for (int i = 0; i < L_SupportsInTeam.Count; i++) {
			if (L_SupportsInTeam [i].id == newSupport.id) {
				L_SupportsInTeam [i] = newSupport;
				T_Player.supports = L_SupportsInTeam.ToArray ();
			}
		}
	}

	public void ChangeCharacter (int newTeamId, int newCharacterType, int newCharacterId, int newCharacterIndex, int newCharacterIdToAdd) {
		if (newCharacterType == 1) {

			for (int j = 0; j < this.playerTeamContainer.transform.childCount; j++) {
				CharacterPanelEditor characterInfosScript = this.playerTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ();
				if (characterInfosScript.summonerScript.id == newCharacterId) {

					Destroy (this.playerTeamContainer.transform.GetChild (j).gameObject);

					for (int k = 0; k < L_Summoners.Count; k++) {
						if (L_Summoners [k].id == newCharacterIdToAdd) {
							T_Player.mainSummoner = L_Summoners [k];

							GameObject characterInfosTemp = Instantiate (this.characterInfosPanelPrefab, this.playerTeamContainer.transform.position, this.transform.rotation, this.playerTeamContainer.transform) as GameObject;
							CharacterPanelEditor characterInfosScriptTemp = characterInfosTemp.GetComponent<CharacterPanelEditor> ();
							SummonerData summonerScript = L_Summoners [k];

							characterInfosTemp.transform.SetSiblingIndex (j);
							characterInfosTemp.name = summonerScript.summonerName;
							characterInfosScriptTemp.summonerScript = summonerScript;
							characterInfosScriptTemp.teamId = 1;
							characterInfosScriptTemp.GenerateSummoner ();
							return;
						}
					}
				}
			}
		} else if (newCharacterType == 2) {
			for (int i = 0; i < L_SupportsInTeam.Count; i++) {
				if (L_SupportsInTeam [i].id == newCharacterId) {

					for (int j = 0; j < this.supportTeamContainer.transform.childCount; j++) {
						CharacterPanelEditor characterInfosScript = this.supportTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ();
						if (characterInfosScript.supportScript.id == newCharacterId) {

							Destroy (this.supportTeamContainer.transform.GetChild (j).gameObject);

							for (int k = 0; k < L_Supports.Count; k++) {
								if (L_Supports [k].id == newCharacterIdToAdd) {

									GameObject characterInfosTemp = Instantiate (this.supportInfosPanelPrefab, this.supportTeamContainer.transform.position, this.transform.rotation, this.supportTeamContainer.transform) as GameObject;
									CharacterPanelEditor characterInfosScriptTemp = characterInfosTemp.GetComponent<CharacterPanelEditor> ();
									SupportData supportScript = L_Supports [k];

									characterInfosTemp.transform.SetSiblingIndex (i);
									characterInfosTemp.name = supportScript.supportName;
									characterInfosScriptTemp.supportScript = supportScript;
									characterInfosScriptTemp.teamId = 1;
									characterInfosScriptTemp.GenerateSupport ();

									L_SupportsInTeam [i] = L_Supports [k];
									T_Player.supports = L_SupportsInTeam.ToArray ();

									return;
								}
							}
						}
					}
				}
			}
			int index = 0;

			for (int j = 0; j < this.supportTeamContainer.transform.childCount; j++) {
				CharacterPanelEditor characterInfosScript = this.supportTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ();
				print (characterInfosScript.supportScript.id + " / " + newCharacterId);
				if (characterInfosScript.supportScript.id == newCharacterId) {
					index = j;
					Destroy (this.supportTeamContainer.transform.GetChild (j).gameObject);

					break;
				}
			}

			for (int i = 0; i < L_Supports.Count; i++) {
				if (L_Supports [i].id == newCharacterIdToAdd) {

					GameObject characterInfosTemp = Instantiate (this.supportInfosPanelPrefab, this.supportTeamContainer.transform.position, this.transform.rotation, this.supportTeamContainer.transform) as GameObject;
					CharacterPanelEditor characterInfosScript = characterInfosTemp.GetComponent<CharacterPanelEditor> ();
					SupportData supportScript = L_Supports [i];

					characterInfosTemp.transform.SetSiblingIndex (index);
					characterInfosTemp.name = supportScript.supportName;
					characterInfosScript.supportScript = supportScript;
					characterInfosScript.teamId = 1;
					characterInfosScript.GenerateSupport ();

					L_SupportsInTeam.Add(L_Supports [i]);
					T_Player.supports = L_SupportsInTeam.ToArray ();

					return;
				}
			}
		} else {
			for (int i = 0; i < L_EnemiesInTeam.Count; i++) {
				if (L_EnemiesInTeam [i].id == newCharacterId) {

					for (int j = 0; j < this.enemyTeamContainer.transform.childCount; j++) {
						CharacterPanelEditor characterInfosScript = this.enemyTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ();
						if (characterInfosScript.summonerScript.id == newCharacterId) {

							Destroy (this.enemyTeamContainer.transform.GetChild (j).gameObject);

							for (int k = 0; k < L_Enemies.Count; k++) {
								if (L_Enemies [k].id == newCharacterIdToAdd) {

									GameObject characterInfosTemp = Instantiate (this.characterInfosPanelPrefab, this.enemyTeamContainer.transform.position, this.transform.rotation, this.enemyTeamContainer.transform) as GameObject;
									CharacterPanelEditor characterInfosScriptTemp = characterInfosTemp.GetComponent<CharacterPanelEditor> ();
									SummonerData summonerScript = L_Enemies [k];

									characterInfosTemp.transform.SetSiblingIndex (i);
									characterInfosTemp.name = summonerScript.summonerName;
									characterInfosScriptTemp.summonerScript = summonerScript;
									characterInfosScriptTemp.teamId = 2;
									characterInfosScriptTemp.GenerateSummoner ();

									L_EnemiesInTeam [i] = L_Enemies [k];
									T_Enemy.enemies = L_EnemiesInTeam.ToArray ();

									return;
								}
							}
						}
					}
				}
			}
			int index = 0;

			for (int j = 0; j < this.enemyTeamContainer.transform.childCount; j++) {
				CharacterPanelEditor characterInfosScript = this.enemyTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ();
				print (characterInfosScript.summonerScript.id + " / " + newCharacterId);
				if (characterInfosScript.summonerScript.id == newCharacterId) {
					index = j;
					Destroy (this.enemyTeamContainer.transform.GetChild (j).gameObject);

					break;
				}
			}

			for (int i = 0; i < L_Enemies.Count; i++) {
				if (L_Enemies [i].id == newCharacterIdToAdd) {

					GameObject characterInfosTemp = Instantiate (this.characterInfosPanelPrefab, this.enemyTeamContainer.transform.position, this.transform.rotation, this.enemyTeamContainer.transform) as GameObject;
					CharacterPanelEditor characterInfosScript = characterInfosTemp.GetComponent<CharacterPanelEditor> ();
					SummonerData summonerScript = L_Enemies [i];

					characterInfosTemp.transform.SetSiblingIndex (index);
					characterInfosTemp.name = summonerScript.summonerName;
					characterInfosScript.summonerScript = summonerScript;
					characterInfosScript.teamId = 2;
					characterInfosScript.GenerateSummoner ();

					L_EnemiesInTeam.Add(L_Enemies [i]);
					T_Enemy.enemies = L_EnemiesInTeam.ToArray ();

					return;
				}
			}
		}
	}

	public void RemoveCharacter (int newTeamId, int newCharacterType, int newId) {
		if (newCharacterType == 1) {
			
		} else if (newCharacterType == 2) {
			for (int i = 0; i < L_SupportsInTeam.Count; i++) {
				if (L_SupportsInTeam [i].id == newId) {
					L_SupportsInTeam.RemoveAt (i);
					T_Player.supports = L_SupportsInTeam.ToArray ();
					this.AddEmptyPanel (newTeamId, newCharacterType);
					return;
				}
			}
		} else {
			for (int i = 0; i < L_EnemiesInTeam.Count; i++) {
				if (L_EnemiesInTeam [i].id == newId) {
					L_EnemiesInTeam.RemoveAt (i);
					T_Enemy.enemies = L_EnemiesInTeam.ToArray ();
					this.AddEmptyPanel (newTeamId, newCharacterType);
					return;
				}
			}
		}
	}

	public void AddEmptyPanel (int newTeamId, int newCharacterType) {
		if (newTeamId == 1) {
			GameObject characterInfosTemp = Instantiate (this.emptyInfosPanelPrefab, this.supportTeamContainer.transform.position, this.transform.rotation, this.supportTeamContainer.transform) as GameObject;
			//CharacterPanelEditor characterInfosScript = characterInfosTemp.GetComponent<CharacterPanelEditor> ();
			//characterInfosScript.teamId = 1;
			PopupElement characterElementScript = characterInfosTemp.GetComponent<PopupElement> ();
			characterElementScript.teamId = newTeamId;
			characterElementScript.characterType = newCharacterType;
		} else {
			GameObject characterInfosTemp = Instantiate (this.monsterEmptyInfosPanelPrefab, this.enemyTeamContainer.transform.position, this.transform.rotation, this.enemyTeamContainer.transform) as GameObject;
			//CharacterPanelEditor characterInfosScript = characterInfosTemp.GetComponent<CharacterPanelEditor> ();
			//characterInfosScript.teamId = 1;
			PopupElement characterElementScript = characterInfosTemp.GetComponent<PopupElement> ();
			characterElementScript.teamId = newTeamId;
			characterElementScript.characterType = newCharacterType;
		}
	}

	#region ChangeAction
	public void ChangeAction (int newTeamId, int newCharacterType, int newCharacterId, int newSkillIndex, int newActionId) {
		if (newTeamId == 1) {
			if (newCharacterType == 1) {
				
				T_Player.mainSummoner.L_skills [newSkillIndex] = newActionId;
				this.playerTeamContainer.transform.GetChild (0).GetComponent<CharacterPanelEditor> ().UpdateActionsSlotImages ();
				return;
				
				/*for (int i = 0; i < L_Summoners.Count; i++) {
					if (L_Summoners [i].id == newCharacterId) {
						L_Summoners [i].L_skills [newSkillIndex] = newActionId;
						this.playerTeamContainer.transform.GetChild (0).GetComponent<CharacterPanelEditor> ().UpdateActionsSlotImages ();
						return;
					}
				}*/
			} else if (newCharacterType == 2) {
				for (int i = 0; i < L_SupportsInTeam.Count; i++) {
					if (L_SupportsInTeam [i].id == newCharacterId) {
						L_SupportsInTeam [i].L_skills [newSkillIndex] = newActionId;

						T_Player.supports = L_SupportsInTeam.ToArray ();

						for (int j = 0; j < this.supportTeamContainer.transform.childCount; j++) {
							if (this.supportTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().supportScript.id == newCharacterId) {
								this.supportTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().UpdateActionsSlotImages ();
								return;
							}
						}
					}
				}
			}
		} else {
			if (newCharacterType == 3) {
				for (int i = 0; i < L_EnemiesInTeam.Count; i++) {
					if (L_EnemiesInTeam [i].id == newCharacterId) {
						L_EnemiesInTeam [i].L_skills [newSkillIndex] = newActionId;

						T_Enemy.enemies = L_EnemiesInTeam.ToArray ();

						for (int j = 0; j < this.enemyTeamContainer.transform.childCount; j++) {
							if (this.enemyTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().summonerScript.id == newCharacterId) {
								this.enemyTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().UpdateActionsSlotImages ();
								return;
							}
						}
					}
				}
			}
		}
	}

	public void RemoveAction (int newTeamId, int newCharacterType, int newCharacterId, int newSkillIndex) {
		if (newTeamId == 1) {
			if (newCharacterType == 1) {

				T_Player.mainSummoner.L_skills [newSkillIndex] = 0;
				this.playerTeamContainer.transform.GetChild (0).GetComponent<CharacterPanelEditor> ().UpdateActionsSlotImages ();
				return;

				/*for (int i = 0; i < L_Summoners.Count; i++) {
					if (L_Summoners [i].id == newCharacterId) {
						L_Summoners [i].L_skills [newSkillIndex] = 0;
						this.playerTeamContainer.transform.GetChild (0).GetComponent<CharacterPanelEditor> ().UpdateActionsSlotImages ();
						return;
					}
				}*/

			} else if (newCharacterType == 2) {
				for (int i = 0; i < L_SupportsInTeam.Count; i++) {
					if (L_SupportsInTeam [i].id == newCharacterId) {
						L_SupportsInTeam [i].L_skills [newSkillIndex] = 0;

						T_Player.supports = L_SupportsInTeam.ToArray ();

						for (int j = 0; j < this.supportTeamContainer.transform.childCount; j++) {
							if (this.supportTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().supportScript.id == newCharacterId) {
								this.supportTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().UpdateActionsSlotImages ();
								return;
							}
						}
					}
				}
			}
		} else {
			if (newCharacterType == 3) {
				for (int i = 0; i < L_EnemiesInTeam.Count; i++) {
					if (L_EnemiesInTeam [i].id == newCharacterId) {
						L_EnemiesInTeam [i].L_skills [newSkillIndex] = 0;

						T_Enemy.enemies = L_EnemiesInTeam.ToArray ();

						for (int j = 0; j < this.enemyTeamContainer.transform.childCount; j++) {
							if (this.enemyTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().summonerScript.id == newCharacterId) {
								this.enemyTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().UpdateActionsSlotImages ();
								return;
							}
						}
					}
				}
			}
		}
	}
	#endregion

	#region ChangeTalent
	public void ChangeTalent (int newTeamId, int newCharacterType, int newCharacterId, int newSkillIndex, int newEtatId) {
		if (newTeamId == 1) {
			if (newCharacterType == 1) {

				T_Player.mainSummoner.L_talents [newSkillIndex] = newEtatId;
				this.playerTeamContainer.transform.GetChild (0).GetComponent<CharacterPanelEditor> ().UpdateTalentsSlotImages ();
				return;

				/*for (int i = 0; i < L_Summoners.Count; i++) {
					if (L_Summoners [i].id == newCharacterId) {
						L_Summoners [i].L_talents [newSkillIndex] = newEtatId;
						this.playerTeamContainer.transform.GetChild (0).GetComponent<CharacterPanelEditor> ().UpdateTalentsSlotImages ();
						return;
					}
				}*/

			} else if (newCharacterType == 2) {
				for (int i = 0; i < L_SupportsInTeam.Count; i++) {
					if (L_SupportsInTeam [i].id == newCharacterId) {
						L_SupportsInTeam [i].L_talents [newSkillIndex] = newEtatId;

						T_Player.supports = L_SupportsInTeam.ToArray ();

						for (int j = 0; j < this.supportTeamContainer.transform.childCount; j++) {
							if (this.supportTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().supportScript.id == newCharacterId) {
								this.supportTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().UpdateTalentsSlotImages ();
								return;
							}
						}
					}
				}
			}
		} else {
			if (newCharacterType == 3) {
				for (int i = 0; i < L_EnemiesInTeam.Count; i++) {
					if (L_EnemiesInTeam [i].id == newCharacterId) {
						L_EnemiesInTeam [i].L_talents [newSkillIndex] = newEtatId;

						T_Enemy.enemies = L_EnemiesInTeam.ToArray ();

						for (int j = 0; j < this.enemyTeamContainer.transform.childCount; j++) {
							if (this.enemyTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().summonerScript.id == newCharacterId) {
								this.enemyTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().UpdateTalentsSlotImages ();
								return;
							}
						}
					}
				}
			}
		}
	}

	public void RemoveTalent (int newTeamId, int newCharacterType, int newCharacterId, int newSkillIndex) {
		if (newTeamId == 1) {
			if (newCharacterType == 1) {

				T_Player.mainSummoner.L_talents [newSkillIndex] = 0;
				this.playerTeamContainer.transform.GetChild (0).GetComponent<CharacterPanelEditor> ().UpdateTalentsSlotImages ();
				return;

				/*for (int i = 0; i < L_Summoners.Count; i++) {
					if (L_Summoners [i].id == newCharacterId) {
						L_Summoners [i].L_talents [newSkillIndex] = 0;
						this.playerTeamContainer.transform.GetChild (0).GetComponent<CharacterPanelEditor> ().UpdateTalentsSlotImages ();
						return;
					}
				}*/

			} else if (newCharacterType == 2) {
				for (int i = 0; i < L_SupportsInTeam.Count; i++) {
					if (L_SupportsInTeam [i].id == newCharacterId) {
						L_SupportsInTeam [i].L_talents [newSkillIndex] = 0;

						T_Player.supports = L_SupportsInTeam.ToArray ();

						for (int j = 0; j < this.supportTeamContainer.transform.childCount; j++) {
							if (this.supportTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().supportScript.id == newCharacterId) {
								this.supportTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().UpdateTalentsSlotImages ();
								return;
							}
						}
					}
				}
			}
		} else {
			if (newCharacterType == 3) {
				for (int i = 0; i < L_EnemiesInTeam.Count; i++) {
					if (L_EnemiesInTeam [i].id == newCharacterId) {
						L_EnemiesInTeam [i].L_talents [newSkillIndex] = 0;

						T_Enemy.enemies = L_EnemiesInTeam.ToArray ();

						for (int j = 0; j < this.enemyTeamContainer.transform.childCount; j++) {
							if (this.enemyTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().summonerScript.id == newCharacterId) {
								this.enemyTeamContainer.transform.GetChild (j).GetComponent<CharacterPanelEditor> ().UpdateTalentsSlotImages ();
								return;
							}
						}
					}
				}
			}
		}
	}
	#endregion
}
