using System.Collections.Generic;
using Assets.Entity;
using UnityEngine;

public class AreaScript : MonoBehaviour, IScript
{
    [SerializeField] public List<MonoBehaviour> Scripts;
    private List<IScript> _scripts = new();

    public List<IScript> GetScripts() => _scripts;
    public Vector2 Position => transform.position;
    public float Radius = 0.5f;

    private void Awake()
    {
        foreach (MonoBehaviour script in Scripts)
        {
            if (script is IScript s)
                _scripts.Add(s);
            else
                Debug.LogWarning("Assigned scriptBehaviour does not implement IScript!");
        }
    }
    public bool Execute(Entity entity)
    {
        if (_scripts.Count == 0)
        {
            Debug.LogWarning("Script list is empty!");
            return false;
        }
        foreach (IScript _script in _scripts)
        {
            if (!_script.Execute(entity))
                return false;
        }
        return true;
    }
}