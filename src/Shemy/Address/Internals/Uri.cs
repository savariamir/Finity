using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Shemy.Address.Internals
{
    internal static class Uri
    {
        private const char QuestionMark = '?';
        private const char ForwardSlash = '/';
        private const char Equal = '=';
        private const char And = '&';
        private const char Comma = ',';

        internal static bool IsNotWellFormedRelativeUriString(string relativeUri)
        {
            return !System.Uri.IsWellFormedUriString(relativeUri, UriKind.Relative);
        }

        internal static bool IsNotWellFormedAbsoluteUriString(string absolute)
        {
            return !System.Uri.IsWellFormedUriString(absolute, UriKind.RelativeOrAbsolute);
        }

        internal static bool IsValidUrl(string urlString)
        {
            return System.Uri.TryCreate(urlString, UriKind.Absolute, out var uri)
                   && (uri.Scheme == System.Uri.UriSchemeHttp
                       || uri.Scheme == System.Uri.UriSchemeHttps
                       || uri.Scheme == System.Uri.UriSchemeFtp
                       || uri.Scheme == System.Uri.UriSchemeMailto);
        }

        internal static string UrlEncode(string text)
        {
            return HttpUtility.UrlEncode(text);
        }

        internal static string Join(params string[] values)
        {
            return string.Join(Comma, values);
        }

        internal static string GenerateUri(string domain, string relativeUri)
        {
            return new StringBuilder()
                   .Append(domain)
                   // .Append(ForwardSlash)
                   .Append(relativeUri).ToString();
        }

        internal static string GenerateRelativeUri(List<PathItem> pathItems, List<QueryParamItem> queryParamItems)
        {
            var path = pathItems.ToPath();
            var queryParams = queryParamItems.ToQueryParam();

            var relativeUri = new StringBuilder()
                              .Append(path)
                              .Append(queryParams)
                              .ToString();

            return relativeUri;
        }

        private static string ToPath(this List<PathItem> pathItems)
        {
            if (pathItems.Count > 0)
                return new StringBuilder()
                       .Append(GeneratePath(pathItems)).ToString();

            return string.Empty;
        }

        private static string ToQueryParam(this List<QueryParamItem> queryParamItems)
        {
            if (queryParamItems.Count > 0)
                return new StringBuilder()
                       .Append(QuestionMark)
                       .Append(GenerateQueryParam(queryParamItems)).ToString();

            return string.Empty;
        }

        private static string GeneratePath(List<PathItem> list)
        {
            var absolutePath = new StringBuilder().Append(ForwardSlash);
            var last = list.Last();
            list.ForEach(item =>
            {
                absolutePath.Append(item.Name);

                if (item != last)
                    absolutePath.Append(ForwardSlash);
            });

            return absolutePath.ToString();
        }

        private static string GenerateQueryParam(List<QueryParamItem> list)
        {
            var last = list.Last();
            var queryParams = new StringBuilder();
            foreach (var queryParam in list)
            {
                queryParams.Append(queryParam.Name);
                queryParams.Append(Equal);
                queryParams.Append(queryParam.Value);

                if (!queryParam.Equals(last))
                    queryParams.Append(And);
            }

            return queryParams.ToString();
        }
    }
}