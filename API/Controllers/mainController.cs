using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Connection;
using API.Models;
using Biblioteca.Estructuras;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using MongoDB.Bson;


namespace API.Controllers
{
    [Route("api/main")]
    [ApiController]
    public class mainController : ControllerBase
    {
        [HttpPost]
        [Route("Login/{user}/{password}")]
        public IActionResult Login(string user, string password)
        {
            Cesar cifrado = new Cesar("Centrifugados");

            DbConnection connection = new DbConnection();
            var db = connection.Client.GetDatabase(connection.DBName);
            var usersCollection = db.GetCollection<Usuario>("users");
            var filter = Builders<Usuario>.Filter.Eq("user", user);
            List<Usuario> usuariosLog = usersCollection.Find<Usuario>(filter).ToList();
            Usuario userLog = UserLog(usuariosLog, user, cifrado.Cifrar(password));
            if (userLog != null)
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("Nuevo/{nombre}/{apellido}/{password}/{user}/{email}")]
        public IActionResult NuevoUsuario(string nombre, string apellido, string password, string user, string email)
        {
            Cesar cifrado = new Cesar("Centrifugados");
            DbConnection mongo = new DbConnection();
            try
            {
                Usuario newUser = new Usuario();
                newUser.Nombre = nombre;
                newUser.Apellido = apellido;
                newUser.Password = cifrado.Cifrar(password);
                newUser.User = user;
                newUser.EMail = email;


                DbConnection connection = new DbConnection();
                var db = connection.Client.GetDatabase(connection.DBName);
                var usersCollection = db.GetCollection<Usuario>("users");
                var filter = Builders<Usuario>.Filter.Eq("user", newUser.User);
                List<Usuario> usuarios = usersCollection.Find<Usuario>(filter).ToList();
                Usuario userCreate = UserCreate(usuarios, newUser.User);

                if (userCreate != null)//Se encontró usuario existente
                {
                    return StatusCode(500);
                }
                else //No se encontro el usuario, crear uno nuevo. 
                {
                    var json = JsonConvert.SerializeObject(newUser);
                    mongo.InsertDb<Usuario>("users", newUser);
                    return StatusCode(200);
                }



            }
            catch
            {
                return StatusCode(500);
            }

        }

        [HttpGet]
        [Route("Buscar/{palabraclave}/{username}")]
        public string buscarMensajes(string palabraclave, string username)
        {
            //string retorno = "null";
            DbConnection connection = new DbConnection();
            var db = connection.Client.GetDatabase(connection.DBName);
            var usersCollection = db.GetCollection<Mensaje>("mensajes");
            var filter = Builders<Mensaje>.Filter.Eq("UsuarioEmisor", username);
            List<Mensaje> enviadosLog = usersCollection.Find(filter).ToList();

            List<Mensaje> enviadosLogDes = ListaMensajesDesEncriptados(enviadosLog, connection);
            
            var filter2 = Builders<Mensaje>.Filter.Eq("UsuarioReceptor", username);
            List<Mensaje> recibidosLog = usersCollection.Find(filter2).ToList();

            List<Mensaje> recibidisLogDes = ListaMensajesDesEncriptados(recibidosLog, connection);

            List<Mensaje> encontrados = buscarCoincidencias(enviadosLogDes, recibidisLogDes, palabraclave);
            var json = JsonConvert.SerializeObject(encontrados);

            /* if (json.Length > 2) 
             {
                 retorno = json;
             }*/

            return json;
        }

        List<Mensaje> ListaMensajesDesEncriptados(List<Mensaje> encriptList, DbConnection connection)
        {
            List<Mensaje> retorno = new List<Mensaje>();
            foreach (var item in encriptList)
            {
                var emisor = item.UsuarioEmisor;
                var usuarioA = connection.BuscarUno<Usuario>("users", Builders<Usuario>.Filter.Eq("user", emisor));
                var receptor = item.UsuarioReceptor;
                var usuarioB = connection.BuscarUno<Usuario>("users", Builders<Usuario>.Filter.Eq("user", receptor));

                DiffieHellman DhUsuarioA = new DiffieHellman(usuarioA.NumeroPrivado);
                DiffieHellman DhUsuarioB = new DiffieHellman(usuarioB.NumeroPrivado);
                DhUsuarioB.PublicoExterno = DhUsuarioA.PublicoInterno;

                string llaveSdes = Convert.ToString(DhUsuarioB.GenerarKey(), 2).PadLeft(10, '0');
                Sdes cipher = new Sdes(llaveSdes);

                string mensajeDesc = "";
                foreach (var caracter in item.Contenido)
                {
                    byte letra = Convert.ToByte(caracter);
                    mensajeDesc += Convert.ToChar(cipher.SDES_DeCipher(letra));
                }
                Mensaje desEncriptado = new Mensaje(item.SalaGuid)
                {
                    Contenido = mensajeDesc,
                    Guid = item.Guid,
                    UsuarioEmisor = item.UsuarioEmisor,
                    UsuarioReceptor = item.UsuarioReceptor,
                    ContenidoArchivos = item.ContenidoArchivos
                };

                retorno.Add(desEncriptado);
            }

            return retorno;
        }



