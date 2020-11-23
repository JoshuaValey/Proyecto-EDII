﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Estructuras
{
    public class DiffieHellman
    {
        int ValorPrivdo;
        public int Primo { get; set; }
        public int Generador { get; set; }
        public int PublicoExterno { get; set; }
        public int PublicoInterno { get; set; }
        public int Key { get; set; }
        public DiffieHellman(int basePrivado, int publicoB = 0, int primo = 251, int generador = 7)
        {
            ValorPrivdo = basePrivado;
            this.Primo = primo;
            this.Generador = generador;
            this.PublicoExterno = publicoB;
            this.PublicoInterno = GenerarValorPublico();





        }

        private int GenerarPrivado(int basePrivado)
        {
            throw new NotImplementedException();
        }

        private int GenerarPrimo()
        {
            throw new NotImplementedException();
        }

        private int GenerarGenerador()
        {
            throw new NotImplementedException();
        }

        //Valor A o B
        private int GenerarValorPublico()
        {
            return Math.Abs((int)Math.Pow(this.Generador, this.ValorPrivdo) % this.Primo);

        }

        public int GenerarKey()
        {
            return Math.Abs((int)Math.Pow(this.PublicoExterno, this.ValorPrivdo) % this.Primo);
        }
    }
}
