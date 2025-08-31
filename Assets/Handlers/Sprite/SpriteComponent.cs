using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Assets.Handlers.Sprite
{
    public class SpriteComponent : MonoBehaviour
    {
        public Action AnimationFinished;

        private GraphicElement _graphicElement;

        public bool IsLooped = true;

        private float _delay;
        private int _currentFrameIndex;
        private float _globalTimer;
        private float _frameTimer;
        private SpriteRenderer _spriteRenderer;

        public IEnumerator SetGraphicElement(GraphicElement graphicElement)
        {
            if (graphicElement == null) throw new ArgumentNullException(nameof(graphicElement));
            _graphicElement = graphicElement;

            _currentFrameIndex = 0;
            _globalTimer = 0;
            _frameTimer = 0;
            _delay = graphicElement.Delay;

            if (_spriteRenderer == null)
                _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

            StopAllCoroutines();
            yield return StartCoroutine(SetSpriteByIndex(0));
        }
        private IEnumerator SetSpriteByIndex(int i)
        {
            if (_graphicElement == null) yield break;

            yield return StreamingSpriteLoader.LoadSprite(_graphicElement, i, true,
                sprite => { _spriteRenderer.sprite = sprite; });
        }

        private void Update()
        {
            if (_graphicElement == null) return;

            _frameTimer += Time.deltaTime;
            if (_frameTimer >= _delay)
            {
                _globalTimer += _frameTimer;
                _frameTimer = 0;

                _currentFrameIndex = (_currentFrameIndex + 1) % _graphicElement.Quantity;
                StartCoroutine(SetSpriteByIndex(_currentFrameIndex));

                if (_globalTimer >= _graphicElement.Time && !IsLooped)
                {
                    AnimationFinished?.Invoke();
                    enabled = false;
                }
            }
        }
    }
}