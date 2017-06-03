using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMath{

	//Set Unit Math seed
	public void SetSeed(int seed){
		Random.InitState (seed);
	}

	//Convert Transform into Map Location
	public Vector2 TransformToMap(Transform t){

		if (t.gameObject.layer == 12 || t.gameObject.layer == 13) {
			//if selction was tile
			return t.gameObject.GetComponent<Tile> ().MapLoc;
		} else if (t.gameObject.layer == 10) {
			//selection was enemy
			return t.gameObject.GetComponent<Unit> ().Movement ().CurrentTile ();
		}else {
			return new Vector2 (-1, -1);
		}
	}

    //Get current movement LERP amount
    public float MoveLERP(List<Vector3> movePos, float speed, float start)
    {
       // Debug.Log("Time: " + (Time.time - start) + " | Speed: " + speed + " | Pos 1: " + movePos[0] + " | Pos 2: " + movePos[1] + " | Distance: " + (Vector3.Distance(movePos[0], movePos[1])));
        return ((Time.time-start) * speed) / Vector3.Distance(movePos[0], movePos[1]);
    }
    public float MoveLERP(Vector3 init, Vector3 dest, float speed, float start)
    {
       // Debug.Log("Time: " + (Time.time - start) + " | Speed: " + speed + " | Pos 1: " + init + " | Pos 2: " + dest + " | Distance: " + (Vector3.Distance(init, dest)));
        return ((Time.time - start) * speed) / Vector3.Distance(init, dest);
    }

    //Current unit Lookact Vector
    public Quaternion MoveLook( Transform t, Vector3 target)
    {
        Vector3 relative = target - t.position;
        Quaternion rot = Quaternion.LookRotation(relative);
        return rot;
    }

    //Current unit world position
    public Vector3 MovePos(List<Vector3> movePos, float f)
    {
        //Debug.Log("Init Pos: " + movePos[0] + " | Dst Pos: " + movePos[1] + " | FrameFrac: " + f);
        return Vector3.Lerp(movePos[0], movePos[1], f);
    }



	//Get Move Vector
	public Vector3 MoveDir( Vector3 forward, float speed, float t){
		return forward * speed * t;
	}
    public Vector3 MoveVec(Vector3 initPos,Vector3 dstPos, float speed, float startTime, float t)
    {
        //confirm speed > 0
        if (speed > 0)
        {
            //get distance between points
            float dist = Vector3.Distance(initPos, dstPos);

            //get Frame speed
            float frameSpeed = (t - startTime) * speed;

            //get completion fraction
            float frameFrac = frameSpeed / dist;

            //return LERP
           // Debug.Log("Init Pos: " + initPos + " | Dst Pos: " + dstPos + " | FrameFrac: " + frameFrac);
            return Vector3.Lerp(initPos, dstPos, frameFrac);
        }
        //return if speed is <= 0
        return new Vector3();
    }


	//Create Sight Plane
	public GameObject CreateSightPlane(int sight, Material mat){
		//create new game object with filtyer and renderer
		GameObject go = new GameObject();
		MeshFilter mf = go.AddComponent<MeshFilter> ();
		MeshRenderer mr = go.AddComponent<MeshRenderer> ();

		//create mesh, vertices, uvs, and tris
		Mesh m = new Mesh();
		Vector3[] v = new Vector3[]{
			new Vector3(0,0,0),
			new Vector3(sight,0,0),
			new Vector3(0,0,sight),
			new Vector3(sight,0,sight)
		};

		Vector2[] uvs = new Vector2[v.Length];
		for (int i = 0; i < uvs.Length; i++) {
			uvs [i] = new Vector2 (v [i].x, v [i].z);
		}


		int[] tris = new int[]{0,2,1,2,3,1};

		//assign mesh
		m.vertices = v;
		m.uv = uvs;
		m.triangles = tris;
		mf.mesh = m;
		mr.enabled = true;
		m.RecalculateBounds ();
		m.RecalculateNormals ();

		go.layer = 12;
		mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		mr.receiveShadows = false;
		mr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
		mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
		mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

		//create texture
		mr.material = mat;
		Texture2D tex = SightTex(sight,sight*2);
		mr.material.mainTexture = tex;
		mr.material.mainTextureScale = new Vector2 ((float)1/sight, (float)1/sight);
		tex.Apply ();

		return go;
	}

	//Create sight texture
	public Texture2D SightTex(float r, int res){

		//Initialize texture
		Texture2D tex = new Texture2D (res, res, TextureFormat.RGBA32, true);
		tex.wrapMode = TextureWrapMode.Clamp;

		//Cycle texture
		for (int y = 0; y < res; ++y) {
			for (int x = 0; x < res; ++x) {
	
				//Check if inside radius
				if (InRadius (r, res*0.5f, res*0.5f, x, y)) {

					//point inside radius, color
					tex.SetPixel(x,y, Color.green);

				} else {
					
					//point inside radius, Transparent
					tex.SetPixel(x,y, new Color(0,0,0,0));
				}
			}
		}
			
		return tex;
	}

	private bool InRadius(float r, float x1, float y1, float x2, float y2){
		return(r > (Mathf.Sqrt (Mathf.Pow ((x2 - x1), 2) + Mathf.Pow ((y2 - y1), 2))));
	}


	//Movement over tile speed
	public float MovementOverTileSpeed(float Speed, float tileCost){
		return Mathf.Clamp01 ((Speed * 0.1f) * (tileCost * 0.1f));
	}


    //Update HP
    public int UpdateHP(int currentHP, int currentMax, int newMax)
    {
        //check if Hp is full
        if(currentHP >= currentMax)
        {
            return newMax;
        }
        else
        {
            //Find hp ratio, return floored HP, with min of 1
            float hp = newMax * (currentHP / currentMax);
            return (hp >= 1) ? Mathf.FloorToInt(hp) : 1;
        }        
    }

    //Check if target is in range
    public bool InRange(Vector3 v1, Vector2 v2, int Range, int tileSize)
    {
        //return if distance between points is les than max range
        return (Vector3.Distance(v1, v2) <= tileSize * Range);
    }

    //Get attacks per second
    public float AttacksPerSecond(float Speed, int MaxSpeed)
    {
        //Tiem between attacks
        return Mathf.Abs((Speed - MaxSpeed) * 0.25f);
    }

    //Attack Power
    public float AttackPower(int numUnits, float attack)
    {
        //attack based on the number of living units
        return numUnits * attack;
    }

    //Structure Flaw
    public float StructureFlaw(float structureFlaw)
    {
        //roll random against flaw, if success mul by 10 and return, else return 1
        return (Random.Range(0f, 1f) < structureFlaw) ? structureFlaw * 10 : 1;
    }

    //Minimum Damage return
    private int MinDamage(int dmg)
    {
        //return a minimum of 1 damage
        return (dmg >= 1) ? dmg : 1;
    }

    //Damge to Keep
    public int KeepDamage(Unit u, Unit e)
    {
        //Keep damage is the rounded value of (( attack * siegeMod * siegeResist) - Defense) / structureflaw
        return MinDamage(Mathf.RoundToInt(((AttackPower(u.CurrentStats().numUnits, u.CurrentStats().attack) * u.CurrentStats().siegeMod * e.CurrentStats().siegeMod) - e.CurrentStats().defense) / StructureFlaw(e.CurrentStats().evade)));
    }

    //Damage to Building
    public int BuildingDamage(Unit u, Unit e)
    {
        //Building Damage is the rounded value of ( attack * siegeMod * siegeResist) / structureflaw
        return MinDamage(Mathf.RoundToInt((AttackPower(u.CurrentStats().numUnits, u.CurrentStats().attack) * u.CurrentStats().siegeMod * e.CurrentStats().siegeMod) / StructureFlaw(e.CurrentStats().evade)));
    }

    //Damage to Units
    public int UnitDamage(Unit u, Unit e)
    {
        //unit damage is the rounded value of Attack - Defense
        return MinDamage(Mathf.RoundToInt(AttackPower(u.CurrentStats().numUnits, u.CurrentStats().attack) - e.CurrentStats().defense));
    }

    //Damage from traps
    public int TrapDamage(Unit u, Unit e)
    {
        //trap takes damage then applys damage
        --u.CurrentStats().HP;
        return (int)u.CurrentStats().attack;
    }
}
