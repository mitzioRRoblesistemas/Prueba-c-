using Microsoft.AspNetCore.Http;
public class IngresoDesdeCdm
{
    private readonly RequestDelegate _next;
    private readonly string _ruta;

    private readonly ApiOperaciones _api;


    public IngresoDesdeCdm(RequestDelegate next, string ruta, ApiOperaciones api)
    {
        _next = next;
        _ruta = ruta;
        _api = api;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == _ruta)
        {
            try
            {
                if (context.Request.Query.ContainsKey("idSesion")){
                    var middlewareAutoriza = new Autoriza(_next,"MiddlewareVerificaSesionToken",_api);
                    await middlewareAutoriza.InvokeAsync(context);
                    return;
                }
            }
            catch (System.Exception)
            {
                await _next(context);

            }
        }
        return;
    }
}