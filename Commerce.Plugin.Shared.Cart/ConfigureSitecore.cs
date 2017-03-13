// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   The SamplePlugin startup class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Commerce.Plugin.Shared.Cart
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using System.Reflection;

    /// <summary>
    /// The carts configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
            .ConfigurePipeline<IPopulateLineItemPipeline>(
                configure =>
                { configure.Add<SharedCartBlock>().After<PopulateLineItemProductBlock>(); }
                )
            .ConfigurePipeline<IGetEntityViewPipeline>(
                configure => 
                { configure.Add<GetSharedCartEntityViewBlock>().Before<IFormatEntityViewPipeline>(); }
            ));
        }
    }
}
