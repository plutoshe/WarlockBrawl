using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Movement : NetworkBehaviour {
	public bool Debug = true;
	public int MaxHealth;
	private int currentHealth;
	public float shootDistance = 10f;
	public float shootRate = .5f;
	private Rigidbody rb;


	private Animator anim;
	public UnityEngine.AI.NavMeshAgent navMeshAgent;


	private Transform targetedEnemy;
	private Ray shootRay;
	private RaycastHit shootHit;
	private bool walking;
	private float nextFire;
	public Material LocalMaterial;
	public Canvas FollowCanvas;

	// put force setting
//	public float radius = 5.0F;
//	public float power = 300.0F;
		
	public override void OnStartLocalPlayer()
	{
		anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody> ();

//		Vector3 explosionPos = new Vector3(0,0,0);
//		Collider[] objectsInRange = Physics.OverlapSphere(explosionPos, radius);
//		foreach (var i in objectsInRange) {
//			Debug.Log(i.name);
//		}
//			transform.position;

//		rb.AddExplosionForce(power, explosionPos, radius, 0, ForceMode.Impulse);
//		Lerp
		currentHealth = MaxHealth;
		navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		Camera.main.GetComponent<CameraFollow> ().SetTarget (transform);
		var childrenMaterial = GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach(var children in childrenMaterial)
		{
			if (children.name == "Player") {
				children.GetComponent<SkinnedMeshRenderer>().material = LocalMaterial;
			}
		}


	}

//	void AddForce() {
//		rb.AddForce(, ForceMode.Impulse);
//	}

	// Update is called once per frame
	void Update() 
	{
		if (!isLocalPlayer) {
			return;
		}

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Input.GetButtonDown ("Fire2")) 
		{
			if (Physics.Raycast(ray, out hit, 100))
			{

				walking = true;
				navMeshAgent.destination = hit.point;
				navMeshAgent.isStopped  = false;

			}
		}
		if (navMeshAgent.isStopped)
			walking = false;
		else if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
			if (!navMeshAgent.hasPath || Mathf.Abs (navMeshAgent.velocity.sqrMagnitude) < float.Epsilon)
				walking = false;
		} else {
			walking = true;
		}
		if (Debug && Input.GetKeyDown(KeyCode.Space)) {
			var force = transform.forward.normalized * 30;
			rb.AddForce (force, ForceMode.Impulse);
			navMeshAgent.isStopped = true;
		}
		anim.SetBool ("IsWalking", walking);
	}

	public void Impluse(Vector3 force) {
		rb.AddForce (force, ForceMode.Impulse);
		navMeshAgent.isStopped = true;
	}

}

