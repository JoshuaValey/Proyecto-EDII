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

namespace Proyecto1ED2.Controllers
{
    public class UsuariosController : Controller
    {
        #region Cargar views
        public ActionResult CrearUsuario()
        {
            return View();
        }

        public ActionResult Index(FormCollection collection)
        {
            return View();
        }

        #endregion
        HttpClient ClienteHttp = new HttpClient();

        #region Action Results botones 
        public ActionResult Logearse(FormCollection collection)
        {
            var user = new Usuario();
            user.User = collection["User"];
            user.Password = collection["Password"];

            var json = JsonConvert.SerializeObject(user);
            var jsonContent = new System.Net.Http.StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = GlobalVariables.WebApiClient.PostAsync("https://localhost:44343/api/main/Login" + "/" + "marce" + "/" + "hola", jsonContent).Result;
            if (response.ReasonPhrase == "OK")
            {
                return View("Index");
            }
            else
            {
                return View("Index");
            }
        }
        #endregion
    }
}