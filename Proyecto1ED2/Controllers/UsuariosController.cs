using Newtonsoft.Json;
using Proyecto1ED2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Proyecto1ED2.Controllers
{
    public class UsuariosController : Controller
    {
        // GET: Usuarios
        HttpClient ClienteHttp = new HttpClient();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CrearUsuario()
        {
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> IndexAsync(FormCollection collection)
        {
            try
            {
                var user = new Usuario();
                user.User = collection["User"];
                user.Password = collection["Password"];
                //https://localhost:44343/
                var json = JsonConvert.SerializeObject(user);
                //var enviar = Nuevo.User + "/" + Nuevo.Password;
                //Generar Token
                //UsuarioActual= 
                //Verificar Que los campos sean correctos
                var cliente = new HttpClient();

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var uri = "https://localhost:44343/main/Login";
                var respose = await cliente.PostAsync(uri, content);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}