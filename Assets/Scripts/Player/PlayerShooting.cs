using UnityEngine;
using UnityEngine.Networking;

public class PlayerShooting : NetworkBehaviour
{	
	public GameObject FireballPrefab;
	public Transform SkillSpawn;
	public float FireballInterval = 2.0f;
	public float FireballWait = 2.0f;
	private bool SkillSelect = false;

	public Texture2D cursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = Vector2.zero;


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

	void Update () {

		FireballWait += Time.deltaTime;
		if (!isLocalPlayer) {
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;


		if (Input.GetButtonDown ("Fire1") && SkillSelect) {
			if (Physics.Raycast(ray, out hit, 100)) {
				Cursor.SetCursor(null, Vector2.zero, cursorMode);
				FireballWait = 0;	
				SkillSelect = false;
				Vector3 targetDir = hit.point - transform.position; 
				transform.rotation = Quaternion.LookRotation(targetDir);
				CmdShootFireball ();
			}
		}

		if (Input.GetKeyDown (KeyCode.Q) && FireballWait >= FireballInterval) {
			Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
			SkillSelect = true;
		}


	}
}
