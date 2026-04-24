using ISFDyT93.Negocio.Core.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ISFDyT93.Negocio.Logica.LogicaForms
{
    public class FormAgregarFechaFinales_Logica
    {
        private MesasFinalesLogica mesasFinalesLogica;
        public FormAgregarFechaFinales_Logica(MesasFinalesLogica logica)
        {
            this.mesasFinalesLogica = logica;
        }


        public void CargaCombo(ComboBox combo, DataTable data, string valueMember, string displayMember)
        {
            combo.DataSource = data;
            combo.ValueMember = valueMember;
            combo.DisplayMember = displayMember;
        }

        public void ValidarCampos(Button btn, params ComboBox[] combos)
        {
            btn.Enabled = combos.All(c => !string.IsNullOrWhiteSpace(c.Text));
        }

        public void CargarTurnoMateria(TipoAccion accion, ComboBox cmbTurno, ComboBox cmbMateria, MesasFinalesLogica mesasFinalesLogica, int mesaFinalId, bool especial)
        {
            switch (accion)
            {
                case TipoAccion.Agregar:
                    CargaCombo(cmbTurno, mesasFinalesLogica.ObtenerTurnos(especial), "TurnoId", "Descripcion");
                    break;

                case TipoAccion.Modificar:
                    CargaCombo(cmbTurno, mesasFinalesLogica.ObtenerTurnoMesa(mesaFinalId), "TurnoId", "Descripcion");
                    CargaCombo(cmbMateria, mesasFinalesLogica.ObtenerMateriaFinal(mesaFinalId), "MateriaId", "Nombre");
                    break;
            }
        }
        
        public void ValidarFechaMesa(DateTimePicker dtpFechaMesa, ref DateTime fecha, Action validarCampos)
        {
            dtpFechaMesa.CustomFormat = "dd/MM/yyyy";
            if (dtpFechaMesa.Value.Date > DateTime.Now.Date)
            {
                fecha = dtpFechaMesa.Value.Date;
                validarCampos();
            }
            else
            {
                MessageBox.Show("Seleccione una fecha posterior al día de hoy.", "Fecha errónea", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public bool ProcesarAccion(TipoAccion accion, DateTime fecha, int carreraId, int anioLectivoId, int turnoId, int mesaFinalId, int materiaId, int presidenteId, int vocalId)            
        {
            int res = 0;
            if (accion == TipoAccion.Modificar)
            {
                res = mesasFinalesLogica.ModificarMesa(
                    fecha, 
                    turnoId, 
                    presidenteId, 
                    vocalId, 
                    mesaFinalId);
            }
            else if(accion == TipoAccion.Agregar)
            {
                res = mesasFinalesLogica.AgregarMesa(
                    carreraId, 
                    fecha, 
                    4, 
                    3, 
                    materiaId, 
                    presidenteId, 
                    vocalId, 
                    anioLectivoId);                    
            }

            return res > 0;
        }


    }
}
