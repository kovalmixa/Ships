using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Handlers
{
    public static class FunctionHandler
    {
        public static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;
            return angle;
        }
        public static bool IsAngleWithinSector(float angle, float min, float max) => angle >= min && angle <= max;
    }
}
