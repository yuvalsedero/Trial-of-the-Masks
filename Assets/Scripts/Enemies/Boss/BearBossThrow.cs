using UnityEngine;

public class BearBossThrow : MonoBehaviour
{
    public GameObject stonePrefab;
    public Transform throwPoint;
    public float throwCooldown = 3f;

    float lastThrowTime;

    public void TryThrow(Vector2 target)
    {
        if (Time.time < lastThrowTime + throwCooldown)
            return;

        Vector2 dir = (target - (Vector2)throwPoint.position).normalized;

        GameObject stone = Instantiate(
            stonePrefab,
            throwPoint.position,
            Quaternion.identity
        );

        StoneProjectile proj = stone.GetComponent<StoneProjectile>();
        if (proj != null)
            proj.Launch(dir);

        lastThrowTime = Time.time;
    }
}
