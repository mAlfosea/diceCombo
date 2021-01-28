using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card  {

	public int uId;
	public GameObject owner;
	public int OwnerType;
	public int diceId;
	public ActionsData action;
	public ActionsData tempAction;
	public bool isLaunchable;

	public Card (int newUId, GameObject newOwner, int newOwnerType, int newDiceId, ActionsData newAction) {
		this.uId = newUId;
		this.owner = newOwner;
		this.OwnerType = newOwnerType;
		this.diceId = newDiceId;
		this.action = newAction;
		this.tempAction = this.action;
	}

	public void IsLaunchable (Card newCard) {
		if (newCard != null) {
			if (this.OwnerType == 1 || this.OwnerType == 2) {
			//if (newCard.owner.GetComponent<Summoner> ().characterType == this.owner.GetComponent<Summoner> ().characterType) {
				if (newCard.tempAction.color == this.tempAction.color || newCard.tempAction.type == this.tempAction.type || newCard.tempAction.color == 0 || this.tempAction.color == 0) {
					this.isLaunchable = true;
				} else {
					this.isLaunchable = false;
				}
			}
		} else {
			this.isLaunchable = true;
		}
	}

	public void UpdateCard (EtatsData newEtat, CombatEtatData newCombatEtat, int newState) {
		if (newEtat.id == 1 || newEtat.id == 5) {
			for (int i = 0; i < this.tempAction.combatActions.Length; i++) {
				if (this.tempAction.combatActions [i].typeDmg == 1 && this.tempAction.combatActions[i].colorDmg == 1) {
					if (newState == 1) { // c'est un ajout, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage += newEtat.value;					
					} else if (newState == 2) { // c'est un ajout, donc j'utiliser newcombat
						this.tempAction.combatActions [i].damage += newCombatEtat.value;					
					} else if (newState == 3) { // c'est un remove, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage -= newEtat.value;					
					}
					this.tempAction.combatActions [i].damage = this.CheckDamageValue (this.tempAction.combatActions [i].damage);
				}
			}
		} else if (newEtat.id == 2 || newEtat.id == 6) {
			for (int i = 0; i < this.tempAction.combatActions.Length; i++) {
				if (this.tempAction.combatActions [i].typeDmg == 1 && this.tempAction.combatActions[i].colorDmg == 2) {
					if (newState == 1) { // c'est un ajout, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage += newEtat.value;					
					} else if (newState == 2) { // c'est un ajout, donc j'utiliser newcombat
						this.tempAction.combatActions [i].damage += newCombatEtat.value;					
					} else if (newState == 3) { // c'est un remove, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage -= newEtat.value;					
					}
					this.tempAction.combatActions [i].damage = this.CheckDamageValue (this.tempAction.combatActions [i].damage);
				}
			}
		} else if (newEtat.id == 3 || newEtat.id == 7) {
			for (int i = 0; i < this.tempAction.combatActions.Length; i++) {
				if (this.tempAction.combatActions [i].typeDmg == 1 && this.tempAction.combatActions[i].colorDmg == 3) {
					if (newState == 1) { // c'est un ajout, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage += newEtat.value;					
					} else if (newState == 2) { // c'est un ajout, donc j'utiliser newcombat
						this.tempAction.combatActions [i].damage += newCombatEtat.value;					
					} else if (newState == 3) { // c'est un remove, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage -= newEtat.value;					
					}
					this.tempAction.combatActions [i].damage = this.CheckDamageValue (this.tempAction.combatActions [i].damage);
				}
			}
		} else if (newEtat.id == 4 || newEtat.id == 8) {
			for (int i = 0; i < this.tempAction.combatActions.Length; i++) {
				if (this.tempAction.combatActions [i].typeDmg == 1 && this.tempAction.combatActions[i].colorDmg == 4) {
					if (newState == 1) { // c'est un ajout, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage += newEtat.value;					
					} else if (newState == 2) { // c'est un ajout, donc j'utiliser newcombat
						this.tempAction.combatActions [i].damage += newCombatEtat.value;					
					} else if (newState == 3) { // c'est un remove, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage -= newEtat.value;					
					}
					this.tempAction.combatActions [i].damage = this.CheckDamageValue (this.tempAction.combatActions [i].damage);
				}
			}
		} else if (newEtat.id == 10 || newEtat.id == 17 || newEtat.id == 23) { // c'est la force
			for (int i = 0; i < this.tempAction.combatActions.Length; i++) {
				if (this.tempAction.combatActions [i].typeDmg == 1) {
					if (newState == 1) { // c'est un ajout, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage += newEtat.value;					
					} else if (newState == 2) { // c'est un ajout, donc j'utiliser newcombat
						this.tempAction.combatActions [i].damage += newCombatEtat.value;					
					} else if (newState == 3) { // c'est un remove, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage -= newEtat.value;					
					}
					this.tempAction.combatActions [i].damage = this.CheckDamageValue (this.tempAction.combatActions [i].damage);
				}
			}
		} else if (newEtat.id == 11) {
			for (int i = 0; i < this.tempAction.combatActions.Length; i++) {
				if (this.tempAction.combatActions [i].typeDmg == 1) {
					if (newState == 1) { // c'est un ajout, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage -= newEtat.value;					
					} else if (newState == 2) { // c'est un ajout, donc j'utiliser newcombat
						this.tempAction.combatActions [i].damage -= newCombatEtat.value;					
					} else if (newState == 3) { // c'est un ajout, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage += newEtat.value;	
					}
					this.tempAction.combatActions [i].damage = this.CheckDamageValue (this.tempAction.combatActions [i].damage);
				}
			}
		} else if (newEtat.id == 12 || newEtat.id == 22 || newEtat.id == 24) { // c'est l'armure
			for (int i = 0; i < this.tempAction.combatActions.Length; i++) {
				if (this.tempAction.combatActions [i].typeDmg == 2) {
					if (newState == 1) { // c'est un ajout, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage += newEtat.value;					
					} else if (newState == 2) { // c'est un ajout, donc j'utiliser newcombat
						this.tempAction.combatActions [i].damage += newCombatEtat.value;					
					} else if (newState == 3) { // c'est un ajout, donc j'utiliser newEtat
						this.tempAction.combatActions [i].damage -= newEtat.value;	
					}
					this.tempAction.combatActions [i].damage = this.CheckDamageValue (this.tempAction.combatActions [i].damage);
				}
			}
		} else if (newEtat.id == 13) {
			for (int i = 0; i < this.tempAction.combatActions.Length; i++) {
				if (this.tempAction.combatActions [i].typeDmg == 2) {
					if (newState == 1) { // c'est un ajout, donc j'utiliser newEtat
						float damage = (float)this.tempAction.combatActions [i].damage;
						int intDamage = Mathf.CeilToInt ((damage * 25) / 100);
						this.tempAction.combatActions [i].damage = this.tempAction.combatActions [i].damage - intDamage;					
					} else if (newState == 2) { // c'est un ajout, donc j'utiliser newcombat
						float damage = (float)this.tempAction.combatActions [i].damage;
						int intDamage = Mathf.CeilToInt ((damage * 25) / 100);
						this.tempAction.combatActions [i].damage = this.tempAction.combatActions [i].damage - intDamage;					
					} else if (newState == 3) { // c'est un ajout, donc j'utiliser newEtat
						float damage = (float)this.tempAction.combatActions [i].damage;
						int intDamage = Mathf.CeilToInt ((damage * 25) / 100);
						this.tempAction.combatActions [i].damage = this.tempAction.combatActions [i].damage + intDamage;		
					}
					this.tempAction.combatActions [i].damage = this.CheckDamageValue (this.tempAction.combatActions [i].damage);
				}
			}
		}
	}

	public int CheckDamageValue (int newDamage) {
		int damage = newDamage;

		if (damage < 0) {
			damage = 0;
		}

		return damage;
	}
}
