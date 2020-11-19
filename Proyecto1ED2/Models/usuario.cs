using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto1ED2.Models
{
    public class usuario
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("usuario")]
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Password { get; set; }
        public string User { get; set; }
        public string eMail { get; set; }
        public int LlaveSDES { get; set; }
        public List<string> Contactos = new List<string>();
    }
}