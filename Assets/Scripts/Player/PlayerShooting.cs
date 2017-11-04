using UnityEngine;

public class PlayerShooting : MonoBehaviour
{	
	public GameObject FireballPrefab;
	public Transform SkillSpawn;
	public float FireballInterval = 2.0f;
	public float FireballWait = 2.0f;
	void ShootFireball() {
		

		var fireball = (GameObject)Instantiate (
			FireballPrefab,
			SkillSpawn.position,
			SkillSpawn.rotation);

		// Add velocity to the bullet
		fireball.GetComponent<Rigidbody>().velocity = 
			fireball.transform.right * 8;

		// Destroy the bullet after 2 seconds
		Destroy(fireball, 8.0f);
	}
	void Update () {
		FireballWait += Time.deltaTime;
		if (Input.GetKeyDown (KeyCode.Q) & FireballWait >= FireballInterval) {
			FireballWait = 0;
			ShootFireball();
		}
	}
//    public int damagePerShot = 20;
//    public float timeBetweenBullets = 0.15f;
//    public float range = 100f;
//
//
//    float timer;
//    Ray shootRay = new Ray();
//    RaycastHit shootHit;
//    int shootableMask;
//    ParticleSystem gunParticles;
//    LineRenderer gunLine;
//    AudioSource gunAudio;
//    Light gunLight;
//    float effectsDisplayTime = 0.2f;
//
//
//    void Awake ()
//    {
//        shootableMask = LayerMask.GetMask ("Shootable");
//        gunParticles = GetComponent<ParticleSystem> ();
//        gunLine = GetComponent <LineRenderer> ();
//        gunAudio = GetComponent<AudioSource> ();
//        gunLight = GetComponent<Light> ();
//    }
//
//
//    void Update ()
//    {
//        timer += Time.deltaTime;
//
//		if(Input.GetButton ("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
//        {
//            Shoot ();
//        }
//
//        if(timer >= timeBetweenBullets * effectsDisplayTime)
//        {
//            DisableEffects ();
//        }
//    }
//
//
//    public void DisableEffects ()
//    {
//        gunLine.enabled = false;
//        gunLight.enabled = false;
//    }
//
//
//    void Shoot ()
//    {
//        timer = 0f;
//
//        gunAudio.Play ();
//
//        gunLight.enabled = true;
//
//        gunParticles.Stop ();
//        gunParticles.Play ();
//
//        gunLine.enabled = true;
//        gunLine.SetPosition (0, transform.position);
//
//        shootRay.origin = transform.position;
//        shootRay.direction = transform.forward;
//
//        if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
//        {
//            EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
//            if(enemyHealth != null)
//            {
//                enemyHealth.TakeDamage (damagePerShot, shootHit.point);
//            }
//            gunLine.SetPosition (1, shootHit.point);
//        }
//        else
//        {
//            gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
//        }
//    }
}
