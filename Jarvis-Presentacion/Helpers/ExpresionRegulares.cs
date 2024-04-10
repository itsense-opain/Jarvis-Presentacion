using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Helpers
{
    public static class ExpresionesRegulares
    {
        public const string NumerosLetras = @"^[a-zA-Z0-9]+$";
        public const string Nombre = @"^([a-zA-Z áÁ éÉ íÍ óÓ úÚ ñÑ,.'-]+)$";
        public const string NombreNumeros = @"^([0-9a-zA-Z áÁ éÉ íÍ óÓ úÚ ñÑ,.'-]+)$";
        public const string NombreNumerosSimbolos = @"^([0-9a-zA-Z áÁ éÉ íÍ óÓ úÚ ñÑ,_.'-]+)$";
        public const string NombreNumerosMayusculasGuion = @"^([0-9A-Z -]+)$";
        public const string NombreNumerosMayusculasSimbolos = @"^([0-9A-Z -\(\)/]+)$";
        public const string NombreNumerosExtendido = @"^([0-9a-zA-Z áÁ éÉ íÍ óÓ úÚ ñÑ Çç Ää Ëë Ïï Öö Üü Àà Èè Ìì Òò Ùù '-]+)$";
        public const string Direccion = @"^([0-9a-zA-Z áÁ éÉ íÍ óÓ úÚ ñÑ,.'-]+)$";
        public const string Digitos = @"^([0-9]*)$";
        public const string Mail = @"^[_a-zA-Z0-9-]+(.[_a-zA-Z0-9-]+)*@[a-zA-Z0-9-]+(.[a-zA-Z0-9-]+)*(.[a-zA-Z]{2,4})$";
        public const string Ip = @"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$";
        //Valida fecha en formato dd-MM-yyyy, valida fechas de años bisiestos.
        public const string Fecha = @"(^(((0[1-9]|1[0-9]|2[0-8])[\-](0[1-9]|1[012]))|((29|30|31)[\-](0[13578]|1[02]))|((29|30)[\-](0[4,6,9]|11)))[\-](19|[2-9][0-9])\d\d$)|(^29[\-]02[\-](19|[2-9][0-9])(00|04|08|12|16|20|24|28|32|36|40|44|48|52|56|60|64|68|72|76|80|84|88|92|96)$)";
        public const string Decimales = @"^[0-9]+([,][0-9]+)?$";

        public readonly static System.Text.RegularExpressions.Regex RegexNumerosLetras = new System.Text.RegularExpressions.Regex(NumerosLetras);
        public readonly static System.Text.RegularExpressions.Regex RegexNombre = new System.Text.RegularExpressions.Regex(Nombre);
        public readonly static System.Text.RegularExpressions.Regex RegexNombreNumerosGuion = new System.Text.RegularExpressions.Regex(NombreNumerosMayusculasGuion);
        public readonly static System.Text.RegularExpressions.Regex RegexNombreMayusculasSimbolos = new System.Text.RegularExpressions.Regex(NombreNumerosMayusculasSimbolos);
        public readonly static System.Text.RegularExpressions.Regex RegexNombreNumerosSimbolos = new System.Text.RegularExpressions.Regex(NombreNumerosSimbolos);
        public readonly static System.Text.RegularExpressions.Regex RegexDireccion = new System.Text.RegularExpressions.Regex(Direccion);
        public readonly static System.Text.RegularExpressions.Regex RegexNombreNumerosExtendido = new System.Text.RegularExpressions.Regex(NombreNumerosExtendido);
        public readonly static System.Text.RegularExpressions.Regex RegexDigitos = new System.Text.RegularExpressions.Regex(Digitos);
        public readonly static System.Text.RegularExpressions.Regex RegexDecimales = new System.Text.RegularExpressions.Regex(Decimales);
        public readonly static System.Text.RegularExpressions.Regex RegexMail = new System.Text.RegularExpressions.Regex(Mail);
        public readonly static System.Text.RegularExpressions.Regex RegexFecha = new System.Text.RegularExpressions.Regex(Fecha);
        public readonly static System.Text.RegularExpressions.Regex RegexIp = new System.Text.RegularExpressions.Regex(Ip);
    }
}
