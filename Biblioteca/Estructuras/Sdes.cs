using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Estructuras
{
    public class Sdes
    {
        private string[,] SwapBox0 = new string[4, 4];
        private string[,] SwapBox1 = new string[4, 4];
        private string k1;
        private string k2;
        public string TenBitsInput { get; set; }

        public Sdes(string keysInput)
        {
            string cero = "00", uno = "01", dos = "10", tres = "11";

            //Inicializar swapBox0

            SwapBox0[0, 0] = uno; SwapBox0[0, 1] = cero; SwapBox0[0, 2] = tres; SwapBox0[0, 3] = dos;
            SwapBox0[1, 0] = tres; SwapBox0[1, 1] = dos; SwapBox0[1, 2] = uno; SwapBox0[1, 3] = cero;
            SwapBox0[2, 0] = cero; SwapBox0[2, 1] = dos; SwapBox0[2, 2] = uno; SwapBox0[2, 3] = tres;
            SwapBox0[3, 0] = tres; SwapBox0[3, 1] = uno; SwapBox0[3, 2] = tres; SwapBox0[3, 3] = dos;

            //Inicializar swapBox1

            SwapBox1[0, 0] = cero; SwapBox1[0, 1] = uno; SwapBox1[0, 2] = dos; SwapBox1[0, 3] = tres;
            SwapBox1[1, 0] = dos; SwapBox1[1, 1] = cero; SwapBox1[1, 2] = uno; SwapBox1[1, 3] = tres;
            SwapBox1[2, 0] = tres; SwapBox1[2, 1] = cero; SwapBox1[2, 2] = uno; SwapBox1[2, 3] = cero;
            SwapBox1[3, 0] = dos; SwapBox1[3, 1] = uno; SwapBox1[3, 2] = cero; SwapBox1[3, 3] = tres;

            keyGenerator(ref this.k1, ref this.k2, keysInput);
            this.TenBitsInput = keysInput;
        }


        public byte SDES_Cipher(byte input) => SDESAlgorithm(input, this.k1, this.k2);

        public byte SDES_DeCipher(byte input) => SDESAlgorithm(input, this.k2, this.k1);

        /// <summary>
        /// Función pemutacion 10. 
        /// </summary>
        /// <param name="input">Cadena de 0s y 1s de 10 posiciones</param>
        /// <returns>Nueva cadena de 10 pos reordenada.</returns>
        private string P10(string input)
        => (input[7].ToString() + input[4].ToString() + input[2].ToString() + input[5].ToString() + input[3].ToString() +
            input[6].ToString() + input[8].ToString() + input[9].ToString() + input[0].ToString() + input[1].ToString());

        /// <summary>
        /// Función pemutacion 8.
        /// </summary>
        /// <param name="input">Cadena de 0s y 1s de 10 posiciones</param>
        /// <returns>Nueva cadena de 8 pos reordenada.</returns>
        private string P8(string input)
            => (input[4].ToString() + input[7].ToString() + input[8].ToString() + input[5].ToString() +
                input[1].ToString() + input[3].ToString() + input[0].ToString() + input[2].ToString());

        /// <summary>
        /// Función pemutacion 4.
        /// </summary>
        /// <param name="input">Cadena de 0s y 1s de 4 posiciones</param>
        /// <returns>Nueva cadena de 4 pos reordenada.</returns>
        private string P4(string input)
            => (input[2].ToString() + input[1].ToString() + input[3].ToString() + input[0].ToString());

        /// <summary>
        /// Función Expandir y Permutar.
        /// </summary>
        /// <param name="input">Cadena de 0s y 1s de 4 posiciones</param>
        /// <returns>Nueva cadena de 8 pos reordenada.</returns>
        private string EP(string input)
            => (input[2].ToString() + input[0].ToString() + input[1].ToString() + input[3].ToString() +
                input[1].ToString() + input[2].ToString() + input[3].ToString() + input[0].ToString());

        /// <summary>
        /// Función Pemutacion Inicial.
        /// </summary>
        /// <param name="input">Cadena de 0s y 1s de 8 posiciones</param>
        /// <returns>Nueva cadena de 8 pos reordenada.</returns>
        private string IP(string input)
            => (input[6].ToString() + input[4].ToString() + input[7].ToString() + input[0].ToString() +
                input[2].ToString() + input[1].ToString() + input[3].ToString() + input[5].ToString());

        /// <summary>
        /// Función Pemutacion Inicial Inversa.
        /// </summary>
        /// <param name="input">Cadena de 0s y 1s de 8 posiciones</param>
        /// <returns>Nueva cadena de 8 pos reordenada.</returns>
        private string IPInversa(string input)
            => (input[3].ToString() + input[5].ToString() + input[4].ToString() + input[6].ToString() +
                input[1].ToString() + input[7].ToString() + input[0].ToString() + input[2].ToString());

        private string LeftShift(string input)
            => (input[1].ToString() + input[2].ToString() + input[3].ToString() + input[4].ToString() + input[0].ToString());

        private string LeftShift2(string input)
           => (input[2].ToString() + input[3].ToString() + input[4].ToString() + input[0].ToString() + input[1].ToString());


        private string Swap(string input)
            => (input[4].ToString() + input[5].ToString() + input[6].ToString() + input[7].ToString() +
                input[0].ToString() + input[1].ToString() + input[2].ToString() + input[3].ToString());


        private void keyGenerator(ref string k1, ref string k2, string input)
        {

            string per10 = P10(input);
            string LS1A = LeftShift(per10.Substring(0, 5));
            string Ls1B = LeftShift(per10.Substring(5, 5));
            k1 = P8(LS1A + Ls1B);
            string Ls2A = LeftShift2(LS1A);
            string Ls2B = LeftShift2(Ls1B);
            k2 = P8(Ls2A + Ls2B);
        }

        private string XOR(string input1, string input2)
        {
            string xorResult = "";
            for (int i = 0; i < input1.Length; i++)
                xorResult += (input1[i] == input2[i]) ? "0" : "1";

            return xorResult;
        }

        /// <summary>
        /// Este metodo recibe una cadena binaria y la convierte a un entero para poder convertir a ASCII
        /// </summary>
        /// <returns>Entero equivalente al binario recibido</returns>
        private byte CadenaBinAByte(string cadenaBinaria)
        {
            byte resultado = 0;

            byte[] baseDecimal = { 128, 64, 32, 16, 8, 4, 2, 1 };

            for (int i = 0; i < 8; i++)
                if (cadenaBinaria[i] == '1') resultado += baseDecimal[i];

            return resultado;
        }

        /// <summary>
        /// Metodo que devuelve el valor de la SwapBox
        /// Segun la cadena binaria de dos caracteres
        /// que se mande como input
        /// </summary>
        /// <param name="input"> cadena binaria de dos pos</param>
        /// <param name="swapBoxNum"> true para swapBox1 false para swapBox0</param>
        /// <returns></returns>
        private string SwapBoxValue(string input, bool swapBoxNum)
        {
            string extremos = (input[0].ToString() + input[3].ToString());
            string internos = (input[1].ToString() + input[2].ToString());

            int fila = PosSwap(extremos);
            int columna = PosSwap(internos);

            return (swapBoxNum) ? this.SwapBox1[fila, columna] : this.SwapBox0[fila, columna];


        }

        /// <summary>
        /// Recibe una cadena de 0s y 1s de dos pociciones
        /// para convertirla a entero. 
        /// </summary>
        /// <param name="cadenaBinaria"> numero de 0 a 3 en binario</param>
        /// <returns> valor decimal entre 0 y 3</returns>
        private int PosSwap(string cadenaBinaria)
        {
            int resultado = 0;

            int[] baseDecimal = { 2, 1 };

            for (int i = 0; i < 2; i++)
                if (cadenaBinaria[i] == '1') resultado += baseDecimal[i];

            return resultado;
        }

        private byte SDESAlgorithm(byte input, string firstKey, string secondKey)
        {
            string cadenaBinaria = Convert.ToString(input, 2).PadLeft(8, '0');
            string Ip = IP(cadenaBinaria);
            string A = Ip.Substring(0, 4);
            string C = Ip.Substring(4, 4);

            string Ep = EP(C);


            string XorK1 = XOR(Ep, firstKey);
            string paraS0 = XorK1.Substring(0, 4);
            string paraS1 = XorK1.Substring(4, 4);

            string S0 = SwapBoxValue(paraS0, false);
            string S1 = SwapBoxValue(paraS1, true);

            string per4 = P4(S0 + S1);

            string XorA = XOR(per4, A);
            string paraSwap = XorA + C;

            string swap = Swap(paraSwap);

            string E = swap.Substring(0, 4);
            string F = swap.Substring(4, 4);

            string EPF = EP(F);

            string XorK2 = XOR(EPF, secondKey);


            paraS0 = XorK2.Substring(0, 4);
            paraS1 = XorK2.Substring(4, 4);

            S0 = SwapBoxValue(paraS0, false);
            S1 = SwapBoxValue(paraS1, true);

            per4 = P4(S0 + S1);

            string XorE = XOR(per4, E);

            string paraIPInversa = XorE + F;

            string IPInv = IPInversa(paraIPInversa);


            return CadenaBinAByte(IPInv);

        }
    }
}
