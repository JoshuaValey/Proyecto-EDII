using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Models
{
    public class Mensaje
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public int ID { get; set; }
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
        [BsonElement("salaGuid")]
        public string SalaGuid { get; set; }
        public Mensaje(string salaGuid)
        {
            this.Guid = System.Guid.NewGuid().ToString();
            this.SalaGuid = salaGuid;
        }
    }
}