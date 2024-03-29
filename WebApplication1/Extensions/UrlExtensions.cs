﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
//using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Http.Extensions;
//using FormBuilder.ViewModels;

namespace FormBuilder.Extensions
{
    public static class UrlExtensions
    {
        //public static string FormUrl(this UrlHelper helper, FormViewModel model, bool isPreview = false)
        //{
        //    if (!isPreview)
        //    {
        //        return helper.RouteUrl("form-register", new { formName = model.Slug });
        //    }
        //    else
        //    {
        //        return helper.RouteUrl("form-register", new { formName = model.Slug, ipv = true });
        //    }            
        //}

        //public static string FormPreviewUrl(this UrlHelper helper, FormViewModel model)
        //{

        //    return helper.RouteUrl("form-preview", new { id=model.Id });

        //}

        /// <summary>
        /// Absolutes the route URL.
        /// </summary>
        /// <param name="url">The URL to redirect to.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>Absolute url route (host name included)</returns>
        /* public static string AbsoluteRouteUrl(this Microsoft.AspNetCore.Mvc.Routing.UrlHelper url, string routeName, object routeValues)
         {
             //Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
             Uri requestUrl = url.ActionContext.HttpContext.Request.
             var routeUrl = url.RouteUrl(routeName, routeValues);
             string absoluteRouteUrl = string.Format("{0}{1}{2}{3}", requestUrl.Scheme, Uri.SchemeDelimiter, requestUrl.Authority, routeUrl);

             return absoluteRouteUrl;
         }*/

        public static string AbsoluteRouteUrl(
        this IUrlHelper url,
        string routeName,
        object routeValues = null)
        {
            return url.RouteUrl(routeName, routeValues, url.ActionContext.HttpContext.Request.Scheme);
        }
    }
}