using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace API.Connection
{
    public class DbConnection
    {

        public MongoClient Client { get; set; }
        public string DBName { get; set; }
        public string DBUrl { get; set; }

        public DbConnection(string dbName = "PruebasURL", string dbUrl = "mongodb://Joshua:1014416@pruebasurl-shard-00-00.gwequ.mongodb.net:27017,pruebasurl-shard-00-01.gwequ.mongodb.net:27017,pruebasurl-shard-00-02.gwequ.mongodb.net:27017/PruebasURL?ssl=true&replicaSet=atlas-ku8d37-shard-0&authSource=admin&retryWrites=true&w=majority")
        {

            this.DBName = dbName;
            this.DBUrl = dbUrl;
            this.Client = new MongoClient(this.DBUrl);
        }

        /// <summary>
        /// Insertar un elemento a la base de datos
        /// </summary>
        /// <typeparam name="T">Tipo de dato generico entre los modelos de los documentos</typeparam>
        /// <param name="collectionName">Nombre de la colleccion de MongoDB</param>
        /// <param name="data">Objeto a insertar en la DB</param>
        public void InsertDb<T>(string collectionName, T data)
        {
            try
            {
                var database = this.Client.GetDatabase(this.DBName);
                var collection = database.GetCollection<T>(collectionName);
                collection.InsertOne(data);
            }
            catch (MongoException ex)
            {

                Console.WriteLine(ex.Message.ToString());
                throw;
            }

        }
        /// <summary>
        /// Insertar varios elementos a la base de datos
        /// </summary>
        /// <typeparam name="T">Tipo de dato generico entre los modelos de los documentos</typeparam>
        /// <param name="collectionName">Nombre de la colleccion de MongoDB</param>
        /// <param name="data">Colleccion generica de tipo IEnumerable<T> compuesta de objetos a insertar en la DB</param>
        public void InsertDb<T>(string collectionName, IEnumerable<T> data)
        {
            try
            {
                var database = this.Client.GetDatabase(this.DBName);
                var collection = database.GetCollection<T>(collectionName);
                collection.InsertMany(data);
            }
            catch (MongoException ex)
            {

                Console.WriteLine(ex.Message.ToString());
                throw;
            }

        }
        /// <summary>
        /// Insertar varios elementos a la base de datos
        /// </summary>
        /// <typeparam name="T">Tipo de dato generico entre los modelos de los documentos</typeparam>
        /// <param name="collectionName">Nombre de la colleccion de MongoDB</param>
        /// <param name="data">Arreglo de objetos tipo T a insertar en la DB</param>
        public void InsertDb<T>(string collectionName, T[] data)
        {
            try
            {
                var database = this.Client.GetDatabase(this.DBName);
                var collection = database.GetCollection<T>(collectionName);
                collection.InsertMany(data);
            }
            catch (MongoException ex)
            {

                Console.WriteLine(ex.Message.ToString());
                throw;
            }

        }

        /// <summary>
        /// Obtener todos los documetos de una coleccion de la DB
        /// </summary>
        /// <typeparam name="T">Model object</typeparam>
        /// <param name="collectionName">Nombre de la coleccion de Mongodb</param>
        /// <returns>Listado con todos los objetos de la coleccion</returns>
        public List<T> GetAllDocumets<T>(string collectionName)
        {
            try
            {
                var database = this.Client.GetDatabase(this.DBName);
                var collection = database.GetCollection<T>(collectionName);
                return collection.Find<T>(a => true).ToList<T>();
            }
            catch (MongoException ex)
            {

                Console.WriteLine(ex.Message.ToString());
                throw;
            }

        }
    }
}
