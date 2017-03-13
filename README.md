# Sitecore Commerce 8.2.1 - Plugin Samples

This repository contains sample plugins for Sitecore Commerce 8.2.1. To use them, you'll need the SDK that was shipped with the release package. You can find the release package [here](https://dev.sitecore.net/Downloads/Sitecore_Commerce/821/Sitecore_Commerce_821.aspx). Inside the SDK you'll find a number of existing sample plugins including a Customer.Sample.Solution.sln Visual Studio solution file. 

## Adding to the Customer Sample Solution

To use these plugins, just copy the folder of the plugin you want to your Customer.Sample.Solution and add the necessary references to project.json in the Sitecore.Commerce.Engine project:

```
"dependencies": {
    ...  
    "Commerce.Plugin.Shared.Cart": "0.0.1",
    ...
  },
  ```
Having the reference to your custom plugin inside the Engine project will automatically include it as part of the debug instance if you chose to run it with F5

## Plugins

### Commerce.Plugin.Shared.Cart
This is a simple plugin that illustrates how one might add a field to the cart, and subsequently the order line items to indicate which online store the product is from. It also adds a column to the order details in Customer & Orders Manager.


**Note**: These are not production-ready plugins, but rather samples to which you can add functionality or just used a guide. 
