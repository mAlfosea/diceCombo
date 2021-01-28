using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour {

	public static PlayerTeam playerTeam;
	public static EnemyTeam enemyTeam;

	public static List<GameObject> L_Summoners = new List<GameObject> ();
	public static List<GameObject> L_Enemies = new List<GameObject> ();
	public static List<GameObject> L_EnemiesToAct = new List<GameObject> ();

	public GameObject summonerPrefab;
	public GameObject enemyPrefab;
	public GameObject supportPrefab;
	public GameObject characterInfosPrefab;
	public GameObject supportInfosPrefab;

	public GameObject changeTurnButton;
	private bool changeTurnButtonState = true;

	public delegate void SendChangeTurn(int newTurnState);
	public static event SendChangeTurn E_ChangeTurn;
	public delegate void CombatState(int newTextId);
	public static event CombatState E_ShowCombatText;

	private Coroutine turnCoroutine;
	public static int playerTurn; // 0 > enemy - 1 > player

	private bool combatEnded;
	private bool isLost;

	void OnEnable()
	{
		Summoner.TurnEnded += this.EnemyHasPlayed;
		Summoner.ToggleChangeTurnButton += this.ToggleChangeTurnButton;
		Summoner.CharacterDeath += this.CheckEndGame;
	}


	void OnDisable()
	{
		Summoner.TurnEnded -= this.EnemyHasPlayed;
		Summoner.ToggleChangeTurnButton -= this.ToggleChangeTurnButton;
		Summoner.CharacterDeath -= this.CheckEndGame;
	}

	// Use this for initialization
	void Start () {
		if (DictionaryManager.isActionLoaded == false) {
			DictionaryManager.LoadActions ();
		}

		if (DictionaryManager.isEtatsLoaded == false) {
			DictionaryManager.LoadEtats ();
		}

		StartCoroutine (this.GetComponent<UiManager> ().HideBlackScreen ());
		this.GetComponent<UiManager> ().UpdateInfos ();
		this.GenerateCombat ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	#region Start / End Combat
	public void GenerateCombat () {
		playerTurn = 0;
		changeTurnButtonState = true;
		L_Summoners.Clear ();
		L_Enemies.Clear ();
		L_EnemiesToAct.Clear ();

		this.changeTurnButton.SetActive (false);

		this.combatEnded = false;
		this.LoadFight ();
		this.LoadMonsters ();
		this.StartCombat ();
	}

	public void EndCombat () {

		if (CampaignManager.isCampaignGenerated == true) {
			playerTeam.mainSummoner.pv = L_Summoners [0].GetComponent<Summoner> ().pv;
			playerTeam.mainSummoner.L_etats.Clear ();
			for (int i = 0; i < playerTeam.supports.Length; i++) {
				playerTeam.supports [i].L_etats.Clear ();
			}
			CampaignManager.combatEnded = true;

			if (CampaignManager.isBossFight || this.isLost) {
				CampaignManager.isCampaignGenerated = false;
				JsonSave.SaveCampaign ();
				JsonSave.SavePlayerTeam (playerTeam);
				StartCoroutine (this.GetComponent<UiManager> ().ShowBlackScreen ());
				Invoke ("LoadMainMenuScene", 0.5f);
			} else {
				JsonSave.SaveCampaign ();
				JsonSave.SavePlayerTeam (playerTeam);
				StartCoroutine (this.GetComponent<UiManager> ().ShowBlackScreen ());
				Invoke ("LoadCampaignScene", 0.5f);
			}
		} else {
			playerTurn = 0;
			StopCoroutine (this.turnCoroutine);
			this.RemoveCharacters ();
			GameObject.Find ("_HAND").GetComponent<CardsInHandPanel> ().EndCombat ();
			this.changeTurnButtonState = true;
			this.changeTurnButton.SetActive (false);

			Invoke ("LoadEditorScene", 0.5f);
		}
	}

	public void LoadCampaignScene () {
		SceneManager.LoadScene ("campaign", LoadSceneMode.Single);		
	}
	public void LoadMainMenuScene () {
		SceneManager.LoadScene ("mainMenu", LoadSceneMode.Single);		
	}
	public void LoadEditorScene () {
		SceneManager.LoadScene ("editor", LoadSceneMode.Single);		
	}
	public void Abandonner () {
		for (int i = 0; i < L_Summoners.Count; i++) {
			L_Summoners [i].GetComponent<Summoner> ().Death();
		}
	}

	private void CheckEndGame () {
		StartCoroutine (this.I_CheckEndGame ());
	}

	private IEnumerator I_CheckEndGame () {
		bool allPlayersDead = true;
		bool allEnemiesDead = true;
		for (int i = 0; i < L_Summoners.Count; i++) {
			if (L_Summoners [i].GetComponent<Summoner> ().isKo == false) {
				allPlayersDead = false;
			} else { // le personnage est dead c'est la fin de la partie
				this.isLost = true;
			}
		}
		if (allPlayersDead) {
			this.combatEnded = true;
			this.changeTurnButton.SetActive (true);
			this.ToggleChangeTurnButton ();

			for (int i = 0; i < L_Enemies.Count; i++) {
				L_Enemies [i].GetComponent<Summoner> ().StopFight ();
			}
			for (int i = 0; i < L_Summoners.Count; i++) {
				L_Summoners [i].GetComponent<Summoner> ().StopFight ();
			}

			yield return new WaitForSeconds (0.5f);

			if (E_ShowCombatText != null) {
				E_ShowCombatText (3);
			}
			if (E_ChangeTurn != null) {
				E_ChangeTurn (3); // code express pour drop la hand sans l'affichage du changeturn
			}
				
			yield return new WaitForSeconds (3f);
			this.EndCombat ();
			//Invoke ("EndCombat", 3f);
			yield return null;
		}

		for (int i = 0; i < L_Enemies.Count; i++) {
			if (L_Enemies [i].GetComponent<Summoner> ().isKo == false) {
				allEnemiesDead = false;
			}
		}
		if (allEnemiesDead) {
			this.combatEnded = true;
			this.changeTurnButton.SetActive (true);
			this.ToggleChangeTurnButton ();

			for (int i = 0; i < L_Enemies.Count; i++) {
				L_Enemies [i].GetComponent<Summoner> ().StopFight ();
			}
			for (int i = 0; i < L_Summoners.Count; i++) {
				L_Summoners [i].GetComponent<Summoner> ().StopFight ();
			}

			yield return new WaitForSeconds (0.5f);

			if (E_ShowCombatText != null) {
				E_ShowCombatText (2);
			}
			if (E_ChangeTurn != null) {
				E_ChangeTurn (3); // code expres pour drop la hand sans l'affichage du changeturn
			}

			yield return new WaitForSeconds (3f);
			this.EndCombat ();
			//Invoke ("EndCombat", 3f);
		}
	}
	#endregion

	public void StartCombat () {
		this.changeTurnButton.SetActive (true);
		this.ToggleChangeTurnButton ();
		turnCoroutine = StartCoroutine (this.ChangeTurn());
	}

	public void EndTurn () {
		turnCoroutine = StartCoroutine (this.ChangeTurn());
	}

	public void ToggleChangeTurnButton () {
		if (this.changeTurnButtonState) {
			this.changeTurnButtonState = false;
		} else {
			this.changeTurnButtonState = true;
		}
		this.changeTurnButton.GetComponent<Button>().interactable = this.changeTurnButtonState;
	}

	#region Gestion des Tours
	public void StartTurnPlayer () {
		for (int i = 0; i < L_Summoners.Count; i++) {
			if (L_Summoners [i].GetComponent<Summoner> ().isKo == false) {
				L_Summoners [i].GetComponent<Summoner> ().StartTurn ();
			}
		}
		for (int i = 0; i < L_Enemies.Count; i++) {
			if (L_Enemies [i].GetComponent<Summoner> ().isKo == false) {
				L_Enemies [i].GetComponent<Summoner> ().LaunchDiceAnimation ();
			}
		}
	}
	public void StartTurnEnemi () {
		this.CheckEnemyToAct ();
	}

	public IEnumerator ChangeTurn () {
		if (playerTurn == 0) { // c'est le tour de l'enemy donc j'appelle les players
			if (E_ChangeTurn != null) { //sert pour afficher le texte de changement de turn
				E_ChangeTurn (playerTurn);
			}
			yield return new WaitForSeconds (3f);
			this.StartTurnPlayer (); 
			playerTurn = 1;
		} else {
			this.ToggleChangeTurnButton ();
			bool isOkayToChangeTurn = false;

			do {
				isOkayToChangeTurn = true;
				for (int i = 0; i < L_Summoners.Count; i++) {
					print (L_Summoners.Count);
					if (L_Summoners [i].GetComponent<Summoner> ().L_CardsToApply.Count > 0) {
						isOkayToChangeTurn = false;
						yield return null;
					}
				}
			} while (isOkayToChangeTurn == false); 
				
			for (int j = 0; j < L_Summoners.Count; j++) {
				if (L_Summoners [j].GetComponent<Summoner> ().isKo == false) {
					L_Summoners [j].GetComponent<Summoner> ().TurnTerminated ();
				}
			}

			if (this.combatEnded == false) {
				if (E_ChangeTurn != null) {
					E_ChangeTurn (playerTurn);
				}
				yield return new WaitForSeconds (3f);
				playerTurn = 0;
				this.AddEnemiesToAct ();
				this.StartTurnEnemi (); 
			}
		}
	}
	#endregion

	#region Gestion des attaques enemies
	public void CheckEnemyToAct () {
		if (L_EnemiesToAct.Count > 0) {
			if (L_EnemiesToAct [0].GetComponent<Summoner> ().isKo) {
				this.EnemyHasPlayed ();
			} else {
				L_EnemiesToAct [0].GetComponent<Summoner> ().StartTurn ();
			}
		} else {
			if (this.combatEnded == false) { 
				this.EndTurn ();
			}
		}
	}

	public void AddEnemiesToAct () {
		for (int i = 0; i < L_Enemies.Count; i++) {
			L_EnemiesToAct.Add (L_Enemies [i]);
		}
	}

	public void EnemyHasPlayed () {
		L_EnemiesToAct.RemoveAt (0);
		this.CheckEnemyToAct ();
	}
	#endregion

	#region Save / Load Summoners
	public void LoadFight () {
		PlayerTeam[] playerTeamTemp = JsonSave.LoadPlayerTeam ();
		playerTeam = playerTeamTemp [0];

		this.LoadSummoners (playerTeamTemp[0].mainSummoner);
	}
	public void LoadSummoners (SummonerData newSummoner)
	{
		Transform parentPos = GameObject.Find ("_characters_pos").transform.GetChild(0);

		int nbSummoner = 0;

		GameObject summonerGO = Instantiate (this.summonerPrefab, parentPos.transform.GetChild (nbSummoner).transform.position, this.transform.rotation, parentPos) as GameObject;
		GameObject characterInfosGO = Instantiate (this.characterInfosPrefab, this.transform.position, this.transform.rotation) as GameObject;
		Summoner summonerScript = summonerGO.GetComponent<Summoner> ();
		CharacterInfosPanel characterInfosScript = characterInfosGO.GetComponent<CharacterInfosPanel> ();

		this.CreateCharacter (summonerScript, newSummoner);
		summonerGO.name = newSummoner.summonerName;
		summonerScript.avatarImg.sprite = Resources.Load<Sprite> ("character_avatars/" + summonerScript.idAvatar);
		summonerScript.charactersInfosPanel = characterInfosGO;
		summonerScript.Init ();
		characterInfosScript.Init (summonerGO, 1);

		L_Summoners.Add (summonerGO);
		this.LoadSupports (playerTeam.supports, summonerGO);
		nbSummoner += 1;
	}

	public void LoadSupports (SupportData[] newSupportList, GameObject newSummoner) {
		Summoner summonerScriptTemp = newSummoner.GetComponent<Summoner> ();
		Transform parentPos = GameObject.Find ("_characters_pos").transform.GetChild(1);

		int nbSupport = 0;

		for (int i = 0; i < newSupportList.Length; i++) {
			GameObject supportGO = Instantiate (this.supportPrefab, parentPos.transform.GetChild(nbSupport).transform.position, this.transform.rotation, parentPos) as GameObject;
			GameObject characterInfosGO = Instantiate (this.supportInfosPrefab, this.transform.position, this.transform.rotation) as GameObject;
			Support supportScript = supportGO.GetComponent<Support> ();
			CharacterInfosPanel characterInfosScript = characterInfosGO.GetComponent<CharacterInfosPanel> ();

			supportGO.name = newSupportList [i].supportName;
			supportScript.id = newSupportList [i].id;
			supportScript.idOriginal = newSupportList [i].idOriginal;
			supportScript.supportName = newSupportList [i].supportName;
			supportScript.idAvatar = newSupportList [i].idAvatar;
			supportScript.avatarImg.sprite = Resources.Load<Sprite> ("character_avatars/" + supportScript.idAvatar);
			supportScript.characterType = newSupportList [i].characterType;
			supportScript.tier = newSupportList [i].tier;

			supportScript.nbDiceMax = newSupportList [i].nbDiceMax;
			supportScript.nbDice = newSupportList [i].nbDice;

			supportScript.nbSkillSlots = newSupportList [i].nbSkillSlots;
			supportScript.L_skills = newSupportList [i].L_skills;
			supportScript.L_talents = newSupportList [i].L_talents;
			supportScript.L_etats = newSupportList [i].L_etats;

			supportScript.charactersInfosPanel = characterInfosGO;

			supportScript.Init ();
			characterInfosScript.Init (supportGO, 2);

			summonerScriptTemp.L_supports.Add (supportGO);
			nbSupport += 1;
		}
	}

	public void LoadMonsters ()
	{
		EnemyTeam[] enemyTeamTemp = JsonSave.LoadEnemyTeam ();
		enemyTeam = enemyTeamTemp [0];

		Transform parentPos = GameObject.Find ("_characters_pos").transform.GetChild(2);

		int nbMonsters = 0;

		for (int i = 0; i < enemyTeam.enemies.Length; i++) {
			GameObject summonerGO = Instantiate (this.enemyPrefab, parentPos.transform.GetChild(nbMonsters).transform.position, this.transform.rotation, parentPos) as GameObject;
			GameObject characterInfosGO = Instantiate (this.characterInfosPrefab, this.transform.position, this.transform.rotation) as GameObject;
			Summoner summonerScript = summonerGO.GetComponent<Summoner> ();
			CharacterInfosPanel characterInfosScript = characterInfosGO.GetComponent<CharacterInfosPanel> ();

			summonerGO.transform.localScale = new Vector3 (-summonerGO.transform.localScale.x, summonerGO.transform.localScale.y, summonerGO.transform.localScale.z);

			this.CreateCharacter (summonerScript, enemyTeam.enemies[i]);
			summonerGO.name = enemyTeam.enemies [i].summonerName;
			summonerScript.charactersInfosPanel = characterInfosGO;
			summonerScript.cardTarget = L_Summoners [0];
			summonerScript.Init ();
			characterInfosScript.Init (summonerGO, 3);

			L_Enemies.Add (summonerGO);
			nbMonsters += 1;
		}
	}

	private void CreateCharacter (Summoner newSummoner, SummonerData summonerJson)
	{
		newSummoner.id = summonerJson.id;
		newSummoner.idOriginal = summonerJson.idOriginal;
		newSummoner.tier = summonerJson.tier;
		newSummoner.nbDiceMax = summonerJson.nbDiceMax;
		newSummoner.nbDice = summonerJson.nbDice;
		newSummoner.nbSkillSlots = summonerJson.nbSkillSlots;
		newSummoner.nbItemSlots = summonerJson.nbItemSlots;
		newSummoner.summonerName = summonerJson.summonerName;
		newSummoner.idAvatar = summonerJson.idAvatar;
		newSummoner.pvMax = summonerJson.pvMax;
		newSummoner.pv = summonerJson.pv;
		newSummoner.force = summonerJson.force;
		newSummoner.armure = summonerJson.armure;
		newSummoner.characterType = summonerJson.characterType;
		newSummoner.L_skills = summonerJson.L_skills;		
		newSummoner.L_talents = summonerJson.L_talents;	
		newSummoner.L_etats = summonerJson.L_etats;		
	}

	private void RemoveCharacters () {
		for (int i = 0; i < L_Summoners.Count; i++) {
			L_Summoners [i].GetComponent<Summoner> ().EndCombat ();
			Destroy (L_Summoners [i]);
		}
		L_Summoners.Clear ();

		for (int i = 0; i < L_Enemies.Count; i++) {
			L_Enemies [i].GetComponent<Summoner> ().EndCombat ();
			Destroy (L_Enemies [i]);
		}
		L_Enemies.Clear ();
		L_EnemiesToAct.Clear ();
	}
	#endregion
}
