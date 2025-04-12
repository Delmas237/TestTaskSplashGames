using UnityEngine;

public class SpherePlaceable : PlaceableObject
{
    public override bool CanBePlacedOn(RaycastHit hit)
    {
        return Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up)) < 0.1f;
    }
}
