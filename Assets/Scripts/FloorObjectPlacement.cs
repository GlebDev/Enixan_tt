using UnityEngine;
using System.Collections;
using System;

public class FloorObjectPlacement : MonoBehaviour {

	[Tooltip("The prefab to place")]
	[SerializeField] private GridObject placementObject;
	[Tooltip("Placement success condition, e.g. a green transparent cube")]
	[SerializeField] private GameObject prefabOK;
	[Tooltip("Placement fail condition, e.g. a red transparent cube")]
	[SerializeField] private  GameObject prefabFail;
	[SerializeField] private Material LineMaterial;

	[SerializeField] private Transform GridLinesParent;
	[SerializeField] private Transform GridObjectsParent;

	[Tooltip("Grid resolution, e.g. for 4x4 place grid 2 would allow 2x2 object to be placed")]
	public float grid = 2.0f;

	// Mask of the ground to hit
	[Tooltip("Which layer to use for raycast target")]
	public LayerMask mask = -1;

	// Store which spaces are in use
	private int[,] usedSpace;
	private Vector3 halfSlots;

	private GameObject CurrentDrawnPlacementObject = null;
	private GameObject[,] areaObject = null;

	private Bounds placementBounds;
	private Vector3 lastPos;

	public GridObject PlacementObject{
		get{
			return placementObject;
		}
		set{
			placementObject = value;
		}
	}

	public System.Action<GridObject> OnCreateObject;

	// Use this for initialization
	void Awake () {

		// Check terrain first because it has usually no Renderer
		if (GetComponent<Terrain>() != null){
			placementBounds = GetComponent<Terrain>().terrainData.bounds;
			halfSlots = Vector3.zero;
		} else if (GetComponent<Renderer>() != null){
			placementBounds = GetComponent<Renderer>().bounds;
			halfSlots = placementBounds.size / 2f;
		}
		Vector3 slots = placementBounds.size / grid;
		usedSpace = new int[Mathf.CeilToInt(slots.x), Mathf.CeilToInt(slots.z)];
		for(var x = 0; x < Mathf.CeilToInt(slots.x); x++){
			for (var z = 0; z < Mathf.CeilToInt(slots.z); z++){
				usedSpace[x, z] = 0;
			}
		}
		DrawGrid (0.1f, 0.1f, GridLinesParent);
	}

	// Update is called once per frame
	void Update () {
		Vector3 point;
		// Check for mouse ray collision with this object
		if (placementObject != null && getTargetLocation( out point) ){
			// Transform position is the center point of this object, x and z are grid slots from 0..slots-1
			int x = (int)Math.Round(Math.Round(point.x - transform.position.x + halfSlots.x - grid / 2.0f) / grid);
			int z = (int)Math.Round(Math.Round(point.z - transform.position.z + halfSlots.z - grid / 2.0f) / grid);

			// Re-instantiate only when the slot has changed or the object not instantiated at all
			if (lastPos.x != x || lastPos.z != z || areaObject == null){
				lastPos.x = x;
				lastPos.z = z;
				if (areaObject != null){
					foreach(GameObject value in areaObject){
						Destroy(value);
					}
				}
				areaObject = new GameObject[placementObject.Size, placementObject.Size];
				bool checkResult = IsGridPlaceSuitable (x, z, placementObject.Size);

				// Create or move the object
				DrawObjectPrototipeOnGrid (x, z, placementObject.Size, checkResult ? prefabOK : prefabFail, placementObject.Prefab);

			}

			// On left click, insert the object to the area and mark it as "used"
			if (Input.GetMouseButtonDown(0) ){
				// Place the object
				if(IsGridPlaceSuitable (x, z, placementObject.Size)){
					GridObject newObj = CreateObjectOnGrid (x, z, (uint)placementObject.Size, placementObject, GridObjectsParent);
					if (OnCreateObject != null) {
						OnCreateObject (newObj);
					}
					ClearPlacementObject ();
				}
			}
		}else{
			if (CurrentDrawnPlacementObject){
				Destroy(CurrentDrawnPlacementObject);
			}
			if(areaObject != null){
				foreach(GameObject value in areaObject){
					Destroy(value);
				}
				areaObject = null;
			}
		}

		if(Input.GetKeyDown(KeyCode.Escape)){
			ClearPlacementObject ();
		}
	}

