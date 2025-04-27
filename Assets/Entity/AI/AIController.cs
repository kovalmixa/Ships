using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class AIController : MonoBehaviour, IEntityController, IAI
{
    private List<List<IScript>> _areaScriptList;
    private List<List<IScript>> _scriptList;
    private IAI _ai;
    //Route
    private int currentRouteNumber = 0;
    private List<Route> _routes = new();
    private Route _currentRoute;
    public void SetAIType(string name) { }
    public void UpdateControl(Entity entity)
    {
        MoveControl(entity);
        RotateControl(entity);
        AttackControl(entity);
    }
    private void AttackControl(Entity entity)
    {
    }
    private void RotateControl(Entity entity)
    {
    }

    private void MoveControl(Entity entity)
    {
        if (_routes.Count == 0)
            return;
        if (_currentRoute == null)
            _currentRoute = _routes[currentRouteNumber];
        var currentScript = _currentRoute.GetCurrentScript();
        if (currentScript is AreaScript point)
        {
            HandleScriptPoint(entity, point);
        }
        else
        {
            List<IScript> areas = _currentRoute.Areas;
            if (areas.Count == 0)
                return;
            AreaScript lastArea = areas[areas.Count - 1] as AreaScript;
            if (lastArea == null)
                return;
            Transform target = null;
            foreach (var script in lastArea.GetScripts())
            {
                if (script is MoveToScript moveToScript && moveToScript.Target != null)
                {
                    target = moveToScript.Target;
                    break;
                }
            }
            if (target != null)
            {
                for (int i = 0; i < areas.Count; i++)
                {
                    if (areas[i] is AreaScript areaScript)
                    {
                        if ((Vector2)areaScript.transform.position == (Vector2)target.position)
                        {
                            _currentRoute.SetIndex(i);
                            currentScript = _currentRoute.GetCurrentScript();
                            break;
                        }
                    }
                }
            }

            if (currentScript == null)
                return;

            bool completed = currentScript.Execute(entity);

            if (completed)
            {
                _currentRoute.MoveNext();
            }
        }
    }

    private void HandleScriptPoint(Entity entity, AreaScript targetPoint)
    {
        Vector2 directionToPoint = targetPoint.Position - (Vector2)entity.transform.position;
        if (directionToPoint.magnitude < targetPoint.Radius)
        {
            targetPoint.Execute(entity);
            _currentRoute.MoveNext();
            return;
        }
        float angleToTarget = Vector2.SignedAngle(entity.transform.up, directionToPoint.normalized);
        float rotationDirection =  -Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
        entity.SpeedLevel = Mathf.Clamp(entity.SpeedLevel + 1, entity.MinSpeedLevel, entity.MaxSpeedLevel);
        entity.Movement(rotationDirection);
    }

    public void SetupRouteScripts(List<GameObject> routeList)
    {
        if (routeList == null) return;
        foreach (GameObject element in routeList)
        {
            Route route = new();
            foreach (Transform area in element.GetComponentsInChildren<Transform>())
            {
                AreaScript areaScript = area.GetComponent<AreaScript>();
                if(areaScript == null) continue;
                route.Areas.Add(areaScript);
            }
            _routes.Add(route);
        }
    }
    public void SetupAreaScripts(GameObject[] scriptAreaSets)
    {
        throw new System.NotImplementedException();
    }
    public void SetupScripts(IScript[] scriptPointSets)
    {
        throw new System.NotImplementedException();
    }
}