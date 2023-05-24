using System;
using System.Web.Mvc;

public class CustomOutputCacheAttribute : ActionFilterAttribute
{
	private int _duration;

	public CustomOutputCacheAttribute(int duration)
	{
		_duration = duration;
	}

	public override void OnActionExecuting(ActionExecutingContext filterContext)
	{
		string cacheKey = string.Format("{0}-{1}", filterContext.HttpContext.Request.Url.AbsoluteUri, filterContext.HttpContext.User.Identity.Name);

		if (filterContext.HttpContext.Cache[cacheKey] != null)
		{
			filterContext.Result = (ActionResult)filterContext.HttpContext.Cache[cacheKey];
		}
	}

	public override void OnActionExecuted(ActionExecutedContext filterContext)
	{
		string cacheKey = string.Format("{0}-{1}", filterContext.HttpContext.Request.Url.AbsoluteUri, filterContext.HttpContext.User.Identity.Name);

		filterContext.HttpContext.Cache.Add(
			cacheKey,
			filterContext.Result,
			null,
			DateTime.Now.AddSeconds(_duration),
			TimeSpan.Zero,
			System.Web.Caching.CacheItemPriority.Default,
			null);
	}
}
