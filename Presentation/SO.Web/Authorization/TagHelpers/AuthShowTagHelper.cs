using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SO.Web.Authorization.TagHelpers
{
    [HtmlTargetElement("auth-show")]
    public class AuthShowTagHelper : TagHelper
    {
        public string Permission { get; set; } = string.Empty;

        private readonly IAppAuthorizationService _auth;
        private readonly IHttpContextAccessor _http;

        public AuthShowTagHelper(IAppAuthorizationService auth, IHttpContextAccessor http)
        {
            _auth = auth;
            _http = http;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var user = _http.HttpContext?.User;
            if (string.IsNullOrEmpty(Permission) || user == null || !_auth.IsAllowed(user, Permission))
            {
                output.SuppressOutput();
            }
            else
            {
                output.TagName = null; // render only inner content
            }
        }
    }
}


