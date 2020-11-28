using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto1ED2.Models
{
    public class Archivo
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string ID { get; set; }
        [BsonElement("emisor")]
        public string Emisor { get; set; }
        [BsonElement("receptor")]
        public string Receptor { get; set; }
        [BsonElement("ruta")]
        public string Ruta { get; set; }
        [BsonElement("nombre")]
        public string Nombre { get; set; }
    }
}