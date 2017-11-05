using UnityEngine;
using UnityEngine.Networking;

public class PlayerShooting : NetworkBehaviour
{	
	public GameObject FireballPrefab;
	public Transform SkillSpawn;
	public float FireballInterval = 2.0f;
	public float FireballWait = 2.0f;

	// This [Command] code is called on the Client …
	// … but it is run on the Server!
	[Command]
	void CmdShootFireball() {
		
		var fireball = (GameObject)Instantiate (
			FireballPrefab,
			SkillSpawn.position,
			SkillSpawn.rotation);
		
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
		if (Input.GetKeyDown (KeyCode.Q) & FireballWait >= FireballInterval) {
			FireballWait = 0;
			CmdShootFireball();
		}
	}
}
