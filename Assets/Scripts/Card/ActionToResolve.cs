using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionToResolve {

	public GameObject launcher;
	public GameObject target;
	public Card card;
	public Card previousCard;

	public ActionToResolve (GameObject newLauncher, GameObject newTarget, Card newCard, Card newPreviousCard) {
		this.launcher = newLauncher;
		this.target = newTarget;
		this.card = newCard;
		this.previousCard = newPreviousCard;
	}
}
