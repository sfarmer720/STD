using UnityEngine;
using System.Collections.Generic;

public class STDMath : MonoBehaviour{

	//event system
	public GameObject EventCaller;

	//Seed Variables
	public Seeder seeder;
	public int seed;

	//Time Variable
	public float time;
	public float gravity = 20;

	//Maths
	private Pathfinder pathfinder = new Pathfinder();
	private MatrixMath matrixMath = new MatrixMath();
	private TileMath tileMath = new TileMath ();
	private UnitMath unitMath = new UnitMath();
	private OverlordMath overMath = new OverlordMath();
	private AntiplayerMath antiMath = new AntiplayerMath ();

    //Constant Variables
    public int MAXSPEED = 20;


	// Initialization
	void Start () {

		//define random seed
		seed = seeder.hashedSeed;
		Random.InitState (seed);

		//set seed for classes
		matrixMath.Seed(seed);
		tileMath.SetSeed (seed);
		unitMath.SetSeed (seed);
		overMath.SetSeed (seed);
		antiMath.SetSeed (seed);


		time = Time.time;
	}

	//Update time
	void FixedUpdate(){

		//Update Time to be used throughout game
		time = Time.smoothDeltaTime;

	}


	/* ===================================================================================================================================
	 * 
	 * 									Common Math
	 * 			 				Commonly used math functions
	 *=================================================================================================================================== */

	//WITHIN DEVIANCE FUNCTIONS//
	public bool withinDev(float f1, float f2, float dev){return(f1 <= f2 + dev && f1 >= f2 - dev);}
	public bool withinDev(int i1,int i2, int dev){return(i1 <= i2 + dev && i1 >= i2 - dev);}
	public bool mouseTol(Vector3 orig, Vector3 pos, float tol){
		return(withinDev (orig.x, pos.x, tol) && withinDev (orig.y, pos.y, tol));
	}

	//IN RANGE FUNCTIONS//
	public bool inRange(float f, float min, float max){return(f>min && f<min);}

	//APPLY GRAVITY//
	public Vector3 ApplyGravity(Vector3 v){
		return new Vector3 (v.x, v.y - (gravity * time), v.z);
	}


	/* ===================================================================================================================================
	 * 
	 * 									Generation Math
	 *  					Math functions for prodcedural generation
	 *=================================================================================================================================== */

	//GENERATE PRIMARY MATRIX//
	public int[,] createPrimeMatrix(List<float[,]> fmats, int size, int prime, int sub, float[] adjust, int numBiomes){
		return matrixMath.PrimeMat (fmats, size, prime, sub, adjust, numBiomes);
	}

	//GENERATE RANDOM MATRIX//
	public int[,] createMatrix(float[,] fmat, int size, int prime, int sub, bool primary){
		return matrixMath.CreateMat (fmat, size, prime, sub, primary);
	}

	//MERGE SUPPLE MATRICES//
	public int[,] mergeMats(List<float[,]> fmats, List<int> subs, int size){
		return matrixMath.MergeMats (fmats, subs, size);
	}

	//COMBINE SUPPLE AND PRIME MATRIX//
	public int[,] finalMat(int[,] prime, int[,]supple, int size){
		return matrixMath.SuppleToPrimeMatrix (prime, supple, size);
	}

	//CLEAN UP MATRIX//
	public int[,] cleanMatrix(int[,] mat, int size, int prime){
		return matrixMath.CleanMat (mat, size, prime);
	}

	//COUNT LIVING//
	public int countLiving(int[,] mat, int x, int y, bool countCenter, bool countOutside){
		return matrixMath.CountLive (mat, x, y, countCenter, countOutside);
	}

	//BLUR MATRIX//
	public float[,] Blur(float[,] fmat, int iterations, bool outside){
		return matrixMath.GaussianFilter (fmat, iterations, outside);
	}

	/* ===================================================================================================================================
	 * 
	 * 									Tile Math
	 *  					Math functions for tile functions
	 *=================================================================================================================================== */

	//MODIFY TERRA MAP//
	public float[,] ModifyTerraMap(float[,] terra, Vector2 start, int sqrt, int mtn){
		return tileMath.ModifyTerra (terra, start, sqrt, mtn);
	}
		
	//TERRAFORM TILE//
	public Vector3[] Terraform(Vector3[] verts, Vector2 start, float[,] noise, int sqrt){
		return tileMath.CreateTerra (verts,start,noise, sqrt);
	}

	//RANDOM ASSET LOCATON//
	public Vector3 AssetLoc(List<Vector3> previous, Vector3 tileloc, int width){
		return tileMath.SetLocation (previous, tileloc, width);
	}

	//NEIGHBOR MATRIX//
	public List<Vector2> GetNeighbors(int[,] map, Vector2 loc){
        return tileMath.GetNeighbors(map, loc);
    }

	//VECTOR MATRIX//
	public Vector3[,] VectorMatrix(Vector3[] verts){
		return tileMath.GetVertMat (verts);
	}

