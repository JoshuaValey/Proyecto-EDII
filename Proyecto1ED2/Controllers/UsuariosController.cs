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
using System.IO;
using System.Web.Script.Serialization;
using System.Net.Mime;

namespace Proyecto1ED2.Controllers
{
    public class UsuariosController : Controller
    {
        static string username;
        static string receptor;
        static string palabraBuscada;

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


        public async Task<ActionResult> Recargar()
        {
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
                return View("Chat",mensajes);
            }
        }
        public async Task<ActionResult> Chat(string id)
        {
            receptor = id;

            if (receptor != username)
            {
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

        public ActionResult BuscarMensaje()
        {
            return View();
        }

        public async Task<ActionResult> Historial()
        {
            var response = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/Historial/" + username);
            var listaMensajes = JsonConvert.DeserializeObject<List<Mensaje>>(response);
            var historial = new List<string>();

            if (listaMensajes.Count != 0)
            {

                /*var salaResponse = await GlobalVariables.WebApiClient.GetStringAsync(
                    "https://localhost:44343/api/main/GetSala/" + usuarioA + "/" + usuarioB);
                var salaEncontrada = JsonConvert.DeserializeObject<Sala>(salaResponse);*/

                foreach (var item in listaMensajes)
                {


                    string usuarioa = item.UsuarioEmisor;
                    var queryA = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/ObtenerUsuario/" + usuarioa);
                    var usuarioA = JsonConvert.DeserializeObject<Usuario>(queryA);
                    string usuariob = item.UsuarioReceptor;
                    var queryB = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/ObtenerUsuario/" + usuariob);
                    var usuarioB = JsonConvert.DeserializeObject<Usuario>(queryB);

                    DiffieHellman DhUsuarioA = new DiffieHellman(usuarioA.NumeroPrivado);
                    DiffieHellman DhUsuarioB = new DiffieHellman(usuarioB.NumeroPrivado);
                    DhUsuarioB.PublicoExterno = DhUsuarioA.PublicoInterno;

                    string llaveSdes = Convert.ToString(DhUsuarioB.GenerarKey(), 2).PadLeft(10, '0');

                    Sdes cipher = new Sdes(llaveSdes);



                    string mensajeDes = "";
                    foreach (var character in item.Contenido)
                    {
                        byte letra = Convert.ToByte(character);
                        mensajeDes += Convert.ToChar(cipher.SDES_DeCipher(letra));
                    }

                    historial.Add(item.UsuarioEmisor + " a " + item.UsuarioReceptor + ": " + mensajeDes);
                }

            }
            else
            {
                Response.Write("<script>alert('El historial está vacío!!')</script>");
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

        public ActionResult Archivo()
        {
            DbConnection connection = new DbConnection();
            var db = connection.Client.GetDatabase(connection.DBName);
            var usersCollection = db.GetCollection<Archivo>("archivos");
            var filter = Builders<Archivo>.Filter.Eq("emisor", username);
            List<Archivo> archivosEnviados = usersCollection.Find(filter).ToList();

            var filter2 = Builders<Archivo>.Filter.Eq("receptor", username);
            List<Archivo> archivosRecibidos = usersCollection.Find(filter2).ToList();

            foreach(var item in archivosRecibidos)
            {
                archivosEnviados.Add(item);
            }

            return View(archivosEnviados);
        }

        public async Task<FileStreamResult> Temporal(string id)
        {
            DbConnection connection = new DbConnection();
            var db = connection.Client.GetDatabase(connection.DBName);
            var usersCollection = db.GetCollection<Archivo>("archivos");
            var filter = Builders<Archivo>.Filter.Eq("nombre", id);
            List<Archivo> archivo = usersCollection.Find(filter).ToList();

            return await DescargarArchivo(archivo[0].Ruta);
        }
        #endregion

        #region Action Results botones 

        public async Task<FileStreamResult> DescargarArchivo(string ruta)
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(ruta, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, MediaTypeNames.Application.Octet, Path.GetFileName(ruta));
        }

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
                if(collection["Nombre"] != "" && collection["Apellido"]!="" && collection["Password"]!="" && collection["User"]!="" && collection["EMail"] != "")
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
                else
                {
                    Response.Write("<script>alert('No se llenaron todos los campos solicitados.')</script>");
                    return View("CrearUsuario");
                }
            }
            catch
            {
                return View("CrearUsuario");
            }
        }

        [HttpPost]
        public async Task<ActionResult> PalabraClave(FormCollection collection)
        {
            palabraBuscada = collection["palabra"];
            try
            {
                string palabra = palabraBuscada;
                List<string> mensajesEncontrados = new List<string>();
                var response = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/Buscar/" + palabra + "/" + username);
                var encontrado = JsonConvert.DeserializeObject<List<Mensaje>>(response);

                foreach (var item in encontrado)
                {
                    mensajesEncontrados.Add(item.UsuarioEmisor + " a " + item.UsuarioReceptor + ": " + item.Contenido);
                }

                return View("BuscarMensaje",mensajesEncontrados);
            }
            catch (Exception e)
            {
                string error = e.Message;
                Console.WriteLine(error);
                return View("index");
            }
            //return View("BuscarMensaje");
        }

        [HttpPost]
        public async Task<ActionResult> Chat(FormCollection collection, HttpPostedFileBase file)
        {
            string mensaje = collection["mensaje"];
            Mensaje nuevo = new Mensaje();
            nuevo.Contenido = mensaje;

            if (mensaje != "" && mensaje != null)
            {
                var json = JsonConvert.SerializeObject(nuevo);
                var jsonContent = new System.Net.Http.StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var response =  GlobalVariables.WebApiClient.PostAsync("https://localhost:44343/api/main/NuevoMensaje" + "/" + username + "/" + receptor + "/"+ mensaje, jsonContent ).Result;

            }
            else if (file!=null)
            {
                string ruta = Server.MapPath("~/Archivos/");

                string rutaUsuario = Server.MapPath("");
                string archivo = ruta + Path.GetFileName(file.FileName);
                var fileName = Path.GetFileName(file.FileName).Substring(0, Path.GetFileName(file.FileName).IndexOf("."));
                file.SaveAs(archivo);

                var infoArchivo = new Archivo();
                infoArchivo.Emisor = username;
                infoArchivo.Receptor = receptor;
                infoArchivo.Ruta = archivo;
                infoArchivo.Nombre = file.FileName;

                DbConnection connection = new DbConnection();
                connection.InsertDb<Archivo>("archivos", infoArchivo);
            }

            var guidResponse = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/GuidSala/" + username + "/" + receptor);

            var response2 = await GlobalVariables.WebApiClient.GetStringAsync("https://localhost:44343/api/main/Recuperar/" + guidResponse + "/" + username);
            var mensajesDesEncriptados = JsonConvert.DeserializeObject<List<Mensaje>>(response2);
            List<string> mensajes = new List<string>();
            foreach (var item in mensajesDesEncriptados)
            {
                mensajes.Add(item.UsuarioEmisor + ": " + item.Contenido);
            }

            ViewBag.Amigo = receptor;
            return View(mensajes);

           // RedirectToAction("Chat",receptor);
        }
        #endregion
    }
}