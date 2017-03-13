// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   SampleBlock pipeline block.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Commerce.Plugin.Shared.Cart
{
    using Sitecore.Framework.Pipelines;
    using System.Threading.Tasks;
    using System.Linq;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Shops;

    public class SharedCartBlock : PipelineBlock<CartLineComponent, CartLineComponent, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">
        /// The SampleArgument argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="SampleEntity"/>.
        /// </returns>
        public override Task<CartLineComponent> Run(CartLineComponent arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull("The argument can not be null");
            //var result = this._pipeline.Run(arg, context).Result;

            //Get shopname from context
            var shopname = context.CommerceContext.GetObjects<Shop>().FirstOrDefault().Name;

            //Add component to CartlineComponent
            arg.GetComponent<SharedCartLineComponent>().ShopName = shopname;
            return Task.FromResult(arg);
        }
    }
}
