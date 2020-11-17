using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaMensajeria.Cifrados
{
    internal class Cesar
    {
        private string UpperAbc;
        private string LowerAbc;
        private string EncriptUpperAbc;
        private string EncriptLowerAbc;

        public Cesar(string key)
        {
            this.UpperAbc = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";
            this.LowerAbc = this.UpperAbc.ToLower();
            this.EncriptUpperAbc = GenerarABCifrado(key.ToUpper());
            this.EncriptLowerAbc = this.EncriptUpperAbc.ToLower();
        }

        private string GenerarABCifrado(string key)
        {
            string auxiliar = "";
            foreach (var item in key)
            {
                if (!auxiliar.Contains(item))
                {
                    auxiliar += item;
                }
            }
            string retorno = auxiliar;

            foreach (var item in this.UpperAbc)
            {
                if (!retorno.Contains(item))
                {
                    retorno += item;
                }
            }

            return retorno;
        }

        public string Cifrar(string cadena) => AlgoritmoCesar(cadena, this.UpperAbc, this.EncriptUpperAbc,
                                                                                this.LowerAbc, this.EncriptLowerAbc);


        public string DesCifrar(string cadena) => AlgoritmoCesar(cadena, this.EncriptUpperAbc, this.UpperAbc,
                                                                                this.EncriptLowerAbc, this.LowerAbc);


        Dictionary<char, char> MakeDiccionary(string originAbc, string changedAbc)
        {
            var retorno = new Dictionary<char, char>();

            for (int i = 0; i < originAbc.Length; i++)
                retorno.Add(originAbc[i], changedAbc[i]);

            return retorno;
        }

        private string AlgoritmoCesar(string cadena, string uClave, string uValor,
            string lClave, string lValor)
        {
            string salida = "";

            var upperDic = MakeDiccionary(uClave, uValor);
            var lowerDic = MakeDiccionary(lClave, lValor);

            foreach (var item in cadena)
            {
                if (upperDic.ContainsKey(item))
                    salida += upperDic[item];
                else if (lowerDic.ContainsKey(item))
                    salida += lowerDic[item];
                else
                    salida += item;
            }
            return salida;
        }
    }
}
