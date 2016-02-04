using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace WGX.Common.Helper
{
    public static class HtmlHelper
    {
        public static MvcHtmlString Pager<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, Pager>> pagerProperty, int labelCount = 10)
        {
            var pager = (Pager)ModelMetadata.FromLambdaExpression(pagerProperty, helper.ViewData).Model;
            var fullName = string.Format("{0}.Page", helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(pagerProperty)));

            var pagination = new Pagination
            {
                Total = pager.Count,
                PageSize = pager.PageSize,
                CurrPage = (pager.Page ?? 0) + 1,
                LableCount = labelCount
            };

            helper.ViewBag.Pagination = pagination;
            helper.ViewBag.HiddenName = fullName;

            return EditorBlockAFor(helper, "Pagination", pagerProperty, true);
        }

        private static MvcHtmlString EditorBlockAFor<TModel>(HtmlHelper<TModel> helper, string property, string template, ModelMetadata metadata, bool withLabel, string containerClass = "col-md-4", object htmlAttributes = null, string labelWidth = "100px", string inputWidth = "", string lineHeight = "30px")
        {
            var ctx = helper.ViewContext.Controller.ControllerContext;
            var result = ViewEngines.Engines.FindPartialView(helper.ViewContext.Controller.ControllerContext, template);
            if (result.View != null)
            {
                var attrs = System.Web.WebPages.Html.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

                using (var writer = new StringWriter(CultureInfo.CurrentCulture))
                {
                    var vctx = new ViewContext(ctx, result.View, helper.ViewData, helper.ViewContext.TempData, writer);
                    vctx.ViewData.ModelMetadata = helper.ViewData.ModelMetadata;//////必须的，调了很长时间，定位问题在这个 ModelMetadata 上
                    vctx.ViewBag.PropertyName = property;
                    vctx.ViewBag.PropertyValue = metadata.Model;

                    vctx.ViewBag.ContainerClass = containerClass;
                    vctx.ViewBag.IsRequired = metadata.IsRequired;
                    vctx.ViewBag.HtmlAttributes = attrs;
                    vctx.ViewBag.WithLabel = withLabel;
                    vctx.ViewBag.LabelWidth = labelWidth;
                    vctx.ViewBag.InputWidth = inputWidth;
                    vctx.ViewBag.LineHeight = lineHeight;

                    //rg 不可用这种方式,如果没有 DisplayName ,就是 NULL, 会报错
                    vctx.ViewBag.DisplayName = metadata.DisplayName ?? metadata.PropertyName; //add by marvinliang

                    result.View.Render(vctx, writer);

                    return MvcHtmlString.Create(writer.ToString());
                }
            }
            throw new InvalidOperationException(string.Format("particle view {0} not found", template));
        }

        public static MvcHtmlString EditorBlockAFor<TModel>(this HtmlHelper<TModel> helper, string template, string property, bool withLabel, string containerClass = "col-md-4", object htmlAttributes = null, string labelWidth = "", string inputWidth = "", string lineHeight = "")
        {
            var metadata = ModelMetadata.FromStringExpression(property, helper.ViewData);
            return EditorBlockAFor(helper, property, template, metadata, withLabel, containerClass, htmlAttributes, labelWidth, inputWidth, lineHeight);
        }

        public static MvcHtmlString EditorBlockAFor<TModel, TProperty>(this HtmlHelper<TModel> helper, string template, Expression<Func<TModel, TProperty>> property, bool withLabel, string containerClass = "col-lg-4 col-md-4 col-xs-4", object htmlAttributes = null, string labelWidth = "100px", string inputWidth = "", string lineHeight = "30px")
        {
            var metadata = ModelMetadata.FromLambdaExpression(property, helper.ViewData);
            var pName = ExpressionHelper.GetExpressionText(property);
            return EditorBlockAFor(helper, pName, template, metadata, withLabel, containerClass, htmlAttributes, labelWidth, inputWidth, lineHeight);
        }

        public static MvcHtmlString TextBlockFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> property, string containerClass = "col-lg-4 col-md-4 col-xs-4", string labelWidth = "100px", string inputWidth = "", string lineHeight = "30px", object htmlAttributes = null, bool withLabel = true, string formart = "")
        {
            if (!string.IsNullOrWhiteSpace(formart))
            {
                helper.ViewBag.Formart = formart;
            }
            return EditorBlockAFor(helper, "TextBlock", property, withLabel, containerClass, htmlAttributes, labelWidth, inputWidth, lineHeight);
        }

        public static MvcHtmlString PasswordBlockFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> property, string containerClass = "col-lg-4 col-md-4 col-xs-4",
            string labelWidth = "100px", string inputWidth = "", string lineHeight = "30px",
            object htmlAttributes = null, bool withLabel = true)
        {
            return EditorBlockAFor(helper, "PasswordBlock", property, withLabel, containerClass, htmlAttributes, labelWidth, inputWidth, lineHeight);
        }

        public static MvcHtmlString DropDownBlockFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> property, IEnumerable<SelectListItem> itemList, string containerClass = "col-lg-4 col-md-4 col-xs-4",
            string labelWidth = "100px", string inputWidth = "", string lineHeight = "30px",
            string optLabel = "", string optValue = "", object htmlAttribute = null, bool withLabel = true)
        {
            if (!string.IsNullOrEmpty(optLabel))
            {
                var list = itemList.ToList();
                list.Insert(0, new SelectListItem
                {
                    Text = optLabel,
                    Value = optValue
                });
                itemList = list;
            }
            helper.ViewData["DropDownBlock"] = itemList;
            return EditorBlockAFor(helper, "DropDownBlock", property, withLabel, containerClass, htmlAttribute, labelWidth, inputWidth, lineHeight);
        }

        public static MvcHtmlString CheckBoxBlockFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> property, string containerClass = "col-lg-4 col-md-4 col-xs-4",
            string labelWidth = "100px", string inputWidth = "", string lineHeight = "30px",
            object htmlAttributes = null, bool withLabel = true)
        {
            return EditorBlockAFor(helper, "CheckBoxBlock", property, withLabel, containerClass, htmlAttributes, labelWidth, inputWidth, lineHeight);
        }

        public static MvcHtmlString BoolDropDownListBlockFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, bool>> property, string trueLabel = "Yes", string falseLabel = "No", string containerClass = "col-lg-4 col-md-4 col-xs-4", string labelWidth = "100px", string inputWidth = "", string lineHeight = "30px", object htmlAttributes = null, bool withLabel = true)
        {
            helper.ViewBag.TrueLabel = trueLabel;
            helper.ViewBag.FalseLabel = falseLabel;
            return EditorBlockAFor(helper, "BoolDropDownListBlock", property, withLabel, containerClass, htmlAttributes, labelWidth, inputWidth, lineHeight);
        }

        public static MvcHtmlString BoolDropDownListBlockFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, bool?>> property, string trueLabel = "Yes", string falseLabel = "No", string nullLabel = "All", string containerClass = "col-lg-4 col-md-4 col-xs-4", string labelWidth = "100px", string inputWidth = "", string lineHeight = "30px", object htmlAttributes = null, bool withLabel = true)
        {
            helper.ViewBag.TrueLabel = trueLabel;
            helper.ViewBag.FalseLabel = falseLabel;
            helper.ViewBag.IsNullable = true;
            helper.ViewBag.NullLabel = nullLabel;
            return EditorBlockAFor(helper, "BoolDropDownListBlock", property, withLabel, containerClass, htmlAttributes, labelWidth, inputWidth, lineHeight);
        }

        public static MvcHtmlString DoubleTimeSelectorFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> property, Expression<Func<TModel, object>> startDateProperty,
            Expression<Func<TModel, object>> endDateProperty, string containerClass = "col-xs-4", object htmlAttribute = null, bool withLabel = true,string labelWidth = "100px", string inputWidth = "", string lineHeight = "30px")
        {
            if (startDateProperty != null)
            {
                var startExpression = startDateProperty.Body is UnaryExpression
                    ? (MemberExpression) (((UnaryExpression) startDateProperty.Body).Operand)
                    : (MemberExpression) startDateProperty.Body;

                helper.ViewBag.StartProperty = string.Join(".", startExpression.ToString().Split('.').Skip(1));
            }
            if (endDateProperty != null)
            {
                var endExpression = endDateProperty.Body is UnaryExpression
                    ? (MemberExpression) (((UnaryExpression) endDateProperty.Body).Operand)
                    : (MemberExpression) endDateProperty.Body;

                helper.ViewBag.EndProperty = string.Join(".", endExpression.ToString().Split('.').Skip(1));
            }

            return EditorBlockAFor(helper, "DoubleTimeSelector", property, withLabel, containerClass, htmlAttribute, labelWidth, inputWidth, lineHeight);
        }

        public static MvcHtmlString FileUploadBlockFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> property, string containerClass = "col-lg-4 col-md-4 col-xs-4",
            string labelWidth = "100px", string inputWidth = "", string lineHeight = "30px",
            object htmlAttributes = null, bool withLabel = true, string fileName = "", string btnWidth = "70px")
        {
            helper.ViewBag.FileName = fileName;
            helper.ViewBag.ButtonWidth = btnWidth;
            return EditorBlockAFor(helper, "FileUpload", property, withLabel, containerClass, htmlAttributes, labelWidth, inputWidth, lineHeight);
        }
    }
}