	private bool getTargetLocation(out Vector3 point){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hitInfo = new RaycastHit();
		if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, mask)){
			if (hitInfo.collider == GetComponent<Collider>()){
				point = hitInfo.point;
				return true;
			}
		}
		point = Vector3.zero;
		return false;
	}

	public bool IsGridPlaceSuitable(int x, int z, uint range){
		for(int i = 0; i < range; i++){
			for(int j = 0; j < range; j++){
				if(x + i > usedSpace.GetUpperBound(0) || z + j > usedSpace.GetUpperBound(1) || usedSpace[x + i, z + j] != 0){
					return false;
				}
			}
		}
		return true;
	}

	private void DrawObjectPrototipeOnGrid(int x, int z, uint range, GameObject objAreaprefab, GameObject obj){
		Vector3 point = Vector3.zero;

		// Calculate the quantized world coordinates on where to actually place the object
		point.x = (float)(x) * grid - halfSlots.x + transform.position.x + grid / 2.0f;
		point.z = (float)(z) * grid - halfSlots.z + transform.position.z + grid / 2.0f;

		for(int i = 0; i < range; i++){
			for(int j = 0; j < range; j++){
				areaObject[i,j] = (GameObject)Instantiate(objAreaprefab, new Vector3(point.x  + i * grid, 0, point.z + j * grid), Quaternion.identity);
			}
		}
		if (!CurrentDrawnPlacementObject){
			CurrentDrawnPlacementObject = Instantiate (obj, new Vector3 (point.x + (placementObject.Size - 1) * grid / 2f, 0, point.z + (placementObject.Size - 1) * grid / 2f), Quaternion.identity).gameObject;
		}else{
			CurrentDrawnPlacementObject.transform.position = new Vector3(point.x + (placementObject.Size-1) * grid / 2f, 0, point.z + (placementObject.Size-1) * grid / 2f);
		}
	}

	public GridObject CreateObjectOnGrid(int x, int z, uint range, GridObject obj, Transform parent){
		Vector3 point = Vector3.zero;

		// Calculate the quantized world coordinates on where to actually place the object
		point.x = (float)(x) * grid - halfSlots.x + transform.position.x + grid / 2.0f;
		point.z = (float)(z) * grid - halfSlots.z + transform.position.z + grid / 2.0f;

		for(int i = 0; i < range; i++){
			for(int j = 0; j < range; j++){
				usedSpace[x + i, z + j] = 1;
			}
		}
		// ToDo: place the result somewhere..
		GridObject NewObj = (GridObject)Instantiate(obj, new Vector3 (point.x + (obj.Size - 1) * grid / 2f, 0, point.z + (obj.Size- 1) * grid / 2f), Quaternion.identity, parent);
		NewObj.SetPosition (x, z);
		return NewObj;
	}

	public void ClearPlacementObject(){
		placementObject = null;
	}

	public void DrawGrid(float height, float width, Transform parent){
		Vector3 point = Vector3.zero;
		point.x = transform.position.x - halfSlots.x;
		point.z = transform.position.z - halfSlots.z;
		for(int i = 0; i <= (usedSpace.GetUpperBound (1) + 1); i++){
			GameObject curLine = new GameObject();
			curLine.transform.SetParent (parent);
			curLine.transform.position = Vector3.zero;
			curLine.AddComponent<LineRenderer>();
			LineRenderer lr = curLine.GetComponent<LineRenderer>();
			lr.receiveShadows = false;
			lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			lr.material = LineMaterial;
			lr.endWidth = width;
			lr.startWidth = width;
			lr.SetPosition(0, new Vector3 (point.x, height, point.z + grid * i));
			lr.SetPosition(1, new Vector3 (point.x + (usedSpace.GetUpperBound (0) + 1) * grid, height, point.z + grid * i));
		}
		for(int i = 0; i <= (usedSpace.GetUpperBound (0) + 1); i++){
			GameObject curLine = new GameObject();
			curLine.transform.SetParent (parent);
			curLine.transform.position = Vector3.zero;
			curLine.AddComponent<LineRenderer>();
			LineRenderer lr = curLine.GetComponent<LineRenderer>();
			lr.receiveShadows = false;
			lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			lr.material = LineMaterial;
			lr.endWidth = width;
			lr.startWidth = width;
			lr.SetPosition(0, new Vector3 (point.x + grid * i, height, point.z ));
			lr.SetPosition(1, new Vector3 (point.x + grid * i, height, point.z + (usedSpace.GetUpperBound (1) + 1)* grid));
		}
	}

}