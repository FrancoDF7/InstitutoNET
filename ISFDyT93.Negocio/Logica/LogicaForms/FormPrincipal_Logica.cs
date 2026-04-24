using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ISFDyT93.Negocio.Logica
{
    public class FormPrincipal_Logica
    {
        private MesasFinalesLogica mesasLogica;
        
        public void ComprobarMesasFinales()
        {
            DataTable dt = mesasLogica.ExistenMesasFinales(2024);
            int marzo = Convert.ToInt32(dt.Rows[0]["Marzo"]);
            int julio = Convert.ToInt32(dt.Rows[0]["Julio"]);
            int diciembre = Convert.ToInt32(dt.Rows[0]["Diciembre"]);
            MessageBox.Show($"{marzo}, {julio}, {diciembre}");
        }

    }
}
