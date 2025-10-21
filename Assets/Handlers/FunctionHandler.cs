using UnityEngine;

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

        public static T GetRandomElementArray<T>(params T[] array)
        {
            if (array.Length == 0) return default;
            int randIndex = Random.Range(0, array.Length);
            return array[randIndex];
        }
    }
}
