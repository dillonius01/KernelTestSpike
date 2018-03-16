using Ninject.Modules;

namespace CoolestLibrary
{
    public class CoolestLibraryModule : NinjectModule
    {
        public override void Load()
        {
            // root
            Bind<Conservatory>().ToSelf().InSingletonScope();

            Bind<IArtist>().To<Actor>().WhenClassHas<DulcetTonesAttribute>();
            Bind<ITeacher>().To<Professor>().WhenClassHas<CampusTypeAttribute>();
        }
    }
}
