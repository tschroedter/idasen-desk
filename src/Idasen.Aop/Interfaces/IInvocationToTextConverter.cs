using Castle.DynamicProxy ;
using JetBrains.Annotations ;

namespace Idasen.Aop.Interfaces
{
    public interface IInvocationToTextConverter
    {
        string Convert ( [ NotNull ] IInvocation invocation ) ;
    }
}