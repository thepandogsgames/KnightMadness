using Code.Events;
using Zenject;

namespace Code.Installer
{
    public class ProjectInstaller : MonoInstaller
    {

        public override void InstallBindings()
        {
            Container.Bind<IEventManager>().To<EventManager>().AsSingle();
        }
    }
}