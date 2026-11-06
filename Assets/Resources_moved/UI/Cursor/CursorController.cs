using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Resources_moved.UI.Cursor
{
    public enum CursorType { None, Attack }

    [Serializable]
    public class CursorTypeTexture
    {
        public CursorType type;
        public Texture2D texture;
        public Vector2 offset;
    }

    public class CursorFollowingObject
    {
        public GameObject gameObject;
        public float speed;
        public float delay;
    }

    public class CursorController : SingletonMonoBehaviour<CursorController>
    {
        [SerializeField] private List<CursorTypeTexture> _CursorTextures;

        private Texture2D _CursorTexture;
        private List<CursorFollowingObject> _followingObjects = new List<CursorFollowingObject>();
        private Dictionary<float, Vector2> _followingObjectsPositions = new Dictionary<float, Vector2>();

        private struct CursorSnapshot
        {
            public float time;
            public Vector2 position;
        }

        private Queue<CursorSnapshot> _cursorHistory = new Queue<CursorSnapshot>();
        private const float MaxHistoryDuration = 5f;

        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();
            _followingObjects = new List<CursorFollowingObject>();
            _followingObjectsPositions = new Dictionary<float, Vector2>();
        }

        private void Start()
        {
            SetCursorByType(CursorType.None);
        }

        private void Update()
        {
            Vector2 currentMousePos = Input.mousePosition;
            _cursorHistory.Enqueue(new CursorSnapshot { time = Time.time, position = currentMousePos });

            float oldestAllowedTime = Time.time - MaxHistoryDuration;
            while (_cursorHistory.Count > 0 && _cursorHistory.Peek().time < oldestAllowedTime) _cursorHistory.Dequeue();

            foreach (var obj in _followingObjects)
            {
                if (obj == null || obj.gameObject == null) continue;
                if (!_followingObjectsPositions.ContainsKey(obj.delay)) SetupObjectsPositions(obj);

                Vector2 targetPos = GetPositionAtTime(Time.time - obj.delay);
                _followingObjectsPositions[obj.delay] = targetPos;

                if (obj.speed <= 0) obj.gameObject.transform.position = targetPos;
                else
                {
                    obj.gameObject.transform.position = Vector2.MoveTowards(
                        obj.gameObject.transform.position,
                        targetPos,
                        obj.speed * Time.deltaTime
                    );
                }
            }
        }

        #endregion

        #region API

        public void SetCursorByType(CursorType type)
        {
            var cursorData = _CursorTextures?.FirstOrDefault(x => x.type == type);
            if (cursorData != null) UnityEngine.Cursor.SetCursor(cursorData.texture, cursorData.offset, CursorMode.Auto);
            else UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        #region Cursor following object

        public void AddFollowingObject(CursorFollowingObject cursorFollowingObject)
        {
            if (cursorFollowingObject == null) return;

            if (!_followingObjects.Contains(cursorFollowingObject))
            {
                _followingObjects.Add(cursorFollowingObject);
                SetupObjectsPositions(cursorFollowingObject);
            }
        }

        public void RemoveFollowingObject(CursorFollowingObject cursorFollowingObject)
        {
            if (cursorFollowingObject != null)
            {
                _followingObjects.Remove(cursorFollowingObject);
                CleanupUnusedDelays();
            }
        }

        private void SetupObjectsPositions(CursorFollowingObject cursorFollowingObject)
        {
            if (cursorFollowingObject == null) return;

            if (!_followingObjectsPositions.ContainsKey(cursorFollowingObject.delay))
            {
                Vector2 initialPos = GetPositionAtTime(Time.time - cursorFollowingObject.delay);
                _followingObjectsPositions[cursorFollowingObject.delay] = initialPos;

                if (cursorFollowingObject.gameObject != null) cursorFollowingObject.gameObject.transform.position = initialPos;
            }
        }

        private Vector2 GetPositionAtTime(float targetTime)
        {
            if (_cursorHistory.Count == 0) return Input.mousePosition;
            CursorSnapshot[] snapshots = _cursorHistory.ToArray();
            if (targetTime <= snapshots[0].time) return snapshots[0].position;
            if (targetTime >= snapshots[snapshots.Length - 1].time) return snapshots[snapshots.Length - 1].position;

            for (int i = 0; i < snapshots.Length - 1; i++)
            {
                if (snapshots[i].time <= targetTime && snapshots[i + 1].time >= targetTime)
                {
                    float t = (targetTime - snapshots[i].time) / (snapshots[i + 1].time - snapshots[i].time);
                    return Vector2.Lerp(snapshots[i].position, snapshots[i + 1].position, t);
                }
            }

            return Input.mousePosition;
        }

        private void CleanupUnusedDelays()
        {
            var activeDelays = new HashSet<float>(_followingObjects.Select(o => o.delay));
            var keysToRemove = _followingObjectsPositions.Keys.Where(k => !activeDelays.Contains(k)).ToList();
            foreach (var key in keysToRemove) _followingObjectsPositions.Remove(key);
        }

        #endregion

        #endregion
    }
}