        [HttpPost]
        [Route("Chat/{amigo}/{username}")]
        public List<Mensaje> Chat(string amigo, string username)
        {
            List<Mensaje> chat = new List<Mensaje>();
            DbConnection connection = new DbConnection();
            var db = connection.Client.GetDatabase(connection.DBName);
            var usersCollection = db.GetCollection<Mensaje>("mensajes");
            var filter = Builders<Mensaje>.Filter.Eq("", username);
            chat = usersCollection.Find(filter).ToList();
            return chat;
        }

        [HttpGet]
        [Route("Contactos")]
        public string Contactos()
        {
            DbConnection connection = new DbConnection();
            var db = connection.Client.GetDatabase(connection.DBName);

            var contactoss = connection.GetAllDocumets<Usuario>("users");
            var json = JsonConvert.SerializeObject(contactoss);

            return json;
        }

        [HttpPost]
        [Route("NuevoMensaje/{emisor}/{receptor}/{contenido}")]
        public void NuevoMensaje(string emisor, string receptor, string contenido)
        {
            
            DbConnection connection = new DbConnection();
            //Obtener sala
            var filter = Builders<Sala>.Filter.Eq("usuarioA", emisor);
            var salas = connection.BuscarVarios<Sala>("salas", filter);
            Sala sala = new Sala();
            //Obtener la sala que coincide con emisor y receptor... 
            foreach (var item in salas)
            {
                if (item.UsuarioB == receptor)
                {
                    sala = item;
                }
            }

            //Obtener el GUID de la sala 
            string guidSala = sala.GUID;
            //Obtener usuario para encriptar con valor privado...
            var userFilter = Builders<Usuario>.Filter.Eq("user", emisor);
            var usuario = connection.BuscarUno<Usuario>("users", userFilter);
            //Recuperar valores publicos y provados para Encriptar...
            int valorPublicoReceptor = sala.ValorPublicoB;
            int valorPrivadoEmisor = usuario.NumeroPrivado;

            DiffieHellman dh = new DiffieHellman(valorPrivadoEmisor);
            dh.PublicoExterno = valorPublicoReceptor;

            string LlaveSdes = Convert.ToString(dh.GenerarKey(), 2).PadLeft(10, '0');
            Sdes cipher = new Sdes(LlaveSdes);

            //Encriptar mensaje con SDES
            string mensajeEncrip = "";
            foreach (var item in contenido)
            {
                byte letra = Convert.ToByte(item);
                mensajeEncrip += Convert.ToChar(cipher.SDES_Cipher(letra));
            }

            Mensaje nuevoMensaje = new Mensaje(guidSala)
            {
                UsuarioEmisor = emisor,
                UsuarioReceptor = receptor,
                Contenido = mensajeEncrip

            };

            connection.InsertDb<Mensaje>("mensajes", nuevoMensaje);


        }

        [HttpGet]
        [Route("GuidSala/{persona1}/{persona2}")]
        public string Guid(string persona1, string persona2)
        {
            string guid = "";
            var salas = ObtenerSalas(persona1);

            foreach (var item in salas)
            {
                if (item.UsuarioB == persona2)
                {
                    guid = item.GUID;
                    break;
                }
            }
            return guid;
        }

        [HttpGet]
        [Route("GetSala/{persona1}/{persona2}")]
        public string GetSala(string persona1, string persona2)
        {
            Sala response = new Sala();
            var salas = ObtenerSalas(persona1);

            foreach (var item in salas)
            {
                if (item.UsuarioB == persona2)
                {
                    response = item;
                }
            }
            return JsonConvert.SerializeObject(response);
        }

