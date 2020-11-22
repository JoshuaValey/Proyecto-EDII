using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto1ED2.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; }
        [BsonElement("apellido")]
        public string Apellido { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonElement("user")]
        public string User { get; set; }
        [BsonElement("email")]
        public string EMail { get; set; }
        [BsonElement("llaveSdes")]
        public string LlaveSDES { get; set; }
        [BsonElement("numeroPrivado")]
        public int NumeroPrivado { get; set; }

        [BsonElement("contactos")]
        public List<string> Contactos = new List<string>();

        [BsonElement("guid")]
        public string _Guid { get; set; }

        public Usuario(){
            this._Guid = System.Guid.NewGuid().ToString();
        }
    }
}