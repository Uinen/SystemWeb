using System;
using System.Text;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Routing;
using System.Web.Mvc;

namespace SystemWeb
{
    public static class Chtml
    {
        public static MvcHtmlString HiddenClassFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            return HiddenClassFor(html, expression, null);
        }

        public static MvcHtmlString HiddenClassFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            ModelMetadata _metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            if (_metaData.Model == null)
                return MvcHtmlString.Empty;

            RouteValueDictionary _dict = htmlAttributes != null ? new RouteValueDictionary(htmlAttributes) : null;

            return MvcHtmlString.Create(HiddenClassFor(html, expression, _metaData, _dict).ToString());
        }

        private static StringBuilder HiddenClassFor<TModel>(HtmlHelper<TModel> html, LambdaExpression expression, ModelMetadata metaData, IDictionary<string, object> htmlAttributes)
        {
            StringBuilder _sb = new StringBuilder();

            foreach (ModelMetadata _prop in metaData.Properties)
            {
                Type _type = typeof(Func<,>).MakeGenericType(typeof(TModel), _prop.ModelType);
                var _body = Expression.Property(expression.Body, _prop.PropertyName);
                LambdaExpression _propExp = Expression.Lambda(_type, _body, expression.Parameters);

                if (!_prop.IsComplexType)
                {
                    string _id = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(_propExp));
                    string _name = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(_propExp));
                    object _value = _prop.Model;

                    _sb.Append(MinHiddenFor(_id, _name, _value, htmlAttributes));
                }
                else
                {
                    if (_prop.ModelType.IsArray)
                        _sb.Append(HiddenArrayFor(html, _propExp, _prop, htmlAttributes));
                    else if (_prop.ModelType.IsClass)
                        _sb.Append(HiddenClassFor(html, _propExp, _prop, htmlAttributes));
                    else
                        throw new Exception(string.Format("Cannot handle complex property, {0}, of type, {1}.", _prop.PropertyName, _prop.ModelType));
                }
            }

            return _sb;
        }

        public static MvcHtmlString HiddenArrayFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            return HiddenArrayFor(html, expression, null);
        }

        public static MvcHtmlString HiddenArrayFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            ModelMetadata _metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            if (_metaData.Model == null)
                return MvcHtmlString.Empty;

            RouteValueDictionary _dict = htmlAttributes != null ? new RouteValueDictionary(htmlAttributes) : null;

            return MvcHtmlString.Create(HiddenArrayFor(html, expression, _metaData, _dict).ToString());
        }

        private static StringBuilder HiddenArrayFor<TModel>(HtmlHelper<TModel> html, LambdaExpression expression, ModelMetadata metaData, IDictionary<string, object> htmlAttributes)
        {
            Type _eleType = metaData.ModelType.GetElementType();
            Type _type = typeof(Func<,>).MakeGenericType(typeof(TModel), _eleType);

            object[] _array = (object[])metaData.Model;

            StringBuilder _sb = new StringBuilder();

            for (int i = 0; i < _array.Length; i++)
            {
                var _body = Expression.ArrayIndex(expression.Body, Expression.Constant(i));
                LambdaExpression _arrayExp = Expression.Lambda(_type, _body, expression.Parameters);
                ModelMetadata _valueMeta = ModelMetadata.FromLambdaExpression((dynamic)_arrayExp, html.ViewData);

                if (_eleType.IsClass)
                {
                    _sb.Append(HiddenClassFor(html, _arrayExp, _valueMeta, htmlAttributes));
                }
                else
                {
                    string _id = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(_arrayExp));
                    string _name = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(_arrayExp));
                    object _value = _valueMeta.Model;

                    _sb.Append(MinHiddenFor(_id, _name, _value, htmlAttributes));
                }
            }

            return _sb;
        }

        public static MvcHtmlString MinHiddenFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            return MinHiddenFor(html, expression, null);
        }

        public static MvcHtmlString MinHiddenFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            string _id = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));
            string _name = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            object _value = ModelMetadata.FromLambdaExpression(expression, html.ViewData).Model;
            RouteValueDictionary _dict = htmlAttributes != null ? new RouteValueDictionary(htmlAttributes) : null;

            return MinHiddenFor(_id, _name, _value, _dict);
        }

        public static MvcHtmlString MinHiddenFor(string id, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            TagBuilder _input = new TagBuilder("input");
            _input.Attributes.Add("id", id);
            _input.Attributes.Add("name", name);
            _input.Attributes.Add("type", "hidden");

            if (value != null)
            {
                _input.Attributes.Add("value", value.ToString());
            }

            if (htmlAttributes != null)
            {
                foreach (KeyValuePair<string, object> _pair in htmlAttributes)
                {
                    _input.MergeAttribute(_pair.Key, _pair.Value.ToString(), true);
                }
            }

            return new MvcHtmlString(_input.ToString(TagRenderMode.SelfClosing));
        }
    }
}