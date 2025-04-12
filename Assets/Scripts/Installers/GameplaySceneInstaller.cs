using UnityEngine;
using Zenject;

public class GameplaySceneInstaller : MonoInstaller
{
    [SerializeField] protected BuildModeController _buildModeController;

    public override void InstallBindings()
    {
        Container
            .Bind<BuildModeController>()
            .FromInstance(_buildModeController)
            .AsSingle().NonLazy();
    }
}
