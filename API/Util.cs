using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Estructuras;

namespace API
{
    static class Util
    {
        static string EncriptarMensaje(string mensaje, string tenBitsInput)
        {
            string retorno = "";
            Sdes cipher = new Sdes(tenBitsInput);

            foreach (var item in mensaje)
            {
                byte input = Convert.ToByte(item);
                byte cifrado = cipher.SDES_Cipher(input);

                retorno += Convert.ToChar(cifrado);
            }

            return retorno;

        }

        static string DesEncriptarMensaje(string mensaje, string tenBitsInput)
        {
            string retorno = "";
            Sdes cipher = new Sdes(tenBitsInput);

            foreach (var item in mensaje)
            {
                byte input = Convert.ToByte(item);
                byte desCifrado = cipher.SDES_DeCipher(input);

                retorno += Convert.ToChar(desCifrado);
            }

            return retorno;

        }

        static string ByteALlaveSdes(byte llaveDiffieHelman) 
            => Convert.ToString(llaveDiffieHelman, 2).PadLeft(10, '1');


    }
}
