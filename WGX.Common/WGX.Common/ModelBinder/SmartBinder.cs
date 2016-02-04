using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace WGX.Common.ModelBinder
{
    public class SmartBinder : DefaultModelBinder
    {
        protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            base.OnModelUpdated(controllerContext, bindingContext);

            ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(controllerContext.Controller.GetType());
            ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(controllerContext, controllerContext.RouteData.Values["action"].ToString());
            ParameterDescriptor paramDescriptor = actionDescriptor.GetParameters()
                                                                  .FirstOrDefault(p => p.ParameterType == bindingContext.ModelMetadata.ModelType);

            if (null != paramDescriptor)
            {
                foreach (var propertyName in paramDescriptor.BindingInfo.Exclude)
                {
                    bindingContext.ModelState.Remove(propertyName);
                }
                if (paramDescriptor.BindingInfo.Include != null && paramDescriptor.BindingInfo.Include.Count > 0)
                {
                    var models = bindingContext.ModelState.ToList();
                    foreach (var item in models)
                    {
                        if (!paramDescriptor.BindingInfo.Include.Contains(item.Key))
                        {
                            bindingContext.ModelState.Remove(item.Key);
                        }
                    }
                }
            }

            Dictionary<string, bool> startedValid = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            var regStringList = new List<String> ()
            { 
                @"<script[\s\S]+</script *>",
                @"on\w+=\s*(['""\s]?)([/s/S]*[^\1]*?)\1[\s]*",
                @"<ScriptBlock>on\w+=\s*(['""\s]?)([/s/S]*[^\1]*?)\1[\s|>|/>]",
                @"href[ ^=]*=\s*(['""\s]?)[\w]*script+?([/s/S]*[^\1]*?)\1[\s]*"
            };
            
            //验证每个属性中的值是否包含有危险字段.
            foreach (var item in bindingContext.PropertyMetadata)
            {
                if (item.Value.ModelType.FullName.EndsWith("String"))
                {
                    if (item.Value.Model != null)
                    {
                        var propertyValue = item.Value.Model.ToString();
                        Regex reg;
                        bool flag = false;
                        foreach (var pattern in regStringList)
                        {
                            reg = new Regex(pattern);
                            if (reg.Match(propertyValue).Success)
                            {
                                bindingContext.ModelState.AddModelError(item.Key, "提交的信息中包含有非法字符");
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    
                }
            }
            //获取模型的验证结果
            //var results = ModelValidator.GetModelValidator(bindingContext.ModelMetadata, controllerContext).Validate(bindingContext.Model);
            
            //foreach (ModelValidationResult validationResult in results)
            //{
            //    string subPropertyName = CreateSubPropertyName(bindingContext.ModelName, validationResult.MemberName);

            //    //if(bindingContext.PropertyFilter(subPropertyName)) {
            //    //bindingContext.PropertyFilter 是一个 delegate, 如果指定的 member 在 BindAttribute 的 Include 的列表内（或者非 Exclude 的列表内），返回 true, 否则为 false
            //    //部分验证的功能就是通过它的结果来实现的
            //    if (bindingContext.PropertyFilter(validationResult.MemberName))
            //    {
            //        if (!startedValid.ContainsKey(subPropertyName))
            //        {
            //            startedValid[subPropertyName] = bindingContext.ModelState.IsValidField(subPropertyName);
            //        }

            //        if (startedValid[subPropertyName])
            //        {
            //            bindingContext.ModelState.AddModelError(subPropertyName, validationResult.Message);
            //        }

                    
            //    }
            //}


        }

        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            if (propertyDescriptor.Attributes.Cast<Attribute>().Any(a => a.GetType() == typeof(DisableBindingAttribute)))
                return null;
            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }
    }
}
