using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour {

	[SerializeField] private int id;
	[SerializeField] private string objName;
	[Tooltip("the size of the object on the grid, e.g. object with a size equal 2 takes 2 x 2 cells on the grid ")]
	[SerializeField] private uint size;
	[SerializeField] private GameObject prefab;

	private int GridPositionX,GridPositionZ;

	public string ObjName{
		get{
			return objName;
		}
	}

	public int Id{
		get{
			return id;
		}
	}
	
	public uint Size{
		get{
			return size;
		}
	}

	public GameObject Prefab{
		get{
			return prefab;
		}
		set{
			prefab = value;
		}
	}
	
	public void SetPosition(int x, int z){
		GridPositionX = x;
		GridPositionZ = z;
	}

	public void GetPosition(out int x, out int z){
		x = GridPositionX;
		z = GridPositionZ;
	}

	public  GridObject(GameObject _objectPrefab, int _id, string _name, int _x, int _z, uint _objectSize){
		prefab = _objectPrefab;
		id = _id;
		name = _name;
		GridPositionX = _x;
		GridPositionZ = _z;
		size = _objectSize;
	}


	public override string ToString ()
	{
		return string.Format ("[GridObject: ObjName={0}, Id={1}, Size={2}, Prefab={3}, Position={4},{5}]", ObjName, Id, Size, Prefab, GridPositionX, GridPositionZ);
	}


}
	




