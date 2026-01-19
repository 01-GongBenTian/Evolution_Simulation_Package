using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHover : MonoBehaviour
{
    private static float HIGHLIGHT_SIZE = 0.25f;

    [SerializeField] private Renderer _Renderer;
    Material _Material;

    private bool _IsHover = false;
    private float _Progress = 0.0f;

    private void Start()
    {
        _Material = _Renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        if(_IsHover)
        {
            if (Mathf.Approximately(_Progress, 1.0f))
                return;

            _Progress = Mathf.Clamp(_Progress + Time.deltaTime, 0.0f, 1.0f);
            _Material.SetFloat("_HighlightSize", HIGHLIGHT_SIZE * _Progress);
        }
        else
        {
            if (Mathf.Approximately(_Progress, 0.0f))
                return;

            _Progress = Mathf.Clamp(_Progress - Time.deltaTime, 0.0f, 1.0f);
            _Material.SetFloat("_HighlightSize", HIGHLIGHT_SIZE * _Progress);
        }
    }

    public void OnHoverEnter()
    {
        if (_IsHover)
            return;

        _IsHover = true;
    }

    public void OnHoverExit()
    {
        if (!_IsHover)
            return;

        _IsHover = false;
    }
}
