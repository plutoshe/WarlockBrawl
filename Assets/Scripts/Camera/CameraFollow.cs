using UnityEngine;
using System.Collections;
public class CameraFollow : MonoBehaviour
{
//	public Transform target;            // The position that that camera will be following.
//	public float smoothing = 5f;        // The speed with which the camera will be following.
//
//	Vector3 offset;                     // The initial offset from the target.
//
//	void Start ()
//	{
//		// Calculate the initial offset.
//		offset = transform.position - target.position;
//	}
//
//	void FixedUpdate ()
//	{
//		// Create a postion the camera is aiming for based on the offset from the target.
//		Vector3 targetCamPos = target.position + offset;
//
//		// Smoothly interpolate between the camera's current position and it's target position.
//		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
//	}
	public float speed;   
	private Transform target;
//	public Transform limitSpace;

	Vector3 offset;
	public float smoothing = 5f; 
	//关于鼠标滑轮的参数  
//	float MouseWheelSensitivity = 0.0001f;   
//	float MouseZoomMin = -2.4f;   
//	float MouseZoomMax = 1.0f;  
//	float normalDistance = -1.1f;   

	//水平和垂直的移动速度  
	float horizontalMoveSpeed = 0.1f;   
	float verticalMoveSpeed = 0.1f;   
//	float camRayLength = 100;
//	int floorMask;
	int cameraMask;

	void Start() {
		cameraMask = LayerMask.GetMask ("Camera");
	}  

	public void SetTarget(Transform target_) {
		offset = transform.position - target_.position;
		target = target_;
	}

	public void FollowTarget() {
		transform.position = target.position + offset;
	}

	void Update() {
		var msPos = Input.mousePosition;  
		if (Input.GetKeyDown (KeyCode.H)) {
			// Create a postion the camera is aiming for based on the offset from the target.
			FollowTarget();
	
			// Smoothly interpolate between the camera's current position and it's target position.
//			transform.position = targetCamPos;
			//Vector3 (transform.position, targetCamPos, smoothing * Time.deltaTime);
			return;
		}
		//边界最小值  
		var widthBorder = Screen.width/50;   
		var heightBorder = Screen.height/50;   

//		Debug.Log (limitSpace.localScale);
		var x = 0.0f;   
		var y = 0.0f;   

		if (widthBorder <= msPos.x && msPos.x <= Screen.width-widthBorder &&
			   heightBorder <= msPos.y && msPos.y <= Screen.height - heightBorder) {  
//			transform.Translate (x, y, y);   

		} else {  

				// Perform the raycast and if it hits something on the floor layer...
//			if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
//			{
//				
//			}
			if (msPos.y > Screen.height - heightBorder)
				y = verticalMoveSpeed;
			if (msPos.x < widthBorder)
				x = -horizontalMoveSpeed;
			if (msPos.y < heightBorder)
				y = -verticalMoveSpeed;
			if (msPos.x > Screen.width - widthBorder)
				x = horizontalMoveSpeed;

			x *= speed * Time.deltaTime;  
			y *= speed * Time.deltaTime;  
			var movement = new Vector3 (x, 0, y);
			//Debug.Log (x + " " + y);  
			//  
				
//			Debug.Log (transform.position);
//			Debug.Log (z);
//			transform.position += movement;

//			Ray camRay = Camera.main.ScreenPointToRay (new Vector3(Screen.width/2 + x*2,0, Screen.height/2 + y*2));
			 

			Ray camRay = new Ray(transform.position + movement, transform.forward);
			RaycastHit cameraHit;
			Debug.DrawRay(camRay.origin, camRay.direction * 100, Color.yellow);
//			int floorMask = LayerMask ("Floor");

//			Debug.Log (Physics.Raycast (camRay, out cameraHit, 100, floorMask));

			if (Physics.Raycast (camRay, out cameraHit, 1000, cameraMask)) {
				transform.position += movement;
			}
		}

	}
}