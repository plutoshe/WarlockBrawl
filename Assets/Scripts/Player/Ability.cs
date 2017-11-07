using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class Ability : NetworkBehaviour
{	
	public GameObject FireballPrefab;
	public GameObject PutballPrefab;
	public GameObject ManaShield;
	private UnityEngine.AI.NavMeshAgent navMeshAgent;

	public Transform SkillSpawn;
	public float[] SkillInterval = {2.0f, 2.0f, 2.0f, 2.0f};
	private float[] SkillWait;
	private float SkillLastWait;
	private int SkillSelect = -1;

	public float BlinkRadius = 6.0f;
	public Texture2D cursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = Vector2.zero;
	public Button[] AbilityList;

	// This [Command] code is called on the Client …
	// … but it is run on the Server!
	[Command]
	void CmdShootFireball() {

		//		var gotoRotation = Quaternion.FromToRotation(transform.rotation, hit.

		var fireball = (GameObject)Instantiate (
			FireballPrefab,
			SkillSpawn.position,
			SkillSpawn.rotation);
		

		//		Quaternion.RotateTowards
		// Add velocity to the bullet
		fireball.GetComponent<Rigidbody>().velocity = fireball.transform.forward.normalized * 8;
		NetworkServer.Spawn(fireball);
		// Destroy the bullet after 2 seconds
		Destroy(fireball, 8.0f);
	}

	[Command]
	void CmdShootPutball() {
		var putball = (GameObject)Instantiate (
            PutballPrefab,
            SkillSpawn.position,
			SkillSpawn.rotation);
		
//		putball.GetComponent<ParticleSystem>().startRotation3D = SkillSpawn.rotation;
		putball.GetComponent<Rigidbody>().velocity = putball.transform.forward.normalized * 3;
//		Debug.Log("!!!" + transform.parent.GetInstanceID ());

		NetworkServer.Spawn(putball);
		Destroy(putball, 8.0f);
	}

	[Command] 
	void CmdManaShield() {
		ManaShield.gameObject.SetActive (true);
		StartCoroutine (ManaShieldFade (0.5f));
	}

	IEnumerator ManaShieldFade(float waitTime) {
		yield return new WaitForSeconds (waitTime);
		ManaShield.gameObject.SetActive (false);
	}

	void Start() {
		Debug.Log("Update " + gameObject.GetInstanceID ());
		for (var i = 0; i < AbilityList.Length; i++) {
			var j = i;
//			AbilityList [i].onClick.AddListener (() => SkillTrigger(i));
			AbilityList [i].onClick.AddListener (delegate {SkillTrigger(j);	});
		}
		SkillWait = new float[SkillInterval.Length];
		SkillInterval.CopyTo (SkillWait, 0);
		navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		ManaShield.SetActive (false);
//		var ballLayer = LayerMask.GetMask ("Ball");
//		Physics.IgnoreLayerCollision (ballLayer, ballLayer);
	}

	void SkillTrigger(int i) {
		if (SkillWait [i] + SkillLastWait >= SkillInterval [i]) {
			SkillSelect = i;
			if (i != 3) { // Defend is a non directive ability
				Cursor.SetCursor (cursorTexture, hotSpot, cursorMode);
			}
		}
	}

	void Update () {
		
//		Debug.Log("Update " + gameObject.GetInstanceID ());
		SkillLastWait += Time.deltaTime;
		if (!isLocalPlayer) {
			return;
		}
		if (ManaShield.active)
			return;
		
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;


		if (SkillSelect == 3 || (Input.GetButtonDown ("Fire1") && SkillSelect >= 0)) {
			if (Physics.Raycast (ray, out hit, 100)) {
				Cursor.SetCursor (null, Vector2.zero, cursorMode);
				for (int i = 0; i < SkillWait.Length; i++) {
					SkillWait [i] += SkillLastWait;
				}
				SkillWait [SkillSelect] = 0;
				SkillLastWait = 0;
				if (SkillSelect == 0) {
					Vector3 targetDir = hit.point - transform.position; 
					transform.rotation = Quaternion.LookRotation (targetDir);
					CmdShootFireball ();
				}
				if (SkillSelect == 1) {
					Vector3 targetDir = hit.point - transform.position; 
					transform.rotation = Quaternion.LookRotation (targetDir);
					CmdShootPutball ();
				}
				if (SkillSelect == 2) {
					Vector3 targetDir = hit.point - transform.position; 
					transform.rotation = Quaternion.LookRotation (targetDir);

					if (Vector3.Distance (hit.point, transform.position) > BlinkRadius) {
						targetDir = targetDir.normalized * BlinkRadius;
					}
					transform.position += targetDir;
					navMeshAgent.isStopped = true;
//					navMeshAgent.destination = transform.position;
				}
				if (SkillSelect == 3) {
					CmdManaShield ();
				}
				SkillSelect = -1;
			}
		} 
		if (Input.GetKeyDown (KeyCode.Q)) {
			AbilityList [0].onClick.Invoke ();
		} 
		if (Input.GetKeyDown (KeyCode.W)) {
			AbilityList [1].onClick.Invoke ();
		} 
		if (Input.GetKeyDown (KeyCode.E)) {
			AbilityList [2].onClick.Invoke ();
		} 
		if (Input.GetKeyDown (KeyCode.R)) {
			AbilityList [3].onClick.Invoke ();
		} 
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Cursor.SetCursor (null, Vector2.zero, cursorMode);
			SkillSelect = -1;
		}
	}
}
