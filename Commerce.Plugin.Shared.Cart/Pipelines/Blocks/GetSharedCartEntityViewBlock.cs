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
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Commerce.Plugin.Carts;

    public class GetSharedCartEntityViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly GetOnHoldOrderCartCommand _getOnHoldOrderCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleBlock"/> class.
        /// </summary>
        /// <param name="pipeline">
        /// The SamplePipeline pipeline.
        /// </param>
        public GetSharedCartEntityViewBlock(GetOnHoldOrderCartCommand command)
        {
            this._getOnHoldOrderCommand = command;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="entityView">
        /// The SampleArgument argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="SampleEntity"/>.
        /// </returns>
        public async override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull("The argument can not be null");

            var request = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault();

            //Do nothing if there is no request
            if (request == null) { return entityView; }

            //Do nothing if this request is for a different view
            if (request.ViewName != context.GetPolicy<KnownOrderViewsPolicy>().Lines &&
                request.ViewName != context.GetPolicy<KnownOrderViewsPolicy>().Master)
            { return entityView; }

            //Do nothing if there is no entity
            if (request.Entity == null) { return entityView; }

            //Only continue if the entity is an Order
            if (!(request.Entity is Order)) { return entityView; }

            var order = request.Entity as Order;

            EntityView entityViewToProcess;
            if(request.ViewName == context.GetPolicy<KnownOrderViewsPolicy>().Master)
            {
                //Get Order Lines
                entityViewToProcess = entityView.ChildViews.FirstOrDefault(p => p.Name == "Lines") as EntityView;
            }
            else
            {
                entityViewToProcess = entityView;
            }

            //Check for On Hold
            if(order.HasComponent<OnHoldOrderComponent>())
            {
                //Creates a new cart to make changes to on hold orders
                var cart = await this._getOnHoldOrderCommand.Process(context.CommerceContext, order);
                foreach (var line in cart.Lines)
                {
                    this.PopulateLineChildView(entityViewToProcess, line, context);
                }
            }
            else
            {
                //Else, just uses order as is
                foreach (var line in order.Lines)
                {
                    this.PopulateLineChildView(entityViewToProcess, line, context);
                }
            }

            return entityView;
        }

        /// <summary>
        /// To add the new shared cart data to the line item
        /// </summary>
        /// <param name="entityView"></param>
        /// <param name="line"></param>
        /// <param name="context"></param>
        private void PopulateLineChildView(EntityView entityView, CartLineComponent line, CommercePipelineExecutionContext context)
        {
            var lineEntityView = entityView.ChildViews.OfType<EntityView>().FirstOrDefault(p => p.ItemId == line.Id);

            if (line.HasComponent<SharedCartLineComponent>())
            {
                var sharedCartLineComponent = line.GetComponent<SharedCartLineComponent>();
                //Add shop name property to the line item view
                lineEntityView.Properties.Add(new ViewProperty { Name = "ShopName", DisplayName = "Store", IsReadOnly = true, RawValue = sharedCartLineComponent.ShopName });
            }
            else
            {
                //Add blank value if component doesn't exist, because Headers
                lineEntityView.Properties.Add(new ViewProperty { Name = "ShopName", DisplayName = "Store", IsReadOnly = true, RawValue = string.Empty });
            }
        }
    }
}
