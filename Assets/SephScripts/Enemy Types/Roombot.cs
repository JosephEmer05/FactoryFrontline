using System.Collections;
using UnityEngine;

public class Roombot : BaseEnemy
{
    void Start()
    {
        StartCoroutine(BehaviorLoop());
    }

    IEnumerator BehaviorLoop()
    {
        while (true)
        {
            if (!isAttacking)
            {
                Transform tower = FindClosestTower();
                if (tower != null)
                {
                    targetTower = tower;
                    StartCoroutine(AttackTower());
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator AttackTower()
    {
        isAttacking = true;

        while (targetTower != null && Vector3.Distance(transform.position, targetTower.position) <= atkRange)
        {
            TestTower tower = targetTower.GetComponent<TestTower>();
            if (tower != null)
            {
                tower.TakeDamage(atkDmg);
                yield return new WaitForSeconds(atkCooldown);
            }
            else
                targetTower = null;
        }

        isAttacking = false;
        targetTower = null;
    }
}