	//FOW QUAD//
	public GameObject FOW(float square, bool textureFX, int filterMode, int anisotropicLevel, Material mat){
		return tileMath.FOWQuad (square, textureFX, filterMode, anisotropicLevel, mat);
	}

	/* ===================================================================================================================================
	 * 
	 * 									Unit Math
	 *  					Math functions for Units, includs combat and movement
	 *=================================================================================================================================== */

	//BAKE PATHFINDER
	public void BakePathfinder(int[,] map){
		pathfinder.BakeHeuristics (map);
	}

	//PATHFINDER//
	public List<Vector2> GetTilePath(Vector2 start, Vector2 end, int[,] tileMap, float[]costs, bool diagnolAllowed){
		return pathfinder.Pathfind (start, end, tileMap, costs, diagnolAllowed);
	}

	//SELECTION TO PATH//
	public List<Vector2> SelectToPath(Transform t, Vector2 start, int[,] tileMap, float[]costs, bool diagnolAllowed){
		return pathfinder.Pathfind (start, unitMath.TransformToMap (t), tileMap, costs, diagnolAllowed);
	}

    //MOVEMENT LERP//
    public float MoveLERP(Vector3 init, Vector3 dest, float speed, float start)
    {
        return unitMath.MoveLERP(init, dest, speed, start);
        //return unitMath.MoveLERP(movePos, speed, start);
    }

    //MOVEMENT ROTATION//
    public Quaternion MoveLook(Transform t, Vector3 target)
    {
        return unitMath.MoveLook(t, target);
    }

    //MOVEMENT POSITION//
    public Vector3 MovePos(List<Vector3> movePos, float f)
    {
        return unitMath.MovePos(movePos, f);
    }

    //MOVEMENT VECTOR//
    public Vector3 MoveVec(Vector3 initPos, Vector3 dstPos, float speed, float startTime){
        return unitMath.MoveVec(initPos, dstPos, speed, startTime, time);
    }
	public Vector3 MoveVec( Vector3 forward, float speed){
		return unitMath.MoveDir (forward, speed, time);
	}

	//FOW SIGHT PLANE//
	public GameObject SightPlane(int sight, Material mat){
		return unitMath.CreateSightPlane (sight, mat);
	}

	//MOVEMENT OVER TILE//
	public float SpeedOverTile(float speed, float cost){
		return unitMath.MovementOverTileSpeed (speed, cost);
	}

    //UPDATE HP//
    public int UpdateHP(int currentHP, int currentMax, int newMax){
        return unitMath.UpdateHP(currentHP, currentMax, newMax);
    }

    //IN RANGE//
    public bool UnitInRange(Vector3 v1, Vector2 v2, int range, int tileSize){
        return unitMath.InRange(v1, v2, range, tileSize);
    }

    //ATTACKS PER SECOND//
    public float AttacksPerSecond(float Speed){
        return unitMath.AttacksPerSecond(Speed, MAXSPEED);
    }

    //ATTACK POWER//
    public float AttackPower(int numUnits, float attack){
        return unitMath.AttackPower(numUnits, attack);
    }

    //STRUCTURE FLAW//
    public float StructureFlaw(float structureFlaw){
        return unitMath.StructureFlaw(structureFlaw);
    }

    //KEEP DAMAGE//
    public int KeepDamage(Unit u, Unit e){
        return unitMath.KeepDamage(u, e);
    }
    //BUILDING DAMAGE//
    public int BuildingDamage(Unit u, Unit e){
        return unitMath.BuildingDamage(u, e);
    }
    //UNIT DAMAGE//
    public int UnitDamage(Unit u, Unit e){
        return unitMath.UnitDamage(u, e);
    }
    //TRAP DAMAGE//
    public int TrapDamage(Unit u, Unit e){
        return unitMath.TrapDamage(u, e);
    }

        /* ===================================================================================================================================
         * 
         * 									Overlord Math
         *  					Math functions for Player and AI base class
         *=================================================================================================================================== */
        //CAN AFFORD//
    public bool CanAfford(Overlord o, int cost) {
        return overMath.CanAfford(o, cost);
    }

    //CAN BUILD UNIT//
    public Vector2 CanBuildUnit(Generator map, Vector2 loc){
        return overMath.CanBuildUnit(map, loc);
    }

    //UNIT SPAWN LOCATION//
    public Vector3 SpawnLocation(Vector2 loc, int mapSize){
        return overMath.SpawnLocation(loc, mapSize);
    }

    //DEFENSE DOMAIN//
    public bool[,] EstablishDomain(Vector2 location, Generator map){
		return overMath.SetDefenseDomain (location, map);
	}

	//STARTING TILE//
	public List<Vector2> GenStartingTiles(Generator map, bool[,] domain, int[] ignore){
		return overMath.OffenseStartingTile (map, domain, ignore);
	}


	/* ===================================================================================================================================
	 * 
	 * 									Antiplayer Math
	 *  					Math functions for Player and AI base class
	 *=================================================================================================================================== */

	//Find best keep loction
	public Vector2 BestKeepLocation(Generator map, int[] tileMod, int[] ignore){
		return antiMath.BestKeepLocation (map, tileMod, ignore);
	}
}
