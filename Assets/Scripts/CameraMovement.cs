using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement: MonoBehaviour{
	[SerializeField] private float speed = 0.1f;
	[SerializeField] private float maxX, minX, maxY, minY, maxZ, minZ;

	private Vector3 oldPosition;
	private Vector2 oldPosition_M;

	void LateUpdate() {
		#if UNITY_STANDALONE || UNITY_WEBPLAYER
		if(Input.GetMouseButton(2)){
			if(Input.GetMouseButtonDown(2)){
				oldPosition = Input.mousePosition;
			}else if(oldPosition != Input.mousePosition){
				Vector3 deltaPosition = oldPosition - Input.mousePosition;
				oldPosition = Input.mousePosition;
				Vector3 vector = new Vector3 (deltaPosition.x, 0, deltaPosition.y) * speed * Time.deltaTime ;
				if((transform.position + vector).x > minX && (transform.position + vector).x < maxX && (transform.position + vector).z > minZ && (transform.position + vector).z < maxZ ){
					transform.Translate(vector, Space.World);
				}
			}

		}
		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
			Vector3 vector = (Input.GetAxis("Mouse ScrollWheel") > 0 ? Vector3.down : Vector3.up) * speed * Time.deltaTime * 40;
			if((transform.position + vector).y > minY && (transform.position + vector).y < maxY){
				transform.Translate(vector , Space.World);
			}
		}
		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
			Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;
			transform.Translate(new Vector3(deltaPosition.x * speed * Time.deltaTime, 0, deltaPosition.y * speed * Time.deltaTime), Space.World);
		}
		if(Input.touchCount > 0){
			if(Input.GetTouch(0).phase == TouchPhase.Began){
				oldPosition_M = Input.GetTouch(0).position;
			}else if(oldPosition_M != Input.GetTouch(0).position){
				Vector2 deltaPosition = oldPosition_M - Input.GetTouch(0).position;
				oldPosition = Input.mousePosition;
				Vector3 vector = new Vector3 (deltaPosition.x, 0, deltaPosition.y) * speed * Time.deltaTime ;
				if((transform.position - vector).x > minX && (transform.position + vector).x < maxX && (transform.position + vector).z > minZ && (transform.position + vector).z < maxZ ){
					transform.Translate(-vector, Space.World);
				}
			}

		}
		#endif

	}
}
