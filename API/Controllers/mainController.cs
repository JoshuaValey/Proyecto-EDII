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

        [HttpPost]
        [Route("Buscar/{palabraclave}/{username}")]
        public List<string> buscarMensajes(string palabraclave, string username)
        {
            List<string> mensajesEncontrados = new List<string>();

            DbConnection connection = new DbConnection();
            var db = connection.Client.GetDatabase(connection.DBName);
            var usersCollection = db.GetCollection<Mensaje>("mensajes");
            var filter = Builders<Mensaje>.Filter.Eq("UsuarioEmisor", username);
            List<Mensaje> enviadosLog = usersCollection.Find(filter).ToList();
            var filter2 = Builders<Mensaje>.Filter.Eq("UsuarioReceptor", username);
            List<Mensaje> recibidosLog = usersCollection.Find(filter2).ToList();
            List<Mensaje> encontrados = buscarCoincidencias(enviadosLog, recibidosLog, palabraclave);
            return mensajesEncontrados;
        }

        static List<Mensaje> buscarCoincidencias(List<Mensaje> mensajesEnviados, List<Mensaje> mensajesRecibidos, string palabraclave)
        {
            List<Mensaje> coincidencias = new List<Mensaje>();

            foreach(var item in mensajesEnviados)
            {
                string[] split = item.Contenido.Split(' ');
                for(int i = 0; i < split.Length; i++)
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
