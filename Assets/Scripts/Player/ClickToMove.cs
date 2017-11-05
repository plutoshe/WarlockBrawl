using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ClickToMove : NetworkBehaviour {
	public int MaxHealth;
	private int currentHealth;
	public float shootDistance = 10f;
	public float shootRate = .5f;
	public PlayerShooting shootingScript;

	private Animator anim;
	private UnityEngine.AI.NavMeshAgent navMeshAgent;
	private Transform targetedEnemy;
	private Ray shootRay;
	private RaycastHit shootHit;
	private bool walking;
//		private bool enemyClicked;
	private float nextFire;
	public Material LocalMaterial;
	// Use this for initialization

//	public void Awake() {
//		
//	}
		
	public override void OnStartLocalPlayer()
	{
		anim = GetComponent<Animator> ();
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
//					enemyClicked = false;
				navMeshAgent.destination = hit.point;
				navMeshAgent.Resume();
//					navMeshAgent.isStopped (false);
			}
		}
			
		if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
			if (!navMeshAgent.hasPath || Mathf.Abs (navMeshAgent.velocity.sqrMagnitude) < float.Epsilon)
				walking = false;
		} else {
			walking = true;
		}

		anim.SetBool ("IsWalking", walking);
	}



}

