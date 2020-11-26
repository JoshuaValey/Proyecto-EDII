using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace API.Models
{
    public class Sala
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string ID { get; set; }

        [BsonElement("guid")]
        public string GUID { get; set; }

        [BsonElement("usuarioA")]
        public string UsuarioA { get; set; }
        [BsonElement("usuarioB")]
        public string UsuarioB { get; set; }
        [BsonElement("valorPublicoA")]
        public string ValorPublicoA { get; set; }
        [BsonElement("valorPublicoB")]
        public string ValorPublicoB { get; set; }
        [BsonElement("mensajes")]
        public List<string> Mensajes { get; set; }


        public Sala()
        {
            this.GUID = System.Guid.NewGuid().ToString();
        }
    }
}
