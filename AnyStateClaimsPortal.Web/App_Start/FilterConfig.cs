using System.Web.Mvc;

namespace AnyStateClaimsPortal.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // HandleErrorAttribute removed — controllers use try-catch with Content() for debugging
        }
    }
}
