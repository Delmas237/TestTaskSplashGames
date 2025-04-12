using UnityEngine;
using Zenject;

public class BuildTrigger : MonoBehaviour
{
    [SerializeField] private PlaceableObject _placeablePrefab;
    private BuildModeController _buildModeController;

    [Inject]
    private void Initialize(BuildModeController buildModeController)
    {
        _buildModeController = buildModeController;
    }

    private void OnMouseUp()
    {
        _buildModeController.EnterBuildMode(_placeablePrefab);
    }
}
