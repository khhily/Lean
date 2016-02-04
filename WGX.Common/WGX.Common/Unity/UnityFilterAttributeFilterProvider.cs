using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace WGX.Common.Unity
{
    public class UnityFilterAttributeFilterProvider : FilterAttributeFilterProvider
    {


        private IUnityContainer Container;

        public UnityFilterAttributeFilterProvider(IUnityContainer container)
        {
            Container = container;
        }

        protected override IEnumerable<FilterAttribute> GetControllerAttributes(
                    ControllerContext controllerContext,
                    ActionDescriptor actionDescriptor)
        {

            var attributes = base.GetControllerAttributes(controllerContext,
                                                          actionDescriptor);
            foreach (var attribute in attributes)
            {
                Container.BuildUp(attribute.GetType(), attribute);
                yield return attribute;
            }
        }

        protected override IEnumerable<FilterAttribute> GetActionAttributes(
                    ControllerContext controllerContext,
                    ActionDescriptor actionDescriptor)
        {

            var attributes = base.GetActionAttributes(controllerContext,
                                                      actionDescriptor);
            foreach (var attribute in attributes)
            {
                Container.BuildUp(attribute.GetType(), attribute);
                yield return attribute;
            }

        }

        public override IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = base.GetFilters(controllerContext, actionDescriptor);

            //var ff = this._container.ResolveAll<IActionFilter>().ToList();

            foreach (var filter in filters)
            {
                Container.BuildUp(filter.Instance);
                yield return filter;
            }
        }

    }
}
