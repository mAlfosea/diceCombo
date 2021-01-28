using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightPanel : MonoBehaviour {

	public GameObject monsterBlocPrefab;

	public GameObject monstersContainer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init() {
		this.DestroyMonsters ();

		for (int i = 0; i < CampaignManager.monstersAvailable.Length; i++) {
			GameObject monsterTeamBloc = Instantiate (this.monsterBlocPrefab, this.transform.position, this.transform.rotation, this.monstersContainer.transform) as GameObject;
			monsterTeamBloc.GetComponent<MonsterPanel> ().Init (CampaignManager.monstersAvailable [i]);
		}
	}

	public void DestroyMonsters() {
		List<GameObject> monstersToDelete = new List<GameObject> ();

		for (int i = 0; i < this.monstersContainer.transform.childCount; i++) {
			monstersToDelete.Add(this.monstersContainer.transform.GetChild(i).gameObject);
		}
		for (int i = 0; i < monstersToDelete.Count; i++) {
			Destroy (monstersToDelete [i]);
		}
		monstersToDelete.Clear ();
	}
}
