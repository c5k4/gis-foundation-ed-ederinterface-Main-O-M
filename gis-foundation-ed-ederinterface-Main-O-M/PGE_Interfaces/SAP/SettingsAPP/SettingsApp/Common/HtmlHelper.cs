using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using SettingsApp.Models;

namespace SettingsApp.Common
{
    public static class HtmlExtensions
    {
        public enum PageMode
        {
            Current = 1,
            Future = 2,
            History = 3,
            GIS = 4,
            File = 5,
            EngineeringInfo = 6,
            //******ENOS2EDGIS Start, added Generation***
            Generation = 7,
            Protection = 8,
            Relay = 9,
            Generator =10,
            Equipment = 11
            //*****ENOS2EDGIS End************************
        }

        public static MvcHtmlString LabelForRequired<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText = "")
        {
            return LabelHelper(html, ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                ExpressionHelper.GetExpressionText(expression), labelText, expression);
        }


        private static MvcHtmlString LabelHelper<TModel, TValue>(HtmlHelper<TModel> html, ModelMetadata metadata, string htmlFieldName, string labelText, Expression<Func<TModel, TValue>> expression)
        {
            if (string.IsNullOrEmpty(labelText))
            {
                labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            }

            if (string.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            bool isRequired = false;

            if (metadata.ContainerType != null)
            {
                var x = metadata.ContainerType.GetProperty(metadata.PropertyName).GetCustomAttributes(typeof(SettingsValidatorAttribute), false);
                if (x.Length > 0)
                {
                    SettingsValidatorAttribute sva = x[0] as SettingsValidatorAttribute;
                    if (sva.DeviceName.ToUpper() == "SWITCH")
                    {
                        object switchType = GetValue(html, expression, "SwitchType");
                        object ats = GetValue(html, expression, "ATSCapable");
                        if (SiteCache.GetRequiredFields(sva.DeviceName, (switchType == null ? "" : switchType.ToString()), (ats == null ? "" : ats.ToString())).Contains(sva.PropertyName))
                            isRequired = true;
                    }
                    else if (sva.DeviceName.ToUpper() == "RECLOSER")
                    {
                        object controlType = GetValue(html, expression, "ControllerUnitType");
                        if (SiteCache.GetRequiredFields(sva.DeviceName, (controlType == null ? "" : controlType.ToString())).Contains(sva.PropertyName))
                            isRequired = true;
                    }
                    else if (sva.DeviceName.ToUpper() == "CAPACITOR")
                    {
                        object controlType = GetValue(html, expression, "ControlType");
                        if (SiteCache.GetRequiredFields(sva.DeviceName, (controlType == null ? "" : controlType.ToString())).Contains(sva.PropertyName))
                            isRequired = true;
                    }
                        //ME Q4 2019 - DA #1
                    else if (sva.DeviceName.ToUpper() == "MSOSWITCH")
                    {
                        object switchType = GetValue(html, expression, "SwitchType");
                        object ats = GetValue(html, expression, "ATSCapable");
                        if (SiteCache.GetRequiredFields(sva.DeviceName, (switchType == null ? "" : switchType.ToString()), (ats == null ? "" : ats.ToString())).Contains(sva.PropertyName))
                            isRequired = true;
                    }
                    else
                    {
                        if (SiteCache.GetRequiredFields(sva.DeviceName).Contains(sva.PropertyName))
                            isRequired = true;
                    }
                }
            }


            TagBuilder tag = new TagBuilder("label");
            tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));


            if (isRequired)
                tag.Attributes.Add("class", "label-required");

            tag.SetInnerText(labelText);

            var output = tag.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(output);
        }

        private static string GetValue<TModel, TProperty>(HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string propertyName)
        {
            //MemberExpression body = (MemberExpression)expression.Body;
            //string propertyName1 = body.Member.Name;
            TModel model = helper.ViewData.Model;
            if (model == null)
                return "";
            object value = typeof(TModel).GetProperty(propertyName).GetValue(model, null);
            return (value == null ? "" : value.ToString());
        }


    }

}