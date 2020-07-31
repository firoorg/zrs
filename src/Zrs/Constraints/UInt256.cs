namespace Zrs.Constraints
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using NBitcoin;

    public sealed class UInt256 : IRouteConstraint
    {
        public bool Match(
            HttpContext httpContext,
            IRouter route,
            string routeKey,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (!values.TryGetValue(routeKey, out var value) || value == null)
            {
                return false;
            }

            switch (value)
            {
                case uint256 _:
                    return true;
                case string s:
                    return uint256.TryParse(s, out _);
                default:
                    return false;
            }
        }
    }
}
