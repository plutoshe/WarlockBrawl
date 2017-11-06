using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerShooting : NetworkBehaviour
{	
	public GameObject FireballPrefab;
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
		fireball.GetComponent<Rigidbody>().velocity = fireball.transform.forward * 8;

		NetworkServer.Spawn(fireball);
		// Destroy the bullet after 2 seconds
		Destroy(fireball, 8.0f);
	}

	void Start() {
		for (var i = 0; i < AbilityList.Length; i++) {
			var j = i;
//			AbilityList [i].onClick.AddListener (() => SkillTrigger(i));
			AbilityList [i].onClick.AddListener (delegate {SkillTrigger(j);	});
		}
		SkillWait = new float[SkillInterval.Length];
		SkillInterval.CopyTo (SkillWait, 0);
		navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	void SkillTrigger(int i) {
		if (SkillWait [i] + SkillLastWait >= SkillInterval [i]) {
			SkillSelect = i;
			Cursor.SetCursor (cursorTexture, hotSpot, cursorMode);
		}
	}

	void Update () {

		SkillLastWait += Time.deltaTime;
		if (!isLocalPlayer) {
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;


		if (Input.GetButtonDown ("Fire1") && SkillSelect >= 0) {
			if (Physics.Raycast(ray, out hit, 100)) {
				Cursor.SetCursor(null, Vector2.zero, cursorMode);
				for (int i = 0; i < SkillWait.Length; i++) {
					SkillWait[i] += SkillLastWait;
				}
				SkillWait [SkillSelect] = 0;
				SkillLastWait = 0;
				if (SkillSelect == 0) {
					Vector3 targetDir = hit.point - transform.position; 
					transform.rotation = Quaternion.LookRotation (targetDir);
					CmdShootFireball ();
				}
				if (SkillSelect == 2) {
					Vector3 targetDir = hit.point - transform.position; 
					transform.rotation = Quaternion.LookRotation (targetDir);

					if (Vector3.Distance (hit.point, transform.position) > BlinkRadius) {
						targetDir = targetDir.normalized * BlinkRadius;
					}
					transform.position += targetDir;
					navMeshAgent.destination = transform.position;
				}
				SkillSelect = -1;
			}
		}

		if (Input.GetKeyDown (KeyCode.Q)) {
			AbilityList[0].onClick.Invoke();
		}
		if (Input.GetKeyDown (KeyCode.E)) {
			AbilityList[2].onClick.Invoke();
		}
	}
}
