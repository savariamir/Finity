using System.Collections.Generic;
using System.Linq;
using UriBuilder.Abstractions;
using UriBuilder.Exceptions;
using UriBuilder.Internals;

namespace UriBuilder.Builders
{
    public class UriBuilder : IPath, IQueryParam, IUriBuilder, IGenerate
    {
        private readonly List<PathItem> _pathItems;
        private readonly List<QueryParamItem> _queryParams;
        private string _domain;
        private bool _encodingNeeded;

        private UriBuilder()
        {
            _pathItems = new List<PathItem>();
            _queryParams = new List<QueryParamItem>();
        }

        public static IUriBuilder NewUrl()
        {
            return new UriBuilder();
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
            _queryParams.Add(new QueryParamItem { Name = name, Value = Uri.Join(values) });
            return this;
        }

        public IGenerate EncodeUrl()
        {
            _encodingNeeded = true;
            return this;
        }

        public string Generate()
        {
            var relativeUri = Uri.GenerateRelativeUri(_pathItems, _queryParams);

            if (Uri.IsNotWellFormedRelativeUriString(relativeUri))
                throw new RelativeUrlException("relative url is not correct");

            if (Uri.IsNotWellFormedAbsoluteUriString(_domain))
                throw new AbsoluteUrlException($"{_domain} is not correct");

            if (_encodingNeeded)
                relativeUri = Uri.UrlEncode(relativeUri);
            
            return Uri.GenerateUri(_domain, relativeUri);
        }
    }
}