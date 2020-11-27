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
            string guidSala;
            var guidResponse = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/GuidSala/" + username + "/" + receptor);
            DbConnection connection = new DbConnection();
            if (guidResponse == "") //aun no hay sala con esa persona
            {

                Sala newSala = new Sala();
                Sala newSala2 = new Sala();
                newSala.UsuarioA = username;
                newSala.UsuarioB = receptor;
                newSala2.UsuarioA = receptor;
                newSala2.UsuarioB = username;
                newSala2.GUID = newSala.GUID;
                //Buscar los usuarios y obtener sus valores publicos... 
                var filtro = Builders<Usuario>.Filter.Eq("user", username);
                var usuarioA = connection.BuscarUno<Usuario>("users", filtro);
                var filtroB = Builders<Usuario>.Filter.Eq("user", receptor);
                var usuarioB = connection.BuscarUno<Usuario>("users", filtroB);

                DiffieHellman PersonaA = new DiffieHellman(usuarioA.NumeroPrivado);
                DiffieHellman PersonaB = new DiffieHellman(usuarioB.NumeroPrivado);

                newSala.ValorPublicoA = PersonaA.PublicoInterno;
                newSala.ValorPublicoB = PersonaB.PublicoInterno;
                newSala2.ValorPublicoA = PersonaB.PublicoInterno;
                newSala2.ValorPublicoB = PersonaA.PublicoInterno;
                connection.InsertDb<Sala>("salas", newSala);
                connection.InsertDb<Sala>("salas", newSala2);
                List<string> mensajes = new List<string>();
                return View(mensajes);
            }
            else // ya hay una sala, recuperar mensajes y mandarlos a vista chat 
            {
                List<Sala> salasDeUsuarios = connection.BuscarVarios<Sala>("salas", Builders<Sala>.Filter.Eq("guid", guidResponse));
                var filtro = Builders<Usuario>.Filter.Eq("user", username);
                var usuarioActual = connection.BuscarUno<Usuario>("users", filtro);
                Sala salaActual = new Sala();
                foreach (var item in salasDeUsuarios)
                {
                    if (item.UsuarioA == usuarioActual.User)
                    {
                        salaActual = item;
                        break;
                    }
                }

                //Generar key DH para generar 10bitsSDES
                DiffieHellman PersonaA = new DiffieHellman(usuarioActual.NumeroPrivado)
                {
                    PublicoExterno = salaActual.ValorPublicoB
                };

                string cadenaLlaveSdes = Convert.ToString(PersonaA.GenerarKey(), 2).PadLeft(10, '0');
                Sdes cipher = new Sdes(cadenaLlaveSdes);

                List<Mensaje> mensajesEncriptados = connection.BuscarVarios<Mensaje>("mensajes", 
                    Builders<Mensaje>.Filter.Eq("salaGuid", guidResponse));

                List<Mensaje> mensajesDesEncriptados = new List<Mensaje>();
                //Des encriptar los mensajes
                foreach (var item in mensajesEncriptados)
                {
                    string desEnc = "";
                    foreach (var caracter in item.Contenido)
                    {
                        byte letra = Convert.ToByte(caracter);
                        desEnc += Convert.ToChar(cipher.SDES_DeCipher(letra));

                    }
                    Mensaje mensajeDes = new Mensaje
                    {
                        Contenido = desEnc,
                        Guid = item.Guid,
                        UsuarioEmisor = item.UsuarioEmisor,
                        UsuarioReceptor = item.UsuarioReceptor
                    };
                    mensajesDesEncriptados.Add(mensajeDes);
                }

                List<string> mensajes = new List<string>();
                foreach(var item in mensajesDesEncriptados)
                {
                    mensajes.Add(item.UsuarioEmisor + ": " + item.Contenido);
                }

                ViewBag.Amigo = receptor;
                return View(mensajes);
            }

        }

        public ActionResult BuscarMensaje()
        {
            return View();
        }

        public async Task<ActionResult> Historial()
        {
            var response = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/Historial/" + username);
            var listJsons = JsonConvert.DeserializeObject<List<Mensaje>>(response);
            var historial = new List<string>();

            foreach(var item in listJsons)
            {
                historial.Add(item.UsuarioEmisor + "a" + item.UsuarioReceptor + ": " + item.Contenido);
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

        [HttpPost]
        public ActionResult PalabraClave(FormCollection collection)
        {
            string palabra = collection["palabra"];
            return View("BuscarMensaje");
        }
        #endregion
    }
}