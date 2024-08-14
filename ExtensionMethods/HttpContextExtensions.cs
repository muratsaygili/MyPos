namespace MyPosTest.ExtensionMethods;

public static class HttpContextExtensions
{


    public static IApplicationBuilder UseHttpContext(this IApplicationBuilder app)
    {
        MyHttpContext.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
        return app;
    }
}