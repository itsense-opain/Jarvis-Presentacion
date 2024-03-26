using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    /// <summary>
    /// Clase maestra de objetos 
    /// </summary>
    public abstract class Base
    {
        protected Decimal IIFValidDecimal(string Valor)
        {
            Decimal Subtotal = 0;
            Decimal TryValor = 0;
            bool IsValid = false;
            try
            {
                IsValid = Decimal.TryParse(Valor, out TryValor);
                if (IsValid)
                    Subtotal = TryValor;
                return Subtotal;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return Subtotal;
            }
        }
    }
}
