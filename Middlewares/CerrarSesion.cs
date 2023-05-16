using Microsoft.AspNetCore.Http;
public class CerrarSesion
{
    private readonly RequestDelegate _next;
    private readonly string _ruta;

    private readonly ApiOperaciones _api;


    public CerrarSesion(RequestDelegate next, string ruta, ApiOperaciones api)
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
                CDMRequest Requestcdm = (CDMRequest)context.Items["cdm"]!;
                ApiResponse rta = await _api.api_cerrarSesion(context.Request.Cookies["cdm-token"] ?? "");
                if (rta.status == 200)
                {
                    context.Response.Cookies.Delete("cdm-token");
                    context.Response.Cookies.Delete("cdm-idSesion");
                    context.Response.Cookies.Delete("cdm-idColeccion");    
                }
                Requestcdm!.status = rta.status;
                Requestcdm!.msg = rta.msg;
                Requestcdm!.data!.Add("cerrarSesion", rta);
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
                Requestcdm!.data!.Add("cerrarSesion", cdm);
                context.Items["cdm"] = Requestcdm;    
            }
        }
        await _next(context);
        return;
    }
}