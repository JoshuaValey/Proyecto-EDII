using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Proyecto1ED2.Models
{
    public class Mensaje
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string ID { get; set; }
        [BsonElement("guid")]
        public string Guid { get; set; }
        [BsonElement("usuarioEmisor")]
        public string UsuarioEmisor { get; set; }
        [BsonElement("usuarioReceptor")]
        public string UsuarioReceptor { get; set; }
        [BsonElement("contenido")]
        public string Contenido { get; set; }
        [BsonElement("contenidoArchivos")]
        public List<string> ContenidoArchivos { get; set; }

        public Mensaje()
        {
            this.Guid = System.Guid.NewGuid().ToString();
        }
    }
}