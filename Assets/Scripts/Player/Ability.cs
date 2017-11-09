using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class Ability : NetworkBehaviour
{	
	[SyncVar(hook = "OnChangemManaShield")] 
	public bool manaShieldStatus = false;
	public GameObject FireballPrefab;
	public GameObject PutballPrefab;
	public GameObject ManaShield;
	public GameObject AbilityPanel;
	private UnityEngine.AI.NavMeshAgent navMeshAgent;

	public Button[] AbilityList;
	public Image[] AbilityCountdown;

	public UnityEngine.KeyCode[] AbilityKey = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };

	public Transform SkillSpawn;
	public float[] SkillInterval = {2.0f, 2.0f, 2.0f, 2.0f};
	private float[] SkillWait;
	private float SkillLastWait;
	private int SkillSelect = -1;

	public float BlinkRadius = 6.0f;
	public Texture2D cursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = Vector2.zero;


	// This [Command] code is called on the Client …
	// … but it is run on the Server!
	[Command]
	void CmdShootFireball(Vector3 ballPosition, Quaternion ballRotation) {
		
		//		var gotoRotation = Quaternion.FromToRotation(transform.rotation, hit.

		var fireball = (GameObject)Instantiate (
			FireballPrefab,
			ballPosition,
			ballRotation);
		

		//		Quaternion.RotateTowards
		// Add velocity to the bullet
		fireball.GetComponent<Rigidbody>().velocity = fireball.transform.forward.normalized * 8;
		NetworkServer.Spawn(fireball);
		// Destroy the bullet after 2 seconds
		Destroy(fireball, 8.0f);
	}

	[Command]
	void CmdShootPutball(Vector3 ballPosition, Quaternion ballRotation) {
		var putball = (GameObject)Instantiate (
            PutballPrefab,
            ballPosition,
			ballRotation);
		
//		putball.GetComponent<ParticleSystem>().startRotation3D = SkillSpawn.rotation;
		putball.GetComponent<Rigidbody>().velocity = putball.transform.forward.normalized * 3;

		NetworkServer.Spawn(putball);
		Destroy(putball, 8.0f);
	}

	[Command] 
	void CmdManaShield(bool status) {
		manaShieldStatus = status;
//		ManaShield.gameObject.SetActive (true);
//		StartCoroutine (ManaShieldFade (0.5f));
//		var objNetId = obj.GetComponent<NetworkIdentity> ();        // get the object's network ID
//		objNetId.AssignClientAuthority (connectionToClient);    // assign authority to the player who is changing the color
//		Debug.Log(obj.name);
//		RpcManaShield (obj, status);                                    // usse a Client RPC function to "paint" the object on all clients
//		objNetId.RemoveClientAuthority (connectionToClient);  
	}

	void OnChangemManaShield(bool manaShieldStatus) {
		ManaShield.SetActive (manaShieldStatus);
	}


	void Start() {

		AbilityList = new Button[AbilityKey.Length];
		AbilityCountdown = new Image[AbilityKey.Length];
		AbilityList = AbilityPanel.GetComponentsInChildren<Button> ();
		for (var i = 0; i < AbilityList.Length; i++) {
			AbilityCountdown [i] = AbilityList [i].transform.GetChild(1).GetComponent<Image>();
			var tmpI = i;
			AbilityList [i].onClick.AddListener (delegate {SkillTrigger(tmpI);	});
		}

		SkillWait = new float[SkillInterval.Length];
		SkillInterval.CopyTo (SkillWait, 0);
		navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		ManaShield.SetActive (false);

	}

	void SkillTrigger(int i) {
		if (SkillWait [i] + SkillLastWait >= SkillInterval [i]) {
			SkillSelect = i;
			if (i != 3) { // Defend is a non directive ability
				Cursor.SetCursor (cursorTexture, hotSpot, cursorMode);
			}
		}
	}

	IEnumerator ManaShieldFade(float waitTime) {
		yield return new WaitForSeconds (waitTime);
		CmdManaShield (false);
	}


	void Update () {
		if (!isLocalPlayer) {
			AbilityPanel.SetActive (false);
			return;
		}
		AbilityPanel.SetActive (true);
		SkillLastWait += Time.deltaTime;
		for (int i = 0; i < AbilityCountdown.Length; i++) {
			var fillFrac = SkillInterval [i] - SkillLastWait - SkillWait [i];
			if (fillFrac < 0) fillFrac = 0;
			if  (ManaShield.activeSelf) 
				AbilityCountdown [i].fillAmount = 1;
			else 
				AbilityCountdown [i].fillAmount = fillFrac / SkillInterval [i];
		}
		if (ManaShield.activeSelf)
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
					CmdShootFireball (SkillSpawn.position, SkillSpawn.rotation);
				}
				if (SkillSelect == 1) {
					Vector3 targetDir = hit.point - transform.position; 
					transform.rotation = Quaternion.LookRotation (targetDir);

					CmdShootPutball (SkillSpawn.position, SkillSpawn.rotation);
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
//					Debug.Log(ManaShield.name);
					CmdManaShield (true);

					StartCoroutine (ManaShieldFade (1f));
//					CmdManaShield ();
				}
				SkillSelect = -1;
			}
		} 
		for (int i = 0; i < AbilityList.Length; i++) {
//			Debug.Log (AbilityKey [i].ToString ());
			if (Input.GetKeyDown (AbilityKey[i])) {
				AbilityList [i].onClick.Invoke ();
			} 
		}
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Cursor.SetCursor (null, Vector2.zero, cursorMode);
			SkillSelect = -1;
		}
	}
}
