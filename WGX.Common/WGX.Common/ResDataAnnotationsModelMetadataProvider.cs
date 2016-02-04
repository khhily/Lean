using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WGX.Common {
    public class ResDataAnnotationsModelMetadataProvider : DataAnnotationsModelMetadataProvider {

        private ResourceManager ResMgr;

        public ResDataAnnotationsModelMetadataProvider(ResourceManager resMgr) {

            if (resMgr == null)
                throw new ArgumentNullException("resMgr");

            this.ResMgr = resMgr;
        }

        private string GetString(Type containerType, string propertyName) {
            var key = string.Format("{0}{1}_{2}_DisplayName", containerType.Namespace.Replace(".", ""), containerType.Name, propertyName);
            return this.ResMgr.GetString(key);
        }

        /// <summary>
        /// 重写生成元数据的方法
        /// 只有页面上来通过Lambda表达式过来的元数据，才进行中英文转换
        /// (该过滤为了减少元数据的中英文转换次数)
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="containerType"></param>
        /// <param name="modelAccessor"></param>
        /// <param name="modelType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName) {
            if (containerType != null) {//请改之前先做好测试!这是最终的条件.

                //动态类型/代理类 EF
                var t = containerType.Assembly.IsDynamic ? containerType.BaseType : containerType;

                var v = this.GetString(t, propertyName);
                if (!string.IsNullOrWhiteSpace(v)) {
                    //这里,如果 v 是空字符串的话,居然会影响到 SmartModelBinder 的 ModelValidator.GetModelValidator
                    var dsp = new DisplayAttribute() {
                        Name = v
                    };
                    var attrs = attributes.ToList();
                    attrs.RemoveAll(a => a is DisplayAttribute);
                    attrs.Add(dsp);
                    return base.CreateMetadata(attrs, containerType, modelAccessor, modelType, propertyName);
                }
            }

            return base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
        }
    }
}
