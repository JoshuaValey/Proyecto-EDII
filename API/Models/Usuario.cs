﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Models
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
       
        [BsonElement("numeroPrivado")]
        public int NumeroPrivado { get; set; }

        [BsonElement("contactos")]
        public List<string> Contactos = new List<string>();

        [BsonElement("guid")]
        public string _Guid { get; set; }

        public Usuario()
        {
            this._Guid = System.Guid.NewGuid().ToString();
            GenerarPrivado();
        }

        public void GenerarPrivado()
        {
            Random rnd = new Random();
            byte value = (byte)rnd.Next(0, 255);

            //Número privado para DH
            this.NumeroPrivado = value;

        }
    }
}
