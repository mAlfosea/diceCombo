using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner : MonoBehaviour {

	#region ID Variables
	public int id;
	public int idOriginal;
	public string summonerName;
	public string idAvatar;
	public SpriteRenderer avatarImg;
	public int characterType;
	public bool isKo;
	#endregion

	#region Stats Variables
	public int pvMax;
	public int pv; 
	public int force;
	public int armure;
	public int defense;
	#endregion

	#region Upgrades Variables
	public int tier; 
	#endregion

	#region Dices Variables
	public int nbDiceMax;
	public int nbDice;
	public List<Dice> L_dices = new List<Dice>();
	public int[] L_dicesValue;
	private Coroutine stopDiceAnimations; 
	#endregion

	#region Skills Variables
	public int nbSkillSlots;
	public int nbItemSlots;
	public List<int> L_skills = new List<int>();
	#endregion

	#region Etats Variables
	public List<int> L_talents = new List<int> ();
	public List<EtatsData> L_etats = new List<EtatsData> ();
	#endregion

	#region Supports Variables
	public List<int> L_supports_id = new List<int>();
	public List<GameObject> L_supports = new List<GameObject>();
	#endregion

	#region Cards Variables
	public List<Card> L_Cards = new List<Card>();
	public List<ActionToResolve> L_CardsToApply = new List<ActionToResolve>();
	private int cardGeneratedId; // me sert a définir chaque card par un UID du nombre de carte crée pendant le tour
	private Card previousCardPlayed; 
	public Card selectedCard;
	public GameObject cardTarget;
	public bool cardIsPlaying;
	//public bool animationIsPlaying;
	//public GameObject animationManagerPrefab;
	#endregion

	#region CharacterInfos Variables
	public GameObject charactersInfosPrefab;
	public GameObject charactersInfosPanel; 
	#endregion

	#region Events 
	public delegate void SendCard(Card newCard);
	public static event SendCard CardGenerated;

	public delegate void CardUpdatedWithEtat();
	public static event CardUpdatedWithEtat E_CardUpdatedWithEtat;

	public delegate void ActionDone(Card newCard);
	public static event ActionDone CardPlayed;

	public delegate void CardBanned(Card newCard);
	public static event CardBanned E_CardBanned;

	public delegate void DroppedCardAnim(Card newCard, bool newCardState); //newCardState permet de dire a la main si la carte est jouéé ou non
	public static event DroppedCardAnim PlayCardDroppedAnimation;

	public delegate void DroppedCard();
	public static event DroppedCard CheckLaunchableCard;

	public delegate void EndTurn();
	public static event EndTurn TurnEnded;

	public delegate void CallForToggleTurnButton();
	public static event CallForToggleTurnButton ToggleChangeTurnButton;

	public delegate void CharacterKO();
	public static event CharacterKO CharacterDeath;
	#endregion

	void OnEnable()
	{
		CardPanel.CardDragged += this.CardSelected;
		CardPanel.CardDropped += this.CardDropped;
		CardZone.TargetSelected += this.TargetSelected;
		CardZone.TargetDeselected += this.TargetDeselected;
	}


	void OnDisable()
	{
		CardPanel.CardDragged -= this.CardSelected;
		CardPanel.CardDropped -= this.CardDropped;
		CardZone.TargetSelected -= this.TargetSelected;
		CardZone.TargetDeselected -= this.TargetDeselected;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {		
	}

	public void Init () {
		this.GetComponent<ActionManager> ().LoadStats ();
		this.GetComponent<ActionManager> ().LoadTalents ();
		this.SetDices ();
	}

	public void StartTurn () {
		StartCoroutine (this.StartTurnFlow ());
	}

	public IEnumerator StartTurnFlow () {
		this.cardGeneratedId = 0; 
		this.defense = 0;
		this.CallUpdateUIInfos ();

		//yield return new WaitForSeconds (0.2f);

		if (this.characterType != 3) {
			this.previousCardPlayed = null;
			this.GetComponent<ActionManager> ().CheckEtats (1); // 1 pour signifier qu'on est en début de tour

			for (int i = 0; i < this.L_supports.Count; i++) {
				this.L_supports [i].GetComponent<ActionManager> ().CheckEtats (1);
			}

			//yield return new WaitForSeconds (0.4f);

			this.LaunchDiceAnimation ();
		} else {
			yield return new WaitForSeconds (0.2f);

			this.GetComponent<ActionManager> ().CheckEtats (1); // 1 pour signifier qu'on est en début de tour

			yield return new WaitForSeconds (0.4f);

			this.GetComponent<ActionManager> ().ApplyCard ();
		}
	}

	public void TurnTerminated () {
		this.GetComponent<ActionManager> ().CheckEtats (2); // 1 pour signifier qu'on est en début de tour

		for (int i = 0; i < this.L_supports.Count; i++) {
			this.L_supports [i].GetComponent<ActionManager> ().CheckEtats (2);
		}
	}

	#region Gestion des Dices
	private void SetDices () {
		for (int i = 0; i < this.nbDice; i++) {
			Dice dice = new Dice (this.gameObject, i, this.nbSkillSlots, 1);
			this.L_dices.Add (dice);
		}
		this.L_dicesValue = new int[this.L_dices.Count];
	}

	public void LaunchDiceAnimation () {
		this.CallUpdateUIShowDices ();

		for (int i = 0; i < this.L_dices.Count; i++) {
			
			//print (this.summonerName + " : Dés " + i + " est " + this.L_dices[i].isBlocked);

			if (this.L_dices [i].isBlocked == false) {
				this.L_dices [i].isDicing = true;
				this.L_dices [i].diceCoroutine = StartCoroutine (this.L_dices [i].DiceAnimation ());
			} else {
				this.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().ShowBlockedDice (i);				
			}
		}
		for (int i = 0; i < this.L_supports.Count; i++) {
			for (int j = 0; j < this.L_supports [i].GetComponent<Support> ().L_dices.Count; j++) {
				
				Support supportScript = this.L_supports [i].GetComponent<Support> ();
				supportScript.CallUpdateUIShowDices ();

				//print (supportScript.supportName + " : Dés " + j + " est " + supportScript.L_dices [j].isBlocked);

				if (supportScript.L_dices [j].isBlocked == false) {
					supportScript.L_dices [j].isDicing = true;
					supportScript.L_dices [j].diceCoroutine = StartCoroutine (supportScript.L_dices [j].DiceAnimation ());
				} else {
					supportScript.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().ShowBlockedDice (j);				
				}
			}
		}
		StartCoroutine (this.StopDiceAnimation());
	}

	public void LaunchSpecificDice (Card newCard) {

		if (newCard.OwnerType == 1 || newCard.OwnerType == 3) {
			newCard.owner.GetComponent<Summoner> ().L_dices [newCard.diceId].isDicing = true;
			newCard.owner.GetComponent<Summoner> ().L_dices [newCard.diceId].diceCoroutine = StartCoroutine (newCard.owner.GetComponent<Summoner> ().L_dices [newCard.diceId].DiceAnimation ());
		} else if (newCard.OwnerType == 2) {
			newCard.owner.GetComponent<Support> ().L_dices [newCard.diceId].isDicing = true;
			newCard.owner.GetComponent<Support> ().L_dices [newCard.diceId].diceCoroutine = StartCoroutine (newCard.owner.GetComponent<Support> ().L_dices [newCard.diceId].DiceAnimation ());
		}

		StartCoroutine (this.StopSpecificDiceAnimation(newCard));
	}

	private IEnumerator StopDiceAnimation () {
		yield return new WaitForSeconds (Random.Range(0.3f, 0.8f));
		for (int i = 0; i < this.L_dices.Count; i++) {
			if (this.L_dices [i].isBlocked == false) {
				StopCoroutine (this.L_dices [i].diceCoroutine);
				this.L_dices [i].isDicing = false;
				this.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().EndBumpDice (i);
				this.GenerateCard (this.gameObject, this.characterType, this.L_skills [this.L_dicesValue [i]], i);
				yield return new WaitForSeconds (0.5f);
			}
		}
		for (int i = 0; i < this.L_supports.Count; i++) {
			for (int j = 0; j < this.L_supports [i].GetComponent<Support> ().L_dices.Count; j++) {
				if (this.L_supports [i].GetComponent<Support> ().L_dices [j].isBlocked == false) {
					Support supportScript = this.L_supports [i].GetComponent<Support> ();
					StopCoroutine (supportScript.L_dices [j].diceCoroutine);
					supportScript.L_dices [j].isDicing = false;
					supportScript.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().EndBumpDice (j);
					this.GenerateCard (this.L_supports [i], supportScript.characterType, supportScript.L_skills [supportScript.L_dicesValue [j]], j);
					yield return new WaitForSeconds (0.5f);
				}
			}
		}

		if (ToggleChangeTurnButton != null && this.characterType == 1) {
			ToggleChangeTurnButton ();
		}
	}

	private IEnumerator StopSpecificDiceAnimation (Card newCard) {
		yield return new WaitForSeconds (0.5f);	

		if (newCard.OwnerType == 1 || newCard.OwnerType == 3) {
			//newCard.owner.GetComponent<Summoner> ().charactersInfosPanel.GetComponent<CharacterInfosPanel> ().ShowDices (newCard.diceId);
			StopCoroutine (newCard.owner.GetComponent<Summoner> ().L_dices [newCard.diceId].diceCoroutine);
			newCard.owner.GetComponent<Summoner> ().L_dices [newCard.diceId].isDicing = false;
			newCard.owner.GetComponent<Summoner> ().charactersInfosPanel.GetComponent<CharacterInfosPanel> ().EndBumpDice (newCard.diceId);
			this.GenerateCard (newCard.owner, newCard.OwnerType, newCard.owner.GetComponent<Summoner> ().L_skills [newCard.owner.GetComponent<Summoner> ().L_dicesValue[newCard.diceId]], newCard.diceId);
		} else if (newCard.OwnerType == 2) {
			StopCoroutine (newCard.owner.GetComponent<Support> ().L_dices [newCard.diceId].diceCoroutine);
			newCard.owner.GetComponent<Support> ().L_dices [newCard.diceId].isDicing = false;
			newCard.owner.GetComponent<Support> ().charactersInfosPanel.GetComponent<CharacterInfosPanel> ().EndBumpDice (newCard.diceId);
			this.GenerateCard (newCard.owner, newCard.OwnerType, newCard.owner.GetComponent<Support> ().L_skills [newCard.owner.GetComponent<Support> ().L_dicesValue[newCard.diceId]], newCard.diceId);
		}
	}
	#endregion

	#region Generation des cards
	public void GenerateCard (GameObject newOwner, int newCharacterType, int newSkillId, int newDiceId) {
		//ActionsData action;
		//DictionaryManager.actionsDictionary.TryGetValue (newSkillId, out action);

		ActionsData tempAction = DictionaryManager.GetAction (newSkillId);

		Card tempCard = new Card (this.cardGeneratedId, newOwner, newCharacterType, newDiceId, tempAction);
		tempCard.IsLaunchable (this.previousCardPlayed);
		this.cardGeneratedId += 1;
		this.L_Cards.Add (tempCard);

		if (newCharacterType == 1 || newCharacterType == 3) {
			for (int i = 0; i < this.L_etats.Count; i++) {
				tempCard.UpdateCard (this.L_etats [i], new CombatEtatData (), 1);
			}
		} else {
			for (int i = 0; i < newOwner.GetComponent<Support> ().L_etats.Count; i++) {
				tempCard.UpdateCard (newOwner.GetComponent<Support> ().L_etats [i], new CombatEtatData (), 1);
			}
			for (int i = 0; i < this.L_etats.Count; i++) {
				tempCard.UpdateCard (this.L_etats [i], new CombatEtatData (), 1);
			}
		}

		if (CardGenerated != null && this.characterType != 3)
			CardGenerated (tempCard);

		this.charactersInfosPanel.GetComponent<CharacterInfosPanel>().UpdateDice (newDiceId, tempCard);

		if (this.characterType == 3) {
			this.L_CardsToApply.Add (new ActionToResolve (this.gameObject, this.cardTarget, tempCard, this.previousCardPlayed));
			this.selectedCard = null;
			this.charactersInfosPanel.GetComponent<CharacterInfosPanel>().ShowActionDice(newDiceId);
		}
		//print (this.summonerName + ": jai genere: " + action.name + " / launchable: " + tempCard.isLaunchable + " / j'ai : " + this.L_Cards.Count);
	}
	#endregion

	#region Gestion de la creation d'attaque: Drag / Drop...
	public void CardSelected (Card newCard) {
		if (this.characterType == 1) {
			this.selectedCard = newCard;
			//print (this.summonerName + ": je selectionne la carte : " + this.selectedCard.action.name + " / " + this.selectedCard.uId);
		}
	}
	public void TargetSelected (GameObject newTarget) {
		if (this.selectedCard != null && this.characterType == 1) {
			this.cardTarget = newTarget;
			//print (this.summonerName + ": je selectionne la target : " + this.cardTarget.name + " / " + this.selectedCard.uId);
		}
	}
	public void TargetDeselected () {
		if (this.selectedCard != null && this.characterType == 1) {
			this.cardTarget = null;
			//print (this.summonerName + ": je deselectionne la target ");
		}
	}
	public void CardDropped () {
		if (this.selectedCard != null && this.cardTarget != null && this.characterType == 1) {
			//print (this.summonerName + ": jai une attaque complete, je vais lancer : " + this.selectedCard.action.name + " sur la cible : " + this.cardTarget.name + " / " + this.selectedCard.uId);

			this.L_CardsToApply.Add (new ActionToResolve (this.gameObject, this.cardTarget, this.selectedCard, this.previousCardPlayed));

			this.previousCardPlayed = this.selectedCard;
			this.CheckCardIsLaunchable ();
			if (CheckLaunchableCard != null) {
				CheckLaunchableCard ();
			}

			this.cardTarget = null;
			this.selectedCard = null;

			this.CheckCardsToApplyList ();
		} else {
			if (this.selectedCard != null && this.characterType == 1) {
				if (PlayCardDroppedAnimation != null) {
					PlayCardDroppedAnimation (this.selectedCard, false);
				}
				this.selectedCard = null;
				//print (this.summonerName + ": l'attaque est un flop j'annule tout");
			}
		}
	}
	#endregion

	#region Apply Card & Degats events
	public void CallDroppedCardEvent (Card newCard, bool newBool) {
		if (PlayCardDroppedAnimation != null) {
			PlayCardDroppedAnimation (newCard, newBool);
		}
	}

	public void CardPlayedEvent (Card newCard) {
		if (CardPlayed != null) {
			CardPlayed (newCard);
			//print ("je demande a supprimer la carte: " + newCard.uId);
		}
	}

	public void CardBanEvent (Card newCard) {
		if (E_CardBanned != null) {
			E_CardBanned (newCard);
			//print ("je demande a supprimer la carte: " + newCard.uId);
		}
	}
	#endregion

	#region Card List Update
	public void RemoveCardFromList (Card newCard) {
		for (int i = 0; i < this.L_Cards.Count; i++) {
			if (this.L_Cards [i].uId == newCard.uId) {
				this.L_Cards.RemoveAt (i);
				return;
			}
		}
	}

	public void CheckCardIsLaunchable () {
		for (int i = 0; i < this.L_Cards.Count; i++) {
			this.L_Cards [i].IsLaunchable (this.previousCardPlayed);
			//print (this.previousCardPlayed.owner.GetComponent<Summoner>().isMonster + " / " + this.L_Cards[i].owner.GetComponent<Summoner>().isMonster + " / " + this.previousCardPlayed.tempAction.color + " / " + this.L_Cards[i].tempAction.color + " / " + this.previousCardPlayed.tempAction.type + " / " + this.L_Cards[i].tempAction.type + " donc is launchable " + this.L_Cards[i].isLaunchable);
			//print ("LE SUMMONER DIT: la dernieres etant : " + this.previousCardPlayed.action.name + " / " + this.L_Cards [i].isLaunchable);
		}
	}

	public void CheckCardsToApplyList () {
		if (this.cardIsPlaying == false) {
			if (this.L_CardsToApply.Count > 0) {
				this.cardIsPlaying = true;
				this.GetComponent<ActionManager> ().ApplyCard ();
			} else {
				if (this.characterType == 3) {
					this.TurnTerminated ();
					if (TurnEnded != null) {
						TurnEnded ();
					}
				}
			}
		}
	}
	#endregion

	#region Call UI Updates
	public void CallUpdateUIShowDices () {
		this.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().ShowDices ();
	}

	public void CallUpdateUIInfos () {
		this.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().UpdateInfos ();
	}

	public void CallUIAddEtat (EtatsData newEtat, int newInt) { // 1 je add / 2 jupdate / 3 je remove
		if (newInt == 1) {
			this.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().AddEtat (newEtat);
		} else if (newInt == 2) {
			this.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().UpdateEtat (newEtat);
		} else if (newInt == 3) {
			this.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().RemoveEtat (newEtat);
		}
	}

	public void CallCardsInHandToUpdate () {
		if (E_CardUpdatedWithEtat != null) {
			E_CardUpdatedWithEtat ();
		}
	}
	#endregion

	#region Remove Supports
	public void EndCombat () {
		Destroy (this.charactersInfosPanel);
		for (int i = 0; i < this.L_supports.Count; i++) {
			Destroy (L_supports [i].GetComponent<Support> ().charactersInfosPanel);
			Destroy (L_supports [i]);
		}
	}
	#endregion

	public void Death () {
		this.isKo = true;
		this.charactersInfosPanel.SetActive (false);
		if (CharacterDeath != null) {
			CharacterDeath ();
		}
	}

	public void StopFight () {
		StopAllCoroutines ();
		this.GetComponent<ActionManager> ().StopAllCoroutines ();
		this.L_CardsToApply.Clear ();
	}
}
