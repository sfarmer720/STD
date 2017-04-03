using UnityEngine;
using System.Collections;

public class Assets : MonoBehaviour {

	//block assets
	public GameObject[] blocks;

	//Tile Assets
	public GameObject[] tileAssets;

	//Defense Assets
	public GameObject[] defenseAssets;

	//Offense assets
	public GameObject[] offenseAssets;
	public Material[] offenseMaterials;
	public Material[] offenseMaterialsOutlined;

	//materials
	//public Material[] materials;
	//public Material[] materialsOutlined;


	public Material[] terraMaterials;

	//public Material[] terraMaterialsOutlined;
	public Material FOW;
	public Material Sight;
	public Color[] terraColors;

	/*
	 * 0, 153, 0, 255		- Grass
	 * 231, 210, 156, 255	- Desert
	 * 8, 6, 6, 255			- Swamp
	 * 36, 68, 5, 255		- Forest
	 * 26, 110, 152, 255	- River
	 * 131, 106, 68, 255	- Mountain
	 * 3, 0, 73, 255		- Sea
	 */ 


	//UPGRADE ASSETS//
	public GameObject upgradeObject;
	public Upgrades[] offenseUpgrades;
	public Upgrades[] defenseUpgrades;

	//Pull assets
	public GameObject[] GetAssets(int first, int last, int assetType){

		//establish new array
		GameObject[] a = new GameObject[0];
		GameObject[] ia = new GameObject[last - first + 1];

		//select array
		switch(assetType){
		case 0:
			a = tileAssets;
			break;
		case 1:
			a = defenseAssets;
			break;
		case 2: 
			a= offenseAssets;
			break;
		}
	
		//add assets to return array
		for(int i =0;first==last;++first,++i){
			ia[i] = a[first];
		}

		//return array
		return ia;
	}

	//TILE ASSETS AND MATERIALS
	public Material GetTileMat(int i){
		return terraMaterials [i];
	}
//	public Material[] GetTileMaterials(bool outlineMats){
//		if(outlineMats){
//			return terraMaterialsOutlined;
//		}else{
//			return terraMaterials;
//		}
//	}
		

	public GameObject[] GetTileAssets(string tileType){
		GameObject[] ret = { };
		switch(tileType){
		case "Forest":
			GameObject[] fores = { tileAssets[0], tileAssets[1], tileAssets[2]};
			return fores;
		case "Swamp":
			GameObject[] swa = { tileAssets[3], tileAssets[4], tileAssets[5], tileAssets[6]};
			return swa;
		}
		return ret;
	}


	//Pull Upgrade Assets//
	public Upgrades[] GetUpgradeAssets(bool defense){
		return (defense) ? defenseUpgrades : offenseUpgrades;
	}
}
