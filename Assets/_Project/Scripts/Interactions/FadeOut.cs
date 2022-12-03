using System;
using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Interactions
{
    public class FadeOut : InteractionHandler
    {
        [SerializeField] private Renderer _meshRenderer;
        [SerializeField] private float _duration = 3f;

        private void Awake()
        {
            if (_meshRenderer == null)
                throw new NullReferenceException("Mesh renderer is null");
        }

        public override void Interact() => StartCoroutine(FadeOutCoroutine());

        private IEnumerator FadeOutCoroutine()
        {
            float elapsed = 0f;

            while (elapsed < _duration)
            {
                Color color = _meshRenderer.material.color;
                color.a = Mathf.Lerp(1,0, elapsed / _duration);
                _meshRenderer.material.color = color;
                yield return null;
                elapsed += Time.deltaTime;
            }
        }
    }
}
