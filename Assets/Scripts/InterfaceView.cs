using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceView : MonoBehaviour {

	[SerializeField] private GameObject ButtonsPanel, ShopPanel;

	public void ShowShop(){
		ButtonsPanel.SetActive (false);
		ShopPanel.SetActive (true);
	}

	public void HideShop(){
		ButtonsPanel.SetActive (true);
		ShopPanel.SetActive (false);
	}
}
