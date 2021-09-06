using System.Collections.Generic;
using System.Linq;
using UriBuilder.Abstractions;
using UriBuilder.Exceptions;
using UriBuilder.Internals;

namespace UriBuilder.Builders
{
    public class UrlBuilder : IPath, IQueryParam, ILinkBuilder, IGenerate
    {
        private readonly List<PathItem> _pathItems;
        private readonly List<QueryParamItem> _queryParams;
        private string _domain;
        private bool _encodingNeeded;

        private UrlBuilder()
        {
            _pathItems = new List<PathItem>();
            _queryParams = new List<QueryParamItem>();
        }

        public static ILinkBuilder NewUrl()
        {
            return new UrlBuilder();
        }

        public IPath SetDomain(string domain)
        {
            _domain = domain;
            return this;
        }

        public IPath SetPath(params string[] paths)
        {
            _pathItems.AddRange(paths.Select(p => new PathItem() { Name = p }));
            return this;
        }

        public IQueryParam SetQueryParam(string name, params string[] values)
        {
            _queryParams.Add(new QueryParamItem { Name = name, Value = UrlExtension.Join(values) });
            return this;
        }

        public IGenerate EncodeUrl()
        {
            _encodingNeeded = true;
            return this;
        }

        public string Generate()
        {
            var relativeUri = UrlExtension.GenerateRelativeUri(_pathItems, _queryParams);

            if (UrlExtension.IsNotWellFormedRelativeUriString(relativeUri))
                throw new RelativeUrlException("relative url is not correct");

            if (UrlExtension.IsNotWellFormedAbsoluteUriString(_domain))
                throw new AbsoluteUrlException($"{_domain} is not correct");

            if (_encodingNeeded)
                relativeUri = UrlExtension.UrlEncode(relativeUri);
            
            return UrlExtension.GenerateUri(_domain, relativeUri);
        }
    }
}