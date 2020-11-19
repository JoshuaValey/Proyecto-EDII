using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BibliotecaMensajeria.Cifrados;

namespace APIApp.Controllers
{
    [ApiController]
    [Route("Chat")]

    public class RecibirInfoController : Controller
    {
        //POST recibe parametro tipo usuario, cifra la contraseña y se envía todo a la base de datos
        [HttpPost("CrearUsuario/{method}")]
        public async Task<IActionResult> CrearUsuario([FromForm] usuario user)
        {
            //key puede averiguarse de un hash
            string key = user.nombreCompleto.Trim();
            Cesar cifrar = new Cesar(key);
            string passwordCifrada = "";

            try
            {
                //antes debo buscar si no se encuentra un usuario igual en la base de datos 
                if (user.password == user.cpassword)
                {
                    //enviar informacion del usuario a base de datos 
                    passwordCifrada = cifrar.Cifrar(user.password);
                }
                else
                {
                    //la contrase;a no es igual, no se confirmo
                }
            }
            catch
            {

            }
            return null;
        }


        //POST recibe usuario y contraseña y recorre la base de datos para verificar que sea correcto y esté creado 

    }
}
