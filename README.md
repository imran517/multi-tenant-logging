# multi-tenant-logging

MultiTenant logging is a sample project that shows how to log Tenant information with each log entry while using Finbuckle and Serilog.

## Dependencies

* [Finbuckle](https://https://github.com/Finbuckle/Finbuckle.MultiTenant)
* [Serilog](https://https://github.com/serilog)

## How does it work

### **Finbuckle Config**

* Register Services: Register Finbuckel related services in ConfigureServices method of "Startup.cs". For simplicity we are using configuration store for tenant info and static strategy for tenant resolution.

```
services.AddMultiTenant<TenantInfo>()
        .WithConfigurationStore()
        .WithStaticStrategy("sample-tenant-1");
```

* Configuration: appSettings.json file looks like the following

```
  "Finbuckle:MultiTenant:Stores:ConfigurationStore": {
    "Defaults": {
      "ConnectionString": "Datasource=sample.db"
    },
    "Tenants": [
      {
        "Id": "sample-tenant-1",
        "Identifier": "sample-tenant-1",
        "Name": "sample-tenant-1"
      }
   
```

### **Serilog Config**

* Coonfiguration: configure logging in the "Program.cs". We are  confguring enrichment of log events with properties from Serilog's LogContext.

```
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .Enrich.FromLogContext())
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

* Template: Setup the output template to include "TenantIdentifier" at the appropriate place. TenantIdentifier will be replaced with the actual tenant resolved in the middleware per request.
```
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "{Timestamp:HH:mm:ss.fff zzz} [{Level}] [{TenantIdentifier}] {Message}{NewLine}{Exception}"
        }
      }
    ]
  }
```

### **Middleware for Logging**

* Middleware: The middleware uses Serilog's LogContext to inject the"TenantIdentifier" property on each request.

```
public async Task Invoke(HttpContext httpContext)
{
    var tenantIdentifier = httpContext?.GetMultiTenantContext<TenantInfo>()?.TenantInfo?.Identifier;
    tenantIdentifier ??= "Tenant: None";

    LogContext.PushProperty("TenantIdentifier", $"Tenenat: {tenantIdentifier}");
    await _next.Invoke(httpContext);
}
```
* Middlwware Extension: Just a helper extension to use the middleware.

```
public static IApplicationBuilder UseMultiTenantLogging(this IApplicationBuilder builder)
{
    return builder.UseMiddleware<MultiTenantLoggingMiddleware>();
}
```
* Middleware Usage: Finally, just use the middleware in the Configure method of Startup.cs

```
 app.UseMultiTenantLogging();
```
### **Check Logs**
Check your log entries and you will see the [sample-tenant-1] on each log entry whenever it is available.
