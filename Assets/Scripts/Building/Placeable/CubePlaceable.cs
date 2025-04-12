using UnityEngine;

public class CubePlaceable : PlaceableObject
{
    public override bool CanBePlacedOn(RaycastHit hit)
    {
        return Vector3.Dot(hit.normal, Vector3.up) > 0.9f;
    }
}
