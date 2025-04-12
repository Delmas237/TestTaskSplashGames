using UnityEngine;

public interface IPlaceable
{
    bool CanBePlacedOnSameType { get; }

    bool CanBePlacedOn(RaycastHit hit);
    void SetColor(Color color);
    void SetDefaultColors();
}
