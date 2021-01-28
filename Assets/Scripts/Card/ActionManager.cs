using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour {

	public Summoner summonerScript;
	public Support supportScript;

	public GameObject animationManagerPrefab;
	public bool animationIsPlaying;
	public Coroutine cardApplicationCoroutine;

	private GameObject cardLauncher;
	private GameObject cardTarget;
	private Summoner cardLauncherScript;
	private Summoner cardTargetScript;
	private Card cardToApply;
	private Card lastCardPlayed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region Load Talents
	public void LoadTalents () {
		if (this.summonerScript != null) {
			for (int i = 0; i < this.summonerScript.L_talents.Count; i++) {
				if (this.summonerScript.L_talents [i] != 0) {
					EtatsData etatTemp = DictionaryManager.GetEtat (this.summonerScript.L_talents [i]);
					print (this.summonerScript.summonerName + " : Load l'etat : " + etatTemp.name);

					CombatEtatData combatEtat = new CombatEtatData ();
					combatEtat.idEtat = etatTemp.id;
					combatEtat.target = 1; 
					combatEtat.modulateur = "+";
					combatEtat.duration = 0;
					combatEtat.value = 0;

					if (etatTemp.id == 17 || etatTemp.id == 22 || etatTemp.id == 18) {
						combatEtat.value += 1;
					}

					this.AddEtat (etatTemp, combatEtat);
				}
			}
		} else {
			for (int i = 0; i < this.supportScript.L_talents.Count; i++) {
				if (this.supportScript.L_talents [i] != 0) {
					EtatsData etatTemp = DictionaryManager.GetEtat (this.supportScript.L_talents [i]);
					print (this.supportScript.supportName + " : Load l'etat : " + etatTemp.name);

					CombatEtatData combatEtat = new CombatEtatData ();
					combatEtat.idEtat = etatTemp.id;
					combatEtat.target = 1; 
					combatEtat.modulateur = "+";
					combatEtat.duration = 0;
					combatEtat.value = 0;

					if (etatTemp.id == 17 || etatTemp.id == 22 || etatTemp.id == 18) {
						combatEtat.value += 1;
					}

					this.AddEtat (etatTemp, combatEtat);
				}
			}
		}
	}

	public void LoadStats () {
		if (this.summonerScript.characterType == 1) {
			if (CampaignManager.playerTeam.mainSummoner.force > 0) {
				EtatsData etatTemp = DictionaryManager.GetEtat (23);
				etatTemp.value = CampaignManager.playerTeam.mainSummoner.force;

				CombatEtatData combatEtat = new CombatEtatData ();
				combatEtat.idEtat = etatTemp.id;
				combatEtat.target = 1; 
				combatEtat.modulateur = "+";
				combatEtat.duration = 0;
				combatEtat.value = 0;
				this.AddEtat (etatTemp, combatEtat);
			}

			if (CampaignManager.playerTeam.mainSummoner.armure > 0) {
				EtatsData etatTemp = DictionaryManager.GetEtat (24);
				etatTemp.value = CampaignManager.playerTeam.mainSummoner.armure;

				CombatEtatData combatEtat = new CombatEtatData ();
				combatEtat.idEtat = etatTemp.id;
				combatEtat.target = 1; 
				combatEtat.modulateur = "+";
				combatEtat.duration = 0;
				combatEtat.value = 0;
				this.AddEtat (etatTemp, combatEtat);
			}
		} else if (this.summonerScript.characterType == 3) {
			if (this.summonerScript.force > 0) {
				EtatsData etatTemp = DictionaryManager.GetEtat (23);
				etatTemp.value = this.summonerScript.force;

				CombatEtatData combatEtat = new CombatEtatData ();
				combatEtat.idEtat = etatTemp.id;
				combatEtat.target = 1; 
				combatEtat.modulateur = "+";
				combatEtat.duration = 0;
				combatEtat.value = 0;
				this.AddEtat (etatTemp, combatEtat);
			}

			if (this.summonerScript.armure > 0) {
				EtatsData etatTemp = DictionaryManager.GetEtat (24);
				etatTemp.value = this.summonerScript.armure;

				CombatEtatData combatEtat = new CombatEtatData ();
				combatEtat.idEtat = etatTemp.id;
				combatEtat.target = 1; 
				combatEtat.modulateur = "+";
				combatEtat.duration = 0;
				combatEtat.value = 0;
				this.AddEtat (etatTemp, combatEtat);
			}
		}
	}
	#endregion

	#region Apply Card & Degats
	public void ApplyCard () {
		this.cardLauncher = this.summonerScript.L_CardsToApply [0].launcher;
		this.cardTarget = this.summonerScript.L_CardsToApply [0].target;
		this.cardLauncherScript = this.cardLauncher.GetComponent<Summoner> ();
		this.cardTargetScript = this.cardTarget.GetComponent<Summoner> ();
		this.cardToApply = this.summonerScript.L_CardsToApply [0].card;
		this.lastCardPlayed = this.summonerScript.L_CardsToApply [0].previousCard;

		if (cardTargetScript != null && this.lastCardPlayed != null) {
		}
		this.cardApplicationCoroutine = StartCoroutine (this.ApplyClassicAction ());
	}

	public void ResetCardVariable () {
		this.cardLauncher = null;
		this.cardTarget = null;
		this.cardLauncherScript = null;
		this.cardTargetScript = null;
		this.cardToApply = null;
		this.lastCardPlayed = null;
	}
	#endregion

	#region Apply Action
	public IEnumerator ApplyClassicAction () {

		if (this.cardLauncherScript.characterType == 3) {
			this.cardLauncherScript.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().EndBumpDice (this.cardToApply.diceId);
		}

		this.cardLauncherScript.CallDroppedCardEvent (this.cardToApply, true); // je dis a la carte d'aller a la bonne position

		yield return new WaitForSeconds (0.5f);
		//print (this.summonerName + ": je lance lattaque sur: " + targetTemp + " / " + cardTemp.uId);

		for (int i = 0; i < this.cardToApply.tempAction.combatActions.Length; i++) {
			if (this.IsConditionOk (this.cardToApply.tempAction.combatActions [i])) {
				CombatActionData combatAction = this.cardToApply.tempAction.combatActions [i];
				GameObject actionAnimation = Instantiate (this.animationManagerPrefab, this.transform.position, this.transform.rotation, this.transform) as GameObject;
				AnimationManager actionAnimationScript = actionAnimation.GetComponent<AnimationManager> ();

				if (combatAction.target == 1) { // je me cible moi
					
					this.animationIsPlaying = true;
					actionAnimationScript.GenerateAnimation (this.cardLauncher, this.cardLauncher, this.cardToApply.tempAction.combatActions [i].idAnim);

					while (this.animationIsPlaying) {
						yield return null;
					}

					this.ApplyCombatAction (combatAction, this.cardLauncher);

				} else if (combatAction.target == 2) { // je cible un ennemi
					
					this.animationIsPlaying = true;
					actionAnimationScript.GenerateAnimation (this.cardLauncher, this.cardTarget, this.cardToApply.tempAction.combatActions [i].idAnim);

					while (this.animationIsPlaying) {
						yield return null;
					}

					this.cardTarget.GetComponent<ActionManager> ().ApplyCombatAction (combatAction, this.cardLauncher);
					this.CheckEtatsOnAction (combatAction, this.cardLauncher, this.cardToApply.tempAction.combatActions [i].damage, 2);

				} else { // je cible tous les enemies
					this.animationIsPlaying = true;
					
					for (int j = 0; j < CombatManager.L_Enemies.Count; j++) {

						GameObject enemyTemp = CombatManager.L_Enemies [j];

						if (enemyTemp.GetComponent<Summoner> ().isKo == false) {
							
							actionAnimationScript.GenerateAnimation (this.cardLauncher, enemyTemp, this.cardToApply.tempAction.combatActions [i].idAnim);

							while (this.animationIsPlaying) {
								yield return null;
							}

							enemyTemp.GetComponent<ActionManager> ().ApplyCombatAction (combatAction, this.cardLauncher);
						}
					}
				}

				if (combatAction.etats.Length != 0) {
					for (int j = 0; j < combatAction.etats.Length; j++) {
						CombatEtatData combatEtatTemp = combatAction.etats [j];
						EtatsData etatTemp = DictionaryManager.GetEtat (combatEtatTemp.idEtat);

						if (etatTemp.id == 14 || etatTemp.id == 16 || etatTemp.id == 21) {
							if (combatEtatTemp.target == 1) { 
								this.AddSpecialEtat (this.cardToApply, etatTemp, combatEtatTemp);
							} else if (combatEtatTemp.target == 2) {
								this.cardTarget.GetComponent<ActionManager>().AddSpecialEtat (this.cardToApply, etatTemp, combatEtatTemp);
							} else if (combatEtatTemp.target == 3) {
								// je l'applique a tous
							}
						} else {
							if (combatEtatTemp.target == 1) { 
								etatTemp.turnStateApply = 1;
								this.AddEtat (etatTemp, combatEtatTemp);
							} else if (combatEtatTemp.target == 2) {
								etatTemp.turnStateApply = 2;
								this.cardTarget.GetComponent<ActionManager>().AddEtat (etatTemp, combatEtatTemp);
							} else if (combatEtatTemp.target == 3) {
								// je l'applique a tous
							}
						}
					}
				}
				yield return new WaitForSeconds (0.3f);	
			}
		}


		if (this.cardLauncherScript.characterType == 1) { //si c'est un player j'update les cartes, si c'est un enemy on s'en fout

			this.cardLauncherScript.RemoveCardFromList (this.cardToApply);
			this.cardLauncherScript.L_CardsToApply.RemoveAt (0);
			//print ("je apply lattaque: " + newCard.tempAction.name + " / il me reste : " + this.L_Cards.Count);

			this.cardLauncherScript.CardPlayedEvent (this.cardToApply);

			this.cardLauncherScript.cardIsPlaying = false;
			this.ResetCardVariable ();

			this.summonerScript.CheckCardsToApplyList ();
		} else {

			this.cardLauncherScript.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().HideDice (this.cardToApply.diceId);

			yield return new WaitForSeconds (0.5f); // je mets en un temps de pause avant de lancer la next expres pour les monstres

			this.cardLauncherScript.RemoveCardFromList (this.cardToApply);
			this.cardLauncherScript.L_CardsToApply.RemoveAt (0);
			this.cardLauncherScript.cardIsPlaying = false;
			this.ResetCardVariable ();

			this.summonerScript.CheckCardsToApplyList ();
		}
	}
	#endregion

	#region Check Condition
	public bool IsConditionOk (CombatActionData newCombatAction) {
		Summoner targetTemp;

		if (newCombatAction.conditions.Length == 0) { // si je ne specifie pas de conditions je peux la lancer
			return true;
		}

		if (newCombatAction.conditions [0].target == 1) {
			targetTemp = this.cardLauncher.GetComponent<Summoner> ();
		} else if (newCombatAction.conditions [0].target == 2) {
			targetTemp = this.cardTarget.GetComponent<Summoner> ();
		} else {
			targetTemp = this.cardLauncher.GetComponent<Summoner> ();
		}

		if (newCombatAction.conditions [0].type == 2) { // je check l'armure
			if (newCombatAction.conditions [0].mod == 1) { // si c'est inférieur
				if (targetTemp.defense < newCombatAction.conditions [0].value) { 
					return true;
				} else {
					return false;
				}
			} else if (newCombatAction.conditions [0].mod == 2) { // si c'est égal
				if (targetTemp.defense == newCombatAction.conditions [0].value) {
					return true;
				} else {
					return false;
				}
			} else {
				if (targetTemp.defense > newCombatAction.conditions [0].value) { // si c'est supérieur
					return true;
				} else {
					return false;
				}
			}
		} else if (newCombatAction.conditions [0].type == 3) { // je check la previous card
			if (newCombatAction.conditions [0].mod == 2) { // je check le type de la previous card
				if (this.lastCardPlayed.tempAction.type == newCombatAction.conditions [0].value) { 
					return true;
				} else {
					return false;
				}
			}
		}
		return false;
	}
	#endregion

	#region Apply Combat Action
	public void ApplyCombatAction (CombatActionData newCombatAction, GameObject newLauncher) {
		if (newCombatAction.typeDmg == 1) { // c'est une attaque
			if (newCombatAction.modulateur == "+") {
				this.summonerScript.pv += newCombatAction.damage;
				this.CheckPv ();
				//print (this.summonerName + " : je me suis soigné de : " + newCombatAction.damage + " pv");
			} else {
				int damage = this.UpdateDefense (newCombatAction);
				this.summonerScript.pv -= damage;
				this.CheckPv ();

				this.CheckEtatsOnAction (newCombatAction, newLauncher, damage, 1);
				//print (this.summonerName + " : je me suis mangé : " + damage + " degats");
			}
		} else if (newCombatAction.typeDmg == 2)  {
			if (newCombatAction.modulateur == "+") { // c'est une défense
				this.summonerScript.defense += newCombatAction.damage;
				//print (this.summonerName + " : jai gagne : " + newCombatAction.damage + " defense");
			} else {
				this.summonerScript.defense -= newCombatAction.damage;
				//print (this.summonerName + " : jai perdu : " + newCombatAction.damage + " defense");
			}
		}

		this.summonerScript.CallUpdateUIInfos ();
	}

	private int UpdateDefense (CombatActionData newCombatAction) {
		if (this.summonerScript.defense >= newCombatAction.damage) {
			int defenseTemp = this.summonerScript.defense;
			defenseTemp -= newCombatAction.damage;
			this.summonerScript.defense = defenseTemp;
			if (this.summonerScript.defense > 0) {
				this.summonerScript.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().BumpDefense ();
			}
			return 0;
		} else {
			int damageTemp = newCombatAction.damage;
			damageTemp -= this.summonerScript.defense;
			this.summonerScript.defense = 0;
			this.GetDamageBegin ();
			this.summonerScript.charactersInfosPanel.GetComponent<CombatEffectsText> ().GenerateDamageText (1, damageTemp);
			return damageTemp;
		}
	}

	private void CheckPv () {
		if (this.summonerScript.pv > this.summonerScript.pvMax) {
			this.summonerScript.pv = this.summonerScript.pvMax;
		}
		if (this.summonerScript.pv <= 0) {
			this.summonerScript.Death();
		}
	}
	#endregion

	#region Add Etat
	public void AddEtat (EtatsData newEtat, CombatEtatData newCombatEtat) {
		if (this.summonerScript != null) {
			for (int i = 0; i < this.summonerScript.L_etats.Count; i++) {
				if (this.summonerScript.L_etats [i].id == newEtat.id) {
					EtatsData etatTemp = this.summonerScript.L_etats [i];

					if (newCombatEtat.modulateur == "+") {
						etatTemp.duration += newCombatEtat.duration;
						etatTemp.value += newCombatEtat.value;
					} else if (newCombatEtat.modulateur == "-") {
						etatTemp.duration += newCombatEtat.duration;
						etatTemp.value -= newCombatEtat.value;
					}

					this.summonerScript.L_etats [i] = etatTemp;

					if (etatTemp.id != 13) {
						this.UpdateCards (etatTemp, newCombatEtat, 2); // 2 pour dire au panel card que je dois update avec les valeur du combatEtat
					}

					this.summonerScript.CallUIAddEtat (etatTemp, 2);
					return;
				}
			}
			EtatsData etatToAdd = newEtat;

			if (newCombatEtat.modulateur == "+") {
				etatToAdd.duration += newCombatEtat.duration;
				etatToAdd.value += newCombatEtat.value;
			} else if (newCombatEtat.modulateur == "-") {
				etatToAdd.duration += newCombatEtat.duration;
				etatToAdd.value -= newCombatEtat.value;
			}

			this.summonerScript.L_etats.Add (etatToAdd);
			this.UpdateCards (etatToAdd, newCombatEtat, 1); // 1 pour dire au panel card que je dois ajouter l'etat entier
			this.summonerScript.CallUIAddEtat (etatToAdd, 1);
		} else {
			for (int i = 0; i < this.supportScript.L_etats.Count; i++) {
				if (this.supportScript.L_etats [i].id == newEtat.id) {
					EtatsData etatTemp = this.supportScript.L_etats [i];

					if (newCombatEtat.modulateur == "+") {
						etatTemp.duration += newCombatEtat.duration;
						etatTemp.value += newCombatEtat.value;
					} else if (newCombatEtat.modulateur == "-") {
						etatTemp.duration += newCombatEtat.duration;
						etatTemp.value -= newCombatEtat.value;
					}

					this.supportScript.L_etats [i] = etatTemp;
					//this.UpdateCards (etatTemp, newCombatEtat, 2); // 2 pour dire au panel card que je dois update avec les valeur du combatEtat
					this.supportScript.CallUIAddEtat (etatTemp, 2);
					return;
				}
			}
			EtatsData etatToAdd = newEtat;

			if (newCombatEtat.modulateur == "+") {
				etatToAdd.duration += newCombatEtat.duration;
				etatToAdd.value += newCombatEtat.value;
			} else if (newCombatEtat.modulateur == "-") {
				etatToAdd.duration += newCombatEtat.duration;
				etatToAdd.value -= newCombatEtat.value;
			}

			this.supportScript.L_etats.Add (etatToAdd);
			//this.UpdateCards (etatToAdd, newCombatEtat, 1); // 1 pour dire au panel card que je dois ajouter l'etat entier
			this.supportScript.CallUIAddEtat (etatToAdd, 1);
		}
	}

	public void AddSpecialEtat (Card newCard, EtatsData newEtat, CombatEtatData newCombatEtat) {
		if (newEtat.id == 14) { // relance
			this.summonerScript.LaunchSpecificDice (newCard);
		} else if (newEtat.id == 16) { // defausse
			if (this.summonerScript.L_Cards.Count > 1) {
				int randomCardId;
				do {
					randomCardId = Random.Range (0, this.summonerScript.L_Cards.Count);
				} while (this.summonerScript.L_Cards [randomCardId].tempAction.id == newCard.tempAction.id);
				Card cardToBan = this.summonerScript.L_Cards [randomCardId];
				//print ("la carte a virer est la " + randomCardId + " / " + this.summonerScript.L_Cards [randomCardId].tempAction.name);
				this.summonerScript.RemoveCardFromList (cardToBan);
				this.summonerScript.CardBanEvent (cardToBan);

				for (int i = 0; i < this.summonerScript.L_CardsToApply.Count; i++) {
					if (this.summonerScript.L_CardsToApply [i].card.tempAction.id == cardToBan.tempAction.id) {
						this.cardLauncherScript.L_CardsToApply.RemoveAt (i);
					}
				}
			}
		} else if (newEtat.id == 18) { // c'est le permanent armure
			CombatActionData combatActionTemp = new CombatActionData();
			combatActionTemp.colorDmg = 0;
			combatActionTemp.damage = newEtat.value;
			combatActionTemp.idAnim = "2";
			combatActionTemp.modulateur = "+";
			combatActionTemp.target = 1;
			combatActionTemp.typeDmg = 2;

			GameObject actionAnimation = Instantiate (this.animationManagerPrefab, this.transform.position, this.transform.rotation, this.transform) as GameObject;
			AnimationManager actionAnimationScript = actionAnimation.GetComponent<AnimationManager> ();
			actionAnimationScript.GenerateAnimation (this.gameObject, this.gameObject, combatActionTemp.idAnim);

			this.ApplyCombatAction(combatActionTemp, this.gameObject);
		} else if (newEtat.id == 19) { // c'est le permanent déflagration
			CombatActionData combatActionTemp = new CombatActionData();
			combatActionTemp.colorDmg = 4;
			combatActionTemp.damage = 1;
			combatActionTemp.idAnim = "1";
			combatActionTemp.modulateur = "-";
			combatActionTemp.target = 1;
			combatActionTemp.typeDmg = 1;
			this.ApplyCombatAction(combatActionTemp, this.gameObject);

			GameObject actionAnimation1 = Instantiate (this.animationManagerPrefab, this.transform.position, this.transform.rotation, this.transform) as GameObject;
			AnimationManager actionAnimationScript1 = actionAnimation1.GetComponent<AnimationManager> ();
			actionAnimationScript1.GenerateAnimation (this.gameObject, this.gameObject, combatActionTemp.idAnim);

			CombatActionData combatActionTemp2 = new CombatActionData();
			combatActionTemp2.colorDmg = 4;
			combatActionTemp2.damage = newEtat.value;
			combatActionTemp2.idAnim = "1";
			combatActionTemp2.modulateur = "-";
			combatActionTemp2.target = 3;
			combatActionTemp2.typeDmg = 1;

			for (int j = 0; j < CombatManager.L_Enemies.Count; j++) {
				GameObject enemyTemp = CombatManager.L_Enemies [j];
				if (enemyTemp.GetComponent<Summoner> ().isKo == false) {
					GameObject actionAnimation = Instantiate (this.animationManagerPrefab, this.transform.position, this.transform.rotation, this.transform) as GameObject;
					AnimationManager actionAnimationScript = actionAnimation.GetComponent<AnimationManager> ();
					actionAnimationScript.GenerateAnimation (this.gameObject, enemyTemp, combatActionTemp2.idAnim);
					enemyTemp.GetComponent<ActionManager> ().ApplyCombatAction (combatActionTemp2, this.gameObject);
				}
			}
		} else if (newEtat.id == 21) { // c'est l'entrave

			int randomDice;
			bool isAlreadyOnEntrave = false;

			List<Dice> diceList = new List<Dice> ();

			for (int i = 0; i < this.summonerScript.L_dices.Count; i++) {
				if (this.summonerScript.L_dices [i].isBlocked == false) {
					diceList.Add (this.summonerScript.L_dices [i]);
				} else {
					isAlreadyOnEntrave = true;
				}
			}
			for (int i = 0; i < this.summonerScript.L_supports.Count; i++) {
				Support supportScript = this.summonerScript.L_supports [i].GetComponent<Support> ();
				for (int j = 0; j < supportScript.L_dices.Count; j++) {
					if (supportScript.L_dices [j].isBlocked == false) {
						diceList.Add (supportScript.L_dices [j]);
					} else {
						isAlreadyOnEntrave = true;
					}
				}
			}

			if (diceList.Count > 0) {
				randomDice = Random.Range (0, diceList.Count);
				diceList [randomDice].isBlocked = true;

				if (diceList [randomDice].characterType == 1 || diceList [randomDice].characterType == 3) {
					diceList [randomDice].parentSummonerScript.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().ShowBlockedDice (diceList [randomDice].diceId);	
				} else {
					diceList [randomDice].parentSupportScript.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().ShowBlockedDice (diceList [randomDice].diceId);
				}

				if (isAlreadyOnEntrave) {
					newCombatEtat.duration = 0;
				}

				this.AddEtat (newEtat, newCombatEtat);
			}
		}
	}
    #endregion

	public void UpdateCards (EtatsData newEtat, CombatEtatData newCombatEtat, int newState) { // newState: 1/ j'ai ajouté un etat, 2/ j'update les valeur de l'etat
		for (int i = 0; i < this.summonerScript.L_Cards.Count; i++) {
			//print (this.summonerScript.L_Cards [i].owner.name + " / update car " + newEtat.name + " / " + newState);
			this.summonerScript.L_Cards [i].UpdateCard (newEtat, newCombatEtat, newState);
			//print (this.summonerScript.L_Cards [i].owner.name + " / " + this.summonerScript.L_Cards [i].tempAction.combatActions [0].damage);

			if (this.summonerScript.characterType == 3) {
				this.summonerScript.charactersInfosPanel.GetComponent<CharacterInfosPanel>().UpdateDice(i, this.summonerScript.L_CardsToApply [i].card);
				this.summonerScript.charactersInfosPanel.GetComponent<CharacterInfosPanel>().ShowActionDice(i);
			}
		}

		if (this.summonerScript.characterType == 1) {
			this.summonerScript.CallCardsInHandToUpdate ();
		}
	}

	public void RemoveEtat (EtatsData newEtat) {
		if (this.summonerScript != null) {
			for (int i = 0; i < this.summonerScript.L_etats.Count; i++) {
				if (this.summonerScript.L_etats [i].id == newEtat.id && this.summonerScript.L_etats [i].isRemovable) {
					this.summonerScript.L_etats.RemoveAt (i);
					this.UpdateCards (newEtat, new CombatEtatData(), 3); // 3 pour dire au panel card que je dois remove l'etat entier
					this.RemoveSpecialEtat (newEtat); // 3 pour dire au panel card que je dois remove l'etat entier
					this.summonerScript.CallUIAddEtat (newEtat, 3);
				}
			}
		} else {
			for (int i = 0; i < this.supportScript.L_etats.Count; i++) {
				if (this.supportScript.L_etats [i].id == newEtat.id && this.supportScript.L_etats [i].isRemovable) {
					this.supportScript.L_etats.RemoveAt (i);
					this.UpdateCards (newEtat, new CombatEtatData(), 3); // 3 pour dire au panel card que je dois remove l'etat entier
					this.RemoveSpecialEtat (newEtat); // 3 pour dire au panel card que je dois remove l'etat entier
					this.supportScript.CallUIAddEtat (newEtat, 3);
				}
			}
		}
	}

	public void RemoveSpecialEtat (EtatsData newEtat) {
		if (newEtat.id == 21) { // je dois remove l'entrave
			List<Dice> diceList = new List<Dice>();
			for (int i = 0; i < this.summonerScript.L_dices.Count; i++) {
				if (this.summonerScript.L_dices [i].isBlocked) {
					diceList.Add (this.summonerScript.L_dices [i]);
				}
			}
			for (int i = 0; i < this.summonerScript.L_supports.Count; i++) {
				Support supportScript = this.summonerScript.L_supports [i].GetComponent<Support> ();
				for (int j = 0; j < supportScript.L_dices.Count; j++) {
					if (supportScript.L_dices [j].isBlocked) {
						diceList.Add (supportScript.L_dices [j]);
					}
				}
			}
			for (int i = 0; i < newEtat.value; i++) {
				print ("la liste a enlever: " + diceList.Count);
				int randomDice = Random.Range (0, diceList.Count);
				print (randomDice);
				diceList [randomDice].isBlocked = false;
				if (diceList [randomDice].characterType == 1 || diceList [randomDice].characterType == 3) {
					diceList [randomDice].parentSummonerScript.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().HideBlockedDice (diceList [randomDice].diceId);	
				} else {
					diceList [randomDice].parentSupportScript.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().HideBlockedDice (diceList [randomDice].diceId);
				}
				diceList.RemoveAt (randomDice);
			}
		}
	}

	public void CheckEtats (int newTurnState) {
		if (this.summonerScript != null) {
			for (int i = 0; i < this.summonerScript.L_etats.Count; i++) {
				EtatsData etatTemp = this.summonerScript.L_etats [i];
				if (etatTemp.turnStateApply == newTurnState) {
					if (etatTemp.id != 21) { //je n'applique pas l'entrave pour lenlever
						this.AddSpecialEtat (new Card (0, this.gameObject, 2, 0, new ActionsData ()), etatTemp, new CombatEtatData ());
					}
					etatTemp.duration -= 1;
					if (etatTemp.duration <= 0 && etatTemp.isRemovable == true) {
						this.RemoveEtat (etatTemp);
						break;
					}
					this.summonerScript.CallUIAddEtat (etatTemp, 2);
				}
			}
		} else {
			for (int i = 0; i < this.supportScript.L_etats.Count; i++) {
				EtatsData etatTemp = this.supportScript.L_etats [i];
				if (etatTemp.turnStateApply == newTurnState) {
					this.AddSpecialEtat (new Card(0, this.gameObject, 2, 0, new ActionsData()), etatTemp, new CombatEtatData());
					etatTemp.duration -= 1;
					if (etatTemp.duration <= 0 && etatTemp.isRemovable == true) {
						this.RemoveEtat (etatTemp);
						break;
					}
					this.supportScript.CallUIAddEtat (etatTemp, 2);
				}
			}
		}
	}

	public void CheckEtatsOnAction (CombatActionData newCombat, GameObject newLauncher, int newDamage, int newActionState) { //action state sert a savoir si je lance l'attaque ou si j'attaque
		for (int i = 0; i < this.summonerScript.L_etats.Count; i++) {
			if (this.summonerScript.L_etats [i].id == 15 && newActionState == 1) { // c'est la riposte

				float damage = (float)newDamage;
				int intDamage = Mathf.CeilToInt ((damage * 30) / 100);

				CombatActionData combatActionTemp = new CombatActionData();
				combatActionTemp.colorDmg = newCombat.colorDmg;
				combatActionTemp.damage = intDamage;
				combatActionTemp.idAnim = "1";
				combatActionTemp.modulateur = "-";
				combatActionTemp.target = 2;
				combatActionTemp.typeDmg = 1;

				newLauncher.GetComponent<ActionManager> ().ApplyCombatAction (combatActionTemp, this.gameObject);
			} else if (this.summonerScript.L_etats [i].id == 20 && newActionState == 2) { // c'est le lien vampirique

				float damage = (float)newDamage;
				int intDamage = Mathf.CeilToInt ((damage * 30) / 100);

				CombatActionData combatActionTemp = new CombatActionData();
				combatActionTemp.colorDmg = newCombat.colorDmg;
				combatActionTemp.damage = intDamage;
				combatActionTemp.idAnim = "3";
				combatActionTemp.modulateur = "+";
				combatActionTemp.target = 1;
				combatActionTemp.typeDmg = 1;

				this.ApplyCombatAction (combatActionTemp, this.gameObject);
				this.summonerScript.charactersInfosPanel.GetComponent<CombatEffectsText> ().GenerateDamageText (2, intDamage);
			}
		}
	}

	#region Animation "convulsion" prise de degats
	public void GetDamageBegin () { // animation va-et-viens sur le personnage quand il prend des dégats
		iTween.MoveTo (this.gameObject, iTween.Hash(
			"position", new Vector3 (this.transform.position.x + 0.1f, this.transform.position.y, this.transform.position.z), 
			"time", 0.05f,
			"oncompletetarget", this.gameObject,
			"oncomplete", "GetDamageEnd"));
	}
	public void GetDamageEnd () {
		iTween.MoveTo (this.gameObject, iTween.Hash(
			"position", new Vector3 (this.transform.position.x - 0.1f, this.transform.position.y, this.transform.position.z), 
			"time", 0.05f,
			"easetype", iTween.EaseType.easeOutBack));
	}
	#endregion
}
