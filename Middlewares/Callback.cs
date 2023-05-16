using Microsoft.AspNetCore.Http;
public class Callback
{
    private readonly RequestDelegate _next;
    private readonly string _ruta;

    private readonly ApiOperaciones _api;


    public Callback(RequestDelegate next, string rutaCallback, ApiOperaciones api)
    {
        _next = next;
        _ruta = rutaCallback;
        _api = api;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        
        if (context.Request.Path == _ruta)
        {
            try
            {
                CDMRequest cdm = new CDMRequest();
                CDMRequest Requestcdm = (CDMRequest)context.Items["cdm"]!;
                if (context.Request.Query.ContainsKey("tipo") && context.Request.Query["tipo"].ToString() == "login")
                {
                    context.Response.Cookies.Delete("cdm-token");
                    context.Response.Cookies.Append("cdm-idSesion", context.Request.Query["idSesion"].ToString() ?? "", new CookieOptions
                    {MaxAge = TimeSpan.FromDays(365),HttpOnly = false});
                    context.Response.Cookies.Append("cdm-keyLogin", context.Request.Query["keyLogin"].ToString() ?? "", new CookieOptions
                    {MaxAge = TimeSpan.FromDays(365),HttpOnly = false});
                    ApiResponse rta = await _api.api_getToken(context.Request.Query["codigoAutorizacion"].ToString());
                    if (rta.status == 200)
                    {
                        Console.WriteLine(rta.data?["token"]);
                        context.Response.Cookies.Append("cdm-token", rta.data?["token"], new CookieOptions
                        {MaxAge = TimeSpan.FromDays(365),HttpOnly = false});
                        ApiResponse perfil = await _api.api_getPerfil(rta.data?["token"]);
                        if (perfil.status == 200)
                        {
                            Console.WriteLine(perfil.data?["id"]);
                            Requestcdm.idColeccion = perfil.data?["id"];
                            context.Response.Cookies.Append("cdm-idColeccion", perfil.data?["id"], new CookieOptions
                            {MaxAge = TimeSpan.FromDays(365),HttpOnly = false});
                            Requestcdm!.data!.Add("getPerfil", perfil);
                        }
                        Requestcdm.status = perfil.status;
                        Requestcdm.msg = perfil.msg;
                        Requestcdm.token = rta.data?["token"];
                        Requestcdm!.data!.Add("origenUri", context.Request.Query["origenUri"].ToString());
                        context.Items["cdm"] = Requestcdm;    
                        await _next(context);
                        return;
                    }
                }
                if (context.Request.Query.ContainsKey("tipo") && context.Request.Query["tipo"].ToString() == "solicitud")
                {
                   Console.WriteLine(context.Request.Query["metodo"].ToString());
                        if(context.Request.Query["metodo"].ToString() == "facetec"){
                            context.Response.Cookies.Append("cdm-solicitud-facetec", context.Request.Query["solicitud"].ToString() ?? "", new CookieOptions
                            {MaxAge = TimeSpan.FromDays(365),HttpOnly = false});
                        }
                        if(context.Request.Query["metodo"].ToString() == "otp"){
                            context.Response.Cookies.Append("cdm-solicitud-otp", context.Request.Query["solicitud"].ToString() ?? "", new CookieOptions
                            {MaxAge = TimeSpan.FromDays(365),HttpOnly = false});
                        }
                        Console.WriteLine(context.Request.Query["origenUri"].ToString());
                        context.Response.Redirect(context.Request.Query["origenUri"].ToString() + "?solicitud=" + context.Request.Query["solicitud"].ToString());
                        return;
                }
                cdm.status = 400;
                cdm.msg ="tipo de callback no reconocido";
                Requestcdm.msg=cdm.msg;
                Requestcdm.status=cdm.status;
                Requestcdm!.data!.Add("getPerfil", cdm);
                Requestcdm!.data!.Add("origenUri", context.Request.Query["origenUri"].ToString() ?? "");
                context.Items["cdm"] = Requestcdm;    
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                CDMRequest cdm = new CDMRequest();
                CDMRequest Requestcdm = (CDMRequest)context.Items["cdm"]!;
                cdm.status = 500;
                cdm.msg ="Error inesperado";
                Requestcdm.msg=cdm.msg;
                Requestcdm.status=cdm.status;
                Requestcdm!.data!.Add("getPerfil", cdm);
                Requestcdm!.data!.Add("origenUri", context.Request.Query["origenUri"].ToString() ?? "");
                context.Items["cdm"] = Requestcdm;    
            }
        }
        await _next(context);
        return;
    }
}