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

        private GraphicElement graphicElement;

        public bool IsLooped = true;

        private float delay;
        private int currentFrameIndex;
        private float globalTimer;
        private float frameTimer;
        private SpriteRenderer spriteRenderer;

        public IEnumerator SetGraphicElement(GraphicElement graphicElement)
        {
            if (graphicElement == null) throw new ArgumentNullException(nameof(graphicElement));
            graphicElement = graphicElement;

            currentFrameIndex = 0;
            globalTimer = 0;
            frameTimer = 0;
            delay = graphicElement.Delay;

            if (spriteRenderer == null)
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

            StopAllCoroutines();
            yield return StartCoroutine(SetSpriteByIndex(0));
        }

        private IEnumerator SetSpriteByIndex(int i)
        {
            if (graphicElement == null) yield break;

            yield return StreamingSpriteLoader.LoadSprite(graphicElement, i, true,
                sprite => { spriteRenderer.sprite = sprite; });
        }

        private void Update()
        {
            if (graphicElement == null) return;

            frameTimer += Time.deltaTime;
            if (frameTimer >= delay)
            {
                globalTimer += frameTimer;
                frameTimer = 0;

                currentFrameIndex = (currentFrameIndex + 1) % graphicElement.Quantity;
                StartCoroutine(SetSpriteByIndex(currentFrameIndex));

                if (globalTimer >= graphicElement.Time && !IsLooped)
                {
                    AnimationFinished?.Invoke();
                    enabled = false;
                }
            }
        }
    }
}