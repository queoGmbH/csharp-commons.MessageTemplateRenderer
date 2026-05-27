# Queo.Commons.MessageTemplateRenderer

[![Build Status](https://dev.azure.com/queo-commons/Commons-OpenSource/_apis/build/status%2FqueoGmbH.csharp-commons.MessageTemplateRenderer?branchName=main)](https://dev.azure.com/queo-commons/Commons-OpenSource/_build/latest?definitionId=3&branchName=main) [![Build Status](https://dev.azure.com/queo-commons/Commons-OpenSource/_apis/build/status%2FqueoGmbH.csharp-commons.MessageTemplateRenderer?branchName=develop)](https://dev.azure.com/queo-commons/Commons-OpenSource/_build/latest?definitionId=3&branchName=develop)

## Description
Queo.Commons.MessageTemplateRenderer makes it possible to personalize texts automatically.


## Example
-

### Steps:
-

## How to use it
- include Nuget-Package (queo.commons.messageTemplateRenderer)

```csharp
<PackageReference Include="Queo.Commons.MessageTemplateRenderer" Version="3.0.0" />
```

## Dependency Injection

The library now supports options-based configuration for `FileMessageProvider`.
The existing constructor with `resourceRelativePath` is still available for backward compatibility.

```csharp
using System.IO;

using Microsoft.Extensions.DependencyInjection;

using Queo.Commons.MessageTemplateRenderer.Context;
using Queo.Commons.MessageTemplateRenderer.Provider;

public static class MessageTemplateRendererRegistration
{
	public static IServiceCollection AddMessageTemplateRenderer(this IServiceCollection services)
	{
		services.Configure<FileMessageProviderOptions>(options =>
		{
			options.ResourceRelativePath = Path.Combine("Resources", "MailTemplates");
		});

		services.AddTransient<IRenderContext, DotLiquidRenderContext>();
		services.AddTransient<IMessageProvider, FileMessageProvider>();

		return services;
	}
}
```
