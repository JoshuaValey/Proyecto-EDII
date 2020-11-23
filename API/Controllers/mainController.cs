using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Modelos;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        public IActionResult NuevoUsuario(users usuario)
        {
            try
            {

                return StatusCode(200);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
