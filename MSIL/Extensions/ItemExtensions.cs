﻿using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using System;

namespace MSIL.Extensions
{
    public static class ItemExtensions
    {
        public static string Url(this Item item, ItemUrlBuilderOptions options = null)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (options != null)
                return LinkManager.GetItemUrl(item, options);

            return LinkManager.GetItemUrl(item);
        }
    }
}