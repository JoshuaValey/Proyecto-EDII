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
using MongoDB.Driver;
using Biblioteca.Estructuras;
using Proyecto1ED2.Connection;

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

            if (receptor != username)
            {
                string guidSala;
                string parajson = "clave";

                var json = JsonConvert.SerializeObject(parajson);
                var jsonContent = new System.Net.Http.StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var guidResponse = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/GuidSala/" + username + "/" + receptor);
                
                if (guidResponse == "") //aun no hay sala con esa persona
                {
                    var response = GlobalVariables.WebApiClient.PostAsync("https://localhost:44343/api/main/NuevaSala/" + username + "/" + receptor, jsonContent).Result;
                    List<string> mensajes = new List<string>();
                    Response.Write("<script>alert('Aun no tiene mensajes con este usuario')</script>");
                    ViewBag.Amigo = receptor;
                    return View(mensajes);
                }
                else // ya hay una sala, recuperar mensajes y mandarlos a vista chat 
                {
                    var response = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/Recuperar/" + guidResponse + "/" + username);
                    var mensajesDesEncriptados = JsonConvert.DeserializeObject<List<Mensaje>>(response);
                    List<string> mensajes = new List<string>();
                    foreach (var item in mensajesDesEncriptados)
                    {
                        mensajes.Add(item.UsuarioEmisor + ": " + item.Contenido);
                    }

                    ViewBag.Amigo = receptor;
                    return View(mensajes);
                }
            }
            else
            {
                Response.Write("<script>alert('No puede enviarse mensajes a usted mismo')</script>");
                return View("Principal");
            }
        }

        public async Task<ActionResult> BuscarMensaje(string palabra)
        {
            try
            {

                List<string> mensajesEncontrados = new List<string>();
                var response = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/Buscar/" + palabra + "/" + username);
                var encontrado = JsonConvert.DeserializeObject<List<Mensaje>>(response);

                foreach (var item in encontrado)
                {
                    mensajesEncontrados.Add(item.UsuarioEmisor + " a " + item.UsuarioReceptor + ": " + item.Contenido);
                }

                return View(mensajesEncontrados);
            }
            catch(Exception e)
            {
                string error = e.Message;
                Console.WriteLine(error);
                return View("index");
            }
        }

        public async Task<ActionResult> Historial()
        {
            var response = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/Historial/" + username);
            var listJsons = JsonConvert.DeserializeObject<List<Mensaje>>(response);
            var historial = new List<string>();

            foreach (var item in listJsons)
            {
                historial.Add(item.UsuarioEmisor + " a " + item.UsuarioReceptor + ": " + item.Contenido);
            }
            return View(historial);
        }

        public async Task<ActionResult> MenuContactos()
        {
            List<Usuario> contactos = new List<Usuario>();
            var response = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/Contactos");
            var listJsons = JsonConvert.DeserializeObject<List<Usuario>>(response);

            foreach (var item in listJsons)
            {
                Usuario contacto = new Usuario();
                contacto.Nombre = item.Nombre + " " + item.Apellido;
                contacto.User = item.User;
                contactos.Add(contacto);
            }

            return View(contactos);
        }

        public ActionResult PalabraClave()
        {
            return View();
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
                Response.Write("<script>alert('Usuario o contraseña incorrectos, inténtelo de nuevo.')</script>");
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
                var response = GlobalVariables.WebApiClient.PostAsync("https://localhost:44343/api/main/Nuevo" + "/" + newUser.Nombre + "/" + 
                    newUser.Apellido + "/" + newUser.Password + "/" + newUser.User + "/" + newUser.EMail, jsonContent).Result;
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

        [HttpPost]
        public async Task PalabraClave(FormCollection collection)
        {
            string palabra = collection["palabra"];
            await BuscarMensaje(palabra);
        }

        [HttpPost]
        public ActionResult Chat(FormCollection collection)
        {
            string mensaje = collection["mensaje"];
            Mensaje nuevo = new Mensaje();
            nuevo.Contenido = mensaje;
            string files = collection["file"];

            if (mensaje != null)
            {

                var json = JsonConvert.SerializeObject(nuevo);
                var jsonContent = new System.Net.Http.StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var response =  GlobalVariables.WebApiClient.PostAsync("https://localhost:44343/api/main/NuevoMensaje" + "/" + username + "/" + receptor + "/"+ mensaje, jsonContent ).Result;
                /*Mensaje nuevoMensaje = new Mensaje();
                nuevoMensaje.UsuarioEmisor = username;
                nuevoMensaje.UsuarioReceptor = receptor;
                nuevoMensaje.Contenido = mensaje;*/

                //Llamar metodo de la API NuevoMensaje
            }
            else if (files != null)
            {

            }
            return View("Chat");
        }
        #endregion
    }
}