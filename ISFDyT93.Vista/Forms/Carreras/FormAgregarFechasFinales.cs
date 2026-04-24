using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ISFDyT93.Vista.Core;
using ISFDyT93.Vista.Core.Enums;
using ISFDyT93.Negocio.Logica;
using ISFDyT93.Negocio.Core.Enums;
using ISFDyT93.Datos.Daos;
using ISFDyT93.Negocio.Logica.LogicaForms;

namespace ISFDyT93.Vista.Forms.Carreras
{
    public partial class FormAgregarFechasFinales : FormBase
    {
        #region Publics
        public int CarreraId { get; set; }
        public string NombreCarrera { get; set; }
        public int MesaFinalId { get; set; }
        public DateTime Fecha { get; set; }
        public TipoAccion Accion { get; set; }

        public int AnioLectivoId { get; set; }
        public int TurnoId { get; set; }
        public int LlamadoId { get; set; }
        #endregion

        #region Privates
        private MateriasLogica materiasLogica;
        private MesasFinalesLogica mesasFinalesLogica;
        private FormAgregarFechaFinales_Logica frmLogica;
        private DateTime fecha;
        private string title;
        #endregion

        public FormAgregarFechasFinales()
        {
            InitializeComponent();
            materiasLogica = new MateriasLogica();
            mesasFinalesLogica = new MesasFinalesLogica();
            frmLogica = new FormAgregarFechaFinales_Logica(mesasFinalesLogica);
        }

        private void FormAgregarMesas_Load(object sender, EventArgs e)
        {
            if (this.Accion == TipoAccion.Agregar)
            {
                CargarMaterias();
                CargarTurnoMateria(false);
                cmbMateria.Enabled = true;
                title = "Agregar fecha especial";
            }

            if (this.Accion == TipoAccion.Modificar)
            {
                CargarTurnoMateria(false);
                CargarProfesorTitular();
                CargarVocales(Convert.ToInt32(cmbPresidenteMesa.SelectedValue));
                title = "Asignar fecha y vocal";
            }

            Contenedor.SetTitulo(title).SetVolver(() =>
            {
                Contenedor.AbrirFormulario<FormMesasFinales>(form =>
                {
                    form.CarreraId = this.CarreraId;
                    form.NombreCarrera = this.NombreCarrera;
                    form.AnioLectivoId = this.AnioLectivoId;
                    form.TurnoId = this.TurnoId;
                    form.LlamadoId = this.LlamadoId;
                    if (this.LlamadoId == 3)
                        form.FechaUnica = true;
                });
            });
        }

        private void CargarMaterias()
        {
            frmLogica.CargaCombo(cmbMateria, 
                materiasLogica.MateriasId(CarreraId),
                "MateriaId",
                "Nombre");
        }

        private void CargarProfesorTitular()
        {
            frmLogica.CargaCombo(cmbPresidenteMesa,
                mesasFinalesLogica.ObtenerProfesorTitular(Convert.ToInt32(cmbMateria.SelectedValue)),
                "PersonalId",
                "Nombre");
        }

        private void CargarVocales(int PersonalId)
        {
            frmLogica.CargaCombo(cmbVocalMesa,
                mesasFinalesLogica.ObtenerVocales(CarreraId, PersonalId),
                "PersonalId",
                "Nombre");
        }

        
        private void dtpFechaMesa_ValueChanged(object sender, EventArgs e)
        {
            frmLogica.ValidarFechaMesa(dtpFechaMesa,ref fecha, ValidarCampos);
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var logica = new FormAgregarFechaFinales_Logica(mesasFinalesLogica);

            bool res = logica.ProcesarAccion(
                Accion,
                fecha,
                CarreraId,
                AnioLectivoId,
                Convert.ToInt32(cmbTurno.SelectedValue),                
                this.MesaFinalId,
                Convert.ToInt32(cmbMateria.SelectedValue),
                Convert.ToInt32(cmbPresidenteMesa.SelectedValue),
                Convert.ToInt32(cmbVocalMesa.SelectedValue));

            if (res)
            {
                Notificar(TipoNotificacion.Success,
                    Accion == TipoAccion.Modificar ? "Mesa modificada correctamente" : "Mesa agregada correctamente");

                Contenedor.AbrirFormulario<FormMesasFinales>(form =>
                {
                    form.CarreraId = CarreraId;
                    form.NombreCarrera = NombreCarrera;
                    form.AnioLectivoId = AnioLectivoId;
                    form.TurnoId = TurnoId;
                    form.LlamadoId = LlamadoId;
                    if (LlamadoId == 3)
                        form.FechaUnica = true;
                });
            }
            else            
                Notificar(TipoNotificacion.Error, "Ocurrió un error");            

        }

        private void ValidarCampos()
        {
            frmLogica.ValidarCampos(btnAgregar, 
                cmbTurno, 
                cmbMateria, 
                cmbPresidenteMesa, 
                cmbVocalMesa);
        }

        private void CargarTurnoMateria(bool especial)
        {
            frmLogica.CargarTurnoMateria(Accion, cmbTurno, cmbMateria, mesasFinalesLogica, MesaFinalId, true);
        }

        private void cmbPresidenteMesa_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ValidarCampos();            
        }

        private void cmbVocalMesa_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ValidarCampos();
        }

        private void cmbMateria_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (this.Accion == TipoAccion.Agregar)
            {
                CargarProfesorTitular();
                CargarVocales(Convert.ToInt32(cmbPresidenteMesa.SelectedValue));
                ValidarCampos();
            }
        }
    }
}
