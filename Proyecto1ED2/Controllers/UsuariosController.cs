using Newtonsoft.Json;
using Proyecto1ED2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net.Http.Formatting;
using System.Dynamic;
using System.Threading.Tasks;

namespace Proyecto1ED2.Controllers
{
    public class UsuariosController : Controller
    {
        static string username;
        static string receptor;
        #region Cargar views
        public ActionResult CrearUsuario()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Principal()
        {
            return View();
        }

        public async Task<ActionResult> Chat(string id)
        {
            receptor = id;
            string guidSala;
            var response = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/GuidSala/" + username + "/" + receptor);
            if (response == "") //aun no hay sala con esa persona
            {
                Sala sala1 = new Sala();
                sala1.UsuarioA = username;
                sala1.UsuarioB = receptor;

                Sala sala2 = sala1;
                sala2.UsuarioA = receptor;
                sala2.UsuarioB = username;
            }
            else // ya hay una sala, recuperar mensajes y mandarlos a vista chat 
            {

            }
            return View();
        }

        public ActionResult BuscarMensaje()
        {
            return View();
        }

        public ActionResult Historial()
        {
            return View();
        }

        public async Task<ActionResult> MenuContactos()
        {
            List<Usuario> contactos = new List<Usuario>();
            var response = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/Contactos");
            var listJsons = JsonConvert.DeserializeObject<List<Usuario>>(response);
            
            foreach(var item in listJsons)
            {
                Usuario contacto = new Usuario();
                contacto.Nombre = item.Nombre + " " + item.Apellido;
                contacto.User = item.User;
                contactos.Add(contacto);
            }

            return View(contactos);
        }

        #endregion

        HttpClient ClienteHttp = new HttpClient();

        #region Action Results botones 
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            var user = new Usuario();
            user.User = collection["User"];
            user.Password = collection["Password"];

            var json = JsonConvert.SerializeObject(user);
            var jsonContent = new System.Net.Http.StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = GlobalVariables.WebApiClient.PostAsync("https://localhost:44343/api/main/Login" + "/" + user.User + "/" + user.Password, jsonContent).Result;
            if (response.ReasonPhrase == "OK")
            {
                username = user.User;
                return View("Principal");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult CrearUsuario(FormCollection collection)
        {
            try
            {
                var newUser = new Usuario();
                newUser.Nombre = collection["Nombre"];
                newUser.Apellido = collection["Apellido"];
                newUser.Password = collection["Password"];
                newUser.User = collection["User"];
                newUser.EMail = collection["EMail"];

                var json = JsonConvert.SerializeObject(newUser);
                var jsonContent = new System.Net.Http.StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var response = GlobalVariables.WebApiClient.PostAsync("https://localhost:44343/api/main/Nuevo" + "/" + newUser.Nombre + "/" + newUser.Apellido + "/" + newUser.Password + "/" + newUser.User + "/" + newUser.EMail, jsonContent).Result;
                if (response.ReasonPhrase == "OK")
                {
                    return View("Index");
                }
                else
                {
                    return View("CrearUsuario");
                }
            }
            catch
            {
                return View("CrearUsuario");
            }
        }


        #endregion
    }
}