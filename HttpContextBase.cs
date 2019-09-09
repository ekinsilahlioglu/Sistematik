using System;
namespace First_part
{
    public abstract class HttpContextBase : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
