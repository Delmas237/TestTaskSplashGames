using System.Collections.Generic;
using UnityEngine;

public abstract class PlaceableObject : MonoBehaviour, IPlaceable
{
    [SerializeField] private LayerMask _allowedLayers;
    [SerializeField] private List<Renderer> _renderers;
    [Space(10)]
    [SerializeField] protected bool _canBePlacedOnSameType;

    private List<Color> _colors = new List<Color>();
    private Collider _collider;

    public bool CanBePlacedOnSameType => _canBePlacedOnSameType;
    public Collider Collider => _collider;

    public virtual void Initialize()
    {
        foreach (var renderer in _renderers)
        {
            renderer.material.SetInt("_ZWrite", 1);
            _colors.Add(renderer.material.color);
        }

        _collider = GetComponent<Collider>();
    }

    public void SetColor(Color color)
    {
        foreach (var renderer in _renderers)
            renderer.material.color = color;
    }
    public void SetDefaultColors()
    {
        for (int i = 0; i < _renderers.Count; i++)
        {
            _renderers[i].material.color = _colors[i];
        }
    }

    public bool HasSameType(GameObject gameObject)
    {
        return gameObject.TryGetComponent(out IPlaceable placeable) && GetType() == placeable.GetType();
    }

    public bool IsAllowedAndCanBePlacedOn(RaycastHit hit)
    {
        return CanBePlacedOn(hit) && IsSurfaceAllowed(hit);
    }

    protected bool IsSurfaceAllowed(RaycastHit hit)
    {
        return (_allowedLayers.value & (1 << hit.collider.gameObject.layer)) > 0 || (HasSameType(hit.collider.gameObject) && _canBePlacedOnSameType);
    }

    public abstract bool CanBePlacedOn(RaycastHit hit);
}
