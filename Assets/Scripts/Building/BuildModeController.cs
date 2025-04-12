using UnityEngine;

public class BuildModeController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0)] private float _maxDistance = 5f;
    [SerializeField, Range(0, 360f)] private float _rotationAngle = 45f;
    [SerializeField, Min(0)] private float _handsPosition = 2f;
    [SerializeField, Min(1)] private int _validFramesThreshold = 3;
    private int _validFramesCounter = 0;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask _buildSurfaceMask;
    [SerializeField] private LayerMask _obstacleMask;

    [Header("Colors")]
    [SerializeField] private Color _validColor;
    [SerializeField] private Color _invalidColor;

    private PlaceableObject _current;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (_current == null)
            return;

        HandlePosition();
        HandleRotation();

        if (Input.GetMouseButtonDown(0))
            TryPlaceObject();
    }

    /// <summary>
    /// Starts build mode by creating a preview of the selected object.
    /// </summary>
    public void EnterBuildMode(PlaceableObject prefab)
    {
        if (_current != null)
            return;

        _current = Instantiate(prefab);
        _current.Initialize();
        _current.SetColor(_invalidColor);
        _current.Collider.isTrigger = true;
        _validFramesCounter = 0;
    }

    /// <summary>
    /// Updates the preview object's position based on the hit surface.
    /// </summary>
    private void HandlePosition()
    {
        Ray ray = GetCenterScreenRay();
        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _buildSurfaceMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 pos = hit.point;
            Vector3 size = _current.Collider.bounds.size;
            float offset = 0.5f * (size.x * Mathf.Abs(hit.normal.x) +
                                   size.y * Mathf.Abs(hit.normal.y) +
                                   size.z * Mathf.Abs(hit.normal.z));

            if (_current.HasSameType(hit.collider.gameObject) && _current.CanBePlacedOnSameType)
            {
                if (Vector3.Dot(hit.normal, Vector3.up) > 0.9f)
                    offset = size.y * 0.5f;
            }
            pos += hit.normal * offset;

            if (_current.CanBePlacedOn(hit))
                _current.transform.position = pos;
            else
                _current.transform.position = GetCenterScreenRay().GetPoint(Mathf.Min(hit.distance, _handsPosition));

            bool areaClear = IsPlacementAreaClear();
            bool allowedAndClearPlace = _current.IsAllowedAndCanBePlacedOn(hit) && areaClear;

            _validFramesCounter = allowedAndClearPlace ? _validFramesCounter + 1 : 0;

            bool stableValid = _validFramesCounter >= _validFramesThreshold;
            _current.SetColor(stableValid ? _validColor : _invalidColor);
        }
        else
        {
            _current.transform.position = GetCenterScreenRay().GetPoint(_handsPosition);
            _current.SetColor(_invalidColor);
            _validFramesCounter = 0;
        }
    }

    /// <summary>
    /// Rotates the preview object using the mouse scroll wheel.
    /// </summary>
    private void HandleRotation()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float angle = scroll > 0 ? _rotationAngle : -_rotationAngle;
            _current.transform.rotation *= Quaternion.Euler(0, angle, 0);
        }
    }

    /// <summary>
    /// Attempts to place the object if all placement conditions are met.
    /// </summary>
    private void TryPlaceObject()
    {
        if (Physics.Raycast(GetCenterScreenRay(), out RaycastHit hit, _maxDistance, _buildSurfaceMask, QueryTriggerInteraction.Ignore)
            && _current.IsAllowedAndCanBePlacedOn(hit) && IsPlacementAreaClear() && _validFramesCounter >= _validFramesThreshold)
        {
            _current.SetDefaultColors();
            _current.Collider.isTrigger = false;
            _current = null;
        }
    }

    /// <summary>
    /// Checks whether the area where the preview object is located is free of obstacles.
    /// </summary>
    /// <returns>True if the placement area is clear.</returns>
    private bool IsPlacementAreaClear()
    {
        if (_current == null)
            return false;

        Collider currentCollider = _current.Collider;
        Bounds currentBounds = currentCollider.bounds;

        Collider[] overlaps = Physics.OverlapBox(currentBounds.center, currentBounds.extents, _current.transform.rotation, _obstacleMask, QueryTriggerInteraction.Ignore);

        const float allowedPenetration = 0.01f;

        foreach (Collider overlap in overlaps)
        {
            if (overlap.gameObject == _current.gameObject)
                continue;

            if (Physics.ComputePenetration(currentCollider, _current.transform.position, _current.transform.rotation, 
                overlap, overlap.transform.position, overlap.transform.rotation,out Vector3 direction, out float distance))
            {
                if (distance <= allowedPenetration)
                    continue;

                if (_current.HasSameType(overlap.gameObject) && _current.CanBePlacedOnSameType)
                {
                    float verticalAlignment = Vector3.Dot(direction, Vector3.up);

                    if (verticalAlignment < 0.9f)
                        return false;

                    if (distance > allowedPenetration)
                        return false;
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }


    private Ray GetCenterScreenRay() => _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
}
