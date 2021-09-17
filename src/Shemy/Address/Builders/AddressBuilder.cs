using System.Collections.Generic;
using System.Linq;
using Shemy.Address.Abstractions;
using Shemy.Address.Exceptions;
using Shemy.Address.Internals;

namespace Shemy.Address.Builders
{
    public class AddressBuilder : IPath, IQueryParam, IAddressBuilder, IGenerate
    {
        private readonly List<PathItem> _pathItems;
        private readonly List<QueryParamItem> _queryParams;
        private string _baseAddress;
        private bool _encodingNeeded;

        private AddressBuilder()
        {
            _pathItems = new List<PathItem>();
            _queryParams = new List<QueryParamItem>();
        }

        public static IAddressBuilder Create() => new AddressBuilder();

        public IPath SetBaseAddress(string domain)
        {
            _baseAddress = domain;
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

            if (!Uri.IsValidUrl(_baseAddress))
                throw new AbsoluteUrlException($"{_baseAddress} is not correct");

            if (_encodingNeeded)
                relativeUri = Uri.UrlEncode(relativeUri);
            
            return Uri.GenerateUri(_baseAddress, relativeUri);
        }
    }
}