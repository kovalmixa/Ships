using Assets.Entity;
using UnityEngine;

public class WaitScript : MonoBehaviour, IScript
{
    private float waitTime;
    private float timer = 0f;

    public WaitScript(float waitTime)
    {
        this.waitTime = waitTime;
    }

    public bool Execute(Entity entity)
    {
        timer += Time.deltaTime;
        return timer >= waitTime;
    }
}