using UnityEngine;
using System.Collections;

public class RotateScript : MonoBehaviour {

	public GameObject cam;

	bool leftClick;
	bool rightClick;
	
	float origCamX;
	float origCamY;
	float origCamZ;
	
	float origX;
	float origY;
	
	float timeToDisplayMode;
	
	float timeStart;
	
	void Start(){
		leftClick = false;
		rightClick = false;
		origCamX = cam.transform.position.x;
		origCamY = cam.transform.position.y;
		origCamZ = cam.transform.position.z;
		timeToDisplayMode = 5;
		timeStart = Time.time;
	}

	void Update() {
		bool flag = false;
		if (Input.GetAxis("Fire1") != 0){
			flag = true;
			if (!leftClick){
				leftClick = true;
				origX = Input.GetAxis("Mouse X");
				origY = Input.GetAxis("Mouse Y");
			} else {
				float nowX = Input.GetAxis("Mouse X");
				float nowY = Input.GetAxis("Mouse Y");
				float xDiff = nowX - origX;
				float yDiff = nowY - origY;
				this.transform.eulerAngles += new Vector3(yDiff * 2, xDiff * 2, 0);
				Debug.Log(this.transform.eulerAngles);
			}
		} else {
			leftClick = false;
		}
		if (Input.GetAxis("Fire2") != 0){
			flag = true;
			if (!rightClick){
				rightClick = true;
				origX = Input.GetAxis("Mouse X");
				origY = Input.GetAxis("Mouse Y");
			} else {
				float nowX = Input.GetAxis("Mouse X");
				float nowY = Input.GetAxis("Mouse Y");
				float xDiff = nowX - origCamX;
				float yDiff = nowY - origCamY;
				cam.transform.position += new Vector3(-xDiff * 2, -yDiff * 2, 0);
			}
		} else {
			rightClick = false;
		}
		float zoomDiff = Input.GetAxis("Mouse ScrollWheel");
		if (zoomDiff != 0){
			flag = true;
			cam.transform.position += new Vector3(0, 0, zoomDiff * 10);
		}
		if (Input.GetAxis("Fire3") != 0){
			flag = true;
			cam.transform.position = new Vector3(origCamX, origCamY, origCamZ);
			this.transform.eulerAngles = Vector3.zero;
		}
		if (flag){
			timeStart = Time.time;
		} else if (Time.time - timeStart >= timeToDisplayMode){
			this.transform.eulerAngles += Vector3.up;
		}
	}
	
	void OnGUI(){
		int s_width = Screen.width;
		int s_height = Screen.height;
		
		GUI.BeginGroup(new Rect(50, s_height - 180, 200, 120));
		GUI.Label(new Rect(0, 0, 100, 20), "Vogue-Like");
		GUI.Label(new Rect(0, 20, 200, 20), "By Kevin Sun");
		GUI.Label(new Rect(0, 40, 200, 20), "Left-click to rotate");
		GUI.Label(new Rect(0, 60, 250, 20), "Right-click to move up or down");
		GUI.Label(new Rect(0, 80, 200, 20), "Mouse wheel to zoom");
		GUI.Label(new Rect(0, 100, 200, 20), "Press R to reset");
		GUI.EndGroup();
	}
}
