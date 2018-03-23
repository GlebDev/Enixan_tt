using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	[SerializeField] private FloorObjectPlacement floorGrid;
	[SerializeField] private GameObject DrawnGridParent;

	[SerializeField] private List<CreatableGridObject> LocatedObjects;
	[SerializeField] private Transform LocatedObjectsParent;

	[System.Serializable]
	class CreatableGridObject{
		public GridObject prefab;
		public int x,z;

	}
	// Use this for initialization
	void Start () {
		floorGrid.OnCreateObject += floorGrid_OnCreateObject;
		foreach(CreatableGridObject obj in LocatedObjects){
			if (floorGrid.IsGridPlaceSuitable (obj.x, obj.z, obj.prefab.Size)) {
				floorGrid.CreateObjectOnGrid (obj.x, obj.z, obj.prefab.Size, obj.prefab, LocatedObjectsParent);
			}
		}
	}

	void Update() {
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hitInfo = new RaycastHit ();
			if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity)) {
				if (hitInfo.collider && hitInfo.transform.GetComponent<GridObject>() != null) {
					GridObject obj = hitInfo.transform.GetComponent<GridObject>();
					Debug.Log(obj.ToString());
				}
			}
		}
	}

	public void ToogleDrawnGrid(){
		DrawnGridParent.SetActive (!DrawnGridParent.activeSelf);
	}

	public void SetPlacementObject(GridObject obj){
		floorGrid.PlacementObject = obj;
	}

	private void floorGrid_OnCreateObject(GridObject obj){
		CreatableGridObject newObj = new CreatableGridObject ();
		newObj.prefab = obj;
		obj.GetPosition (out newObj.x, out newObj.z);
		LocatedObjects.Add (newObj);
	}
		


}
