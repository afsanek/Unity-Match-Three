using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoint;
    private SpriteRenderer sprite;
    public GoalManager goalManager;
    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        goalManager = GameObject.FindWithTag("GoalManager").GetComponent<GoalManager>();
    }
    public void SetHitPoint(int hitpoint)
    {
        if(hitpoint == 0)
        {
            hitpoint = 1;
        }
        hitPoint = hitpoint;
    }
    private void Update()
    {
        if (hitPoint <= 0)
        {
            Destroy(gameObject);
        }

    }
    public void TakeDamage(int damage)
    {
        hitPoint -= damage;
        MakeLighter();
    }
    private void MakeLighter()
    {
        Color currentColor = sprite.color;
        currentColor.a = currentColor.a * .7f;
        sprite.color = currentColor;
    }
}
