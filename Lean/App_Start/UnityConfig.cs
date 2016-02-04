using Lean.Filters;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Web.Mvc;
using Unity.Mvc5;
using WGX.Common.Unity;

namespace Lean
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.LoadConfiguration();

            //对 ActionFilter 进行注入
            container.RegisterType<IFilterProvider, UnityFilterAttributeFilterProvider>();

            //对全局异常处理程序自注入
            container.RegisterType<ExceptionLogAttribute, ExceptionLogAttribute>();
            container.RegisterType<NoPermissionExceptionHandlerAttribute, NoPermissionExceptionHandlerAttribute>();

            container.RegisterType<PageDatas, PageDatas>();

            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}