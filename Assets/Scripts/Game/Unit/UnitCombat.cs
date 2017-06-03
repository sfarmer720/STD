using UnityEngine;
using System.Collections;

public class UnitCombat : MonoBehaviour
{
    //Cross Class
    private STDMath stdMath;
    private Generator map;
    private Unit unit;

    //Enemy Variables
    public Unit enemy;

    //Combat Variables
    public float attacksPerSecond;
    public float timeSinceLastAttack;

    //Booleans
    public bool isTrap = false;
    public bool InCombat = false;


    //Initialize
    public void Init(STDMath std, Generator gen, Unit u)
    {
        stdMath = std;
        map = gen;
        unit = u;
    }

    //Update
    public void CombatUpdate()
    {
        //Check if valid target
        TargetVisible();

        //Check if attacking
        Attack();
    }


    //Attempt to attack enemy
    public void Attack()
    {
        //check if unit has enemy
        if(enemy != null)
        {
            //check if enemy is currently alive
            if(enemy.CurrentStats().HP > 0)
            {
                //check if enemy is within range
                if (EnemyInRange())
                {
                    //Check if attack is possible
                    if (!isTrap && CanAttack())
                    {
                        //apply damage based on type of unit
                        if (enemy.CurrentStats().isBuilding)
                        {
                            //is building a Keep
                            if (enemy.CurrentStats().isKeep)
                            {
                                //apply keep damage
                                enemy.Damage(stdMath.KeepDamage(unit, enemy), 1);
                            }
                            else
                            {
                                //apply building damage
                                enemy.Damage(stdMath.BuildingDamage(unit, enemy), 1);
                            }
                        }
                        else
                        {
                            //apply unit damage
                            enemy.Damage(stdMath.UnitDamage(unit, enemy), 0);
                        }
                    }
                    else
                    {
                        //apply trap Damage
                        enemy.Damage(stdMath.TrapDamage(unit, enemy), 2);
                    }
                }
            }
        }
    }

    //Target Enemy
    public void TargetEnemy(Unit e)
    {
        enemy = e;
    }

    //check if target still visible
    public void TargetVisible()
    {
        //confirm active enemy unit
        if (enemy != null)
        {
            //is enemy building or unit
            if (enemy.CurrentStats().isBuilding)
            {
                //building must be alive in visible or visited tile
                if(!(enemy.CurrentStats().HP > 0 && (enemy.currentTile.visible || enemy.currentTile.visited)))
                {
                    enemy = null;
                }

            }
            else
            {
                //enemy is a unit, must be in visible tile
                if (!enemy.currentTile.visible)
                {
                    enemy = null;
                }
            }
        }
    }

    //Reset Attack timer
    public void ResetAttackTime()
    {
        timeSinceLastAttack = Time.time;
    }

    //Check if enemy is in range
    public bool EnemyInRange()
    {
        return stdMath.UnitInRange(this.gameObject.transform.position, enemy.gameObject.transform.position, unit.CurrentStats().range, map.tileWidth);
    }

    //Check if unit can attack
    public bool CanAttack()
    {
        //check if starting combat
        if (!InCombat)
        {
            //check if unit is moving
            if (unit.Movement().IsMoving())
            {
                //Unit is moving, stop all movements
                unit.Movement().StopMovement();
            }

            //determine attacks per second and reset timer
            attacksPerSecond = stdMath.AttacksPerSecond(unit.Movement().Speed());
            ResetAttackTime();

            //return true
            InCombat = true;
            return true;
        }
        else
        {
            //check if unit has been moved since attack
            if (unit.Movement().IsMoving())
            {
                //unit has been moved, end combat
                InCombat = false;
                timeSinceLastAttack = attacksPerSecond = 0;
                return false;
            }
            else
            {
                //Unit already in combat, check attack timer
                if (Time.time - timeSinceLastAttack >= attacksPerSecond)
                {
                    //recalculate attacks per second, and return true
                    attacksPerSecond = stdMath.AttacksPerSecond(unit.Movement().Speed());
                    ResetAttackTime();
                    return true;
                }
                //unit not ready to attack
                return false;
            }
        }
    }

}
