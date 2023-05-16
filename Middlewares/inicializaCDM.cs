using Microsoft.AspNetCore.Http;
public class Inicializa
{
    private readonly RequestDelegate _next;


    private readonly ApiOperaciones _api;


    public Inicializa(RequestDelegate next, ApiOperaciones api)
    {
        _next = next;
        _api = api;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        
            CDMRequest cdm = new CDMRequest();
            cdm.idColeccion = context.Request.Cookies["cdm-idColeccion"] ?? "";
            cdm.data = new Dictionary<string, dynamic>();
            cdm.msg = "";
            cdm.status = 0;
            context.Items["cdm"] = cdm;
            
            // Llamar al siguiente middleware en la tubería
            await _next(context);
            return;
    }
}