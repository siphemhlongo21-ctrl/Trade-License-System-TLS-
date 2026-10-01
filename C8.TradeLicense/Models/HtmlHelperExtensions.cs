using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace C8.TradeLicense.Models
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Required<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string resolvedLabelText = metadata.DisplayName ?? metadata.PropertyName;
            if (metadata.IsRequired)
            {
                resolvedLabelText += "*";
            }
            return LabelExtensions.LabelFor<TModel, TValue>(html, expression, resolvedLabelText, htmlAttributes);
        }
    }
}