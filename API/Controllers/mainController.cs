using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Connection;
using API.Modelos;
using Biblioteca.Estructuras;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            if (user == "marce")
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(500);
            }
            //hacer comprobacion de usuario y contrase;a 
            //devolver si se comprobo o no 
        }

        [HttpPost]
        [Route("Nuevo/{nombre}/{apellido}/{password}/{user}/{email}/{llave}")]
        public IActionResult NuevoUsuario(string nombre, string apellido, string password, string user, string email, string llave)
        {
            Cesar cifrado = new Cesar("Centrifugados");
            DbConnection mongo = new DbConnection();
            try
            {
                users newUser = new users();
                newUser.Nombre = nombre;
                newUser.Apellido = apellido;
                newUser.Password = cifrado.Cifrar(password);
                newUser.User = user;
                newUser.EMail = email;
                newUser.LlaveSDES = llave;

                var json = JsonConvert.SerializeObject(newUser);
                mongo.InsertDb<users>("users", newUser);
                return StatusCode(200);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
