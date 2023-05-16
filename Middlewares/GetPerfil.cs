using Microsoft.AspNetCore.Http;
public class GetPerfil
{
    private readonly RequestDelegate _next;
    private readonly opcionesMiddleware _ruta;

    private readonly ApiOperaciones _api;


    public GetPerfil(RequestDelegate next, opcionesMiddleware rutas, ApiOperaciones api)
    {
        _next = next;
        _ruta = rutas;
        _api = api;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (Array.Exists(_ruta.rutas!, element => element == context.Request.Path))
        {
            try
            {
                CDMRequest Requestcdm = (CDMRequest)context.Items["cdm"]!;
                ApiResponse perfil = await _api.api_getPerfil(context.Request.Cookies["cdm-token"] ?? "");
                Requestcdm!.status = perfil.status;
                Requestcdm!.msg = perfil.msg;
                Requestcdm!.data!.Add("getPerfil", perfil);
                context.Items["cdm"] = Requestcdm;    
            }
            catch (System.Exception)
            {
                CDMRequest cdm = new CDMRequest();
                CDMRequest Requestcdm = (CDMRequest)context.Items["cdm"]!;
                cdm.status = 500;
                cdm.msg ="Error inesperado";
                Requestcdm!.status = cdm.status;
                Requestcdm!.msg = cdm.msg;
                Requestcdm!.data!.Add("getPerfil", cdm);
                context.Items["cdm"] = Requestcdm;    
            }
        }
        await _next(context);
    }
}