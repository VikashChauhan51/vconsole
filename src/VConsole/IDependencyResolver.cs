namespace VConsole;
public interface IDependencyResolver
{
    object GetService(Type serviceType);
}