       public List<Sala> ObtenerSalas(string persona1)
        {
            DbConnection connection = new DbConnection();
            
            var filter = Builders<Sala>.Filter.Eq("usuarioA", persona1);
            
            return connection.BuscarVarios<Sala>("salas", filter);

        }


        [HttpGet]
        [Route("Historial/{username}")]
        public string HistorialUser(string username)
        {
            DbConnection connection = new DbConnection();
            var db = connection.Client.GetDatabase(connection.DBName);
            var usersCollection = db.GetCollection<Mensaje>("mensajes");
            var filter = Builders<Mensaje>.Filter.Eq("UsuarioEmisor", username);
            List<Mensaje> enviadosLog = usersCollection.Find(filter).ToList();
            var filter2 = Builders<Mensaje>.Filter.Eq("UsuarioReceptor", username);
            List<Mensaje> recibidosLog = usersCollection.Find(filter2).ToList();

            foreach (var item in recibidosLog)
            {
                enviadosLog.Add(item);
            }

            var json = JsonConvert.SerializeObject(enviadosLog);

            return json;
        }

        [HttpPost]
        [Route ("NuevaSala/{username}/{receptor}")]
        public IActionResult CrearSala(string username, string receptor)
        {
            try
            {
                DbConnection connection = new DbConnection();
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
                return StatusCode(200);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route ("Recuperar/{guidResponse}/{username}")]
        public string RecuperarMensajesSala(string guidResponse, string username)
        {
            try
            {
                DbConnection connection = new DbConnection();
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
                    Mensaje mensajeDes = new Mensaje(guidResponse)
                    {
                        Contenido = desEnc,
                        Guid = item.Guid,
                        UsuarioEmisor = item.UsuarioEmisor,
                        UsuarioReceptor = item.UsuarioReceptor
                    };
                    mensajesDesEncriptados.Add(mensajeDes);
                }

                var json = JsonConvert.SerializeObject(mensajesDesEncriptados);
                return json;
            }
            catch
            {
                return "";
            }
        }

        [HttpGet]
        [Route("ObtenerUsuario/{usuario}")]
        public string ObtenerUsuario(string usuario)
        {
            DbConnection connection = new DbConnection();
            var filter = Builders<Usuario>.Filter.Eq("user", usuario);
            var usuarioResponse = connection.BuscarUno<Usuario>("users", filter);
            return JsonConvert.SerializeObject(usuarioResponse);
        }

        static List<Mensaje> buscarCoincidencias(List<Mensaje> mensajesEnviados, List<Mensaje> mensajesRecibidos, string palabraclave)
        {
            List<Mensaje> coincidencias = new List<Mensaje>();

            foreach (var item in mensajesEnviados)
            {
                string[] split = item.Contenido.Split(' ');
                for (int i = 0; i < split.Length; i++)
                {
                    if (split[i].ToLower() == palabraclave.ToLower()) //si contiene la palabra
                    {
                        coincidencias.Add(item);
                        break;
                    }
                }
            }

            foreach (var item in mensajesRecibidos)
            {
                string[] split = item.Contenido.Split(' ');
                for (int i = 0; i < split.Length; i++)
                {
                    if (split[i].ToLower() == palabraclave.ToLower()) //si contiene la palabra
                    {
                        coincidencias.Add(item);
                        break;
                    }
                }
            }

            return coincidencias;
        }

        static Usuario UserLog(List<Usuario> usuariosLog, string user, string pass)
        {
            Usuario retorno = null;
            int i; bool exist = false;
            for (i = 0; i < usuariosLog.Count; i++)
            {
                if (usuariosLog[i].Password == pass && usuariosLog[i].User == user)
                {
                    exist = true;
                    break;
                }
            }
            if (exist)
            {
                retorno = usuariosLog[i];
            }
            return retorno;
        }

        static Usuario UserCreate(List<Usuario> usuarios, string user)
        {
            Usuario retorno = null;
            int i; bool exist = false;
            for (i = 0; i < usuarios.Count; i++)
            {
                if (usuarios[i].User == user)
                {
                    exist = true;
                    break;
                }
            }
            if (exist)
            {
                retorno = usuarios[i];
            }
            return retorno;
        }
    }
}
