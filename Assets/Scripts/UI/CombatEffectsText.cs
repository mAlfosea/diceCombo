using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatEffectsText : MonoBehaviour {

	public GameObject damageTextPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GenerateDamageText (int newDamageType, int newDamage) {
		Vector3 instantiatePos = new Vector3 (Random.Range(this.transform.position.x - 30f, this.transform.position.x + 30f), this.transform.position.y + 200f, this.transform.position.z);
		GameObject damageText = Instantiate (this.damageTextPrefab, instantiatePos, this.transform.rotation, this.transform) as GameObject;
		damageText.GetComponent<TextAnimation> ().DamageAnimation (newDamageType, newDamage);
	}
}
