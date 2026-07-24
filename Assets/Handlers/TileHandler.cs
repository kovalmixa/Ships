using UnityEngine;

namespace Assets.Handlers
{
    public class TileHandler : SingletonMonoBehaviour<TileHandler>
    {
        public static string GetHitLayerName(Vector2 position)
        {
            Collider2D hitCollider = Physics2D.OverlapPoint(position);
            int layer = 0;
            if (hitCollider != null)
            {
                GameObject hitObject = hitCollider.gameObject;
                layer = hitObject.layer;
            }
            return LayerMask.LayerToName(layer);
        }
    }
}
