using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Handlers
{
    public static class FunctionHandler
    {
        public static Vector3 GetAngleDistancePoint(Vector2 startPosition, float angle, float distance)
        {
            float angleRadians = angle * Mathf.Deg2Rad;
            float newX = startPosition.x + distance * Mathf.Cos(angleRadians);
            float newY = startPosition.y + distance * Mathf.Sin(angleRadians);
            return new Vector3(newX, newY, 0);
        }
    }
}
