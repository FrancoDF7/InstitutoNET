using ISFDyT93.Datos.Modelos;
using ISFDyT93.Negocio.Logica;
using Syncfusion.XlsIO;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ISFDyT93.Negocio;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;

namespace ISFDyT93.Vista.Forms.Componentes
{
    public partial class FormCargaMasivaExcel : Form
    {
        CarrerasLogica carrerasLogica = new CarrerasLogica();

        public FormPrincipal Contenedor { get; set; }
        public string Accion { get; set; }
        public int AlumnoId { get; set; }
        public int AlumnoCarreraId { get; set; }

        DataTable dtExcel;
        HashSet<int> _columnasNoMapeadas = new HashSet<int>();
        List<string> _propiedades;

        public FormCargaMasivaExcel()
        {
            InitializeComponent();
            dgvCargaMasiva.CellPainting += PintarHeaderNoMapeado;
            dgvCargaMasiva.ColumnHeaderMouseClick += MostrarMenuMapeo;
        }

        private void btnBuscarArchivoExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog archivoExcel = new OpenFileDialog();
            archivoExcel.Filter = "Archivos Excel|*.xls;*.xlsx|Archivos .csv (*.csv)|*.csv";
            archivoExcel.InitialDirectory = "C://";

            if (archivoExcel.ShowDialog() == DialogResult.OK)
            {
                string rutaCvs = archivoExcel.FileName;
                using (Stream inputStream = File.OpenRead(rutaCvs))
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IWorksheet worksheet = excelEngine.Excel.Workbooks.Open(inputStream).Worksheets[0];
                    dtExcel = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                }
                dgvCargaMasiva.DataSource = dtExcel;
                ProcesarHeaders();
            }
        }

        public void ProcesarHeaders()
        {
            _columnasNoMapeadas.Clear();
            _propiedades = typeof(AlumnosModelo).GetProperties().Select(p => p.Name).ToList();

            foreach (DataColumn column in dtExcel.Columns.Cast<DataColumn>().ToList())
            {
                bool matched = false;
                foreach (var prop in typeof(AlumnosModelo).GetProperties())
                {
                    if (BuscarCoincidencia(prop.Name, column.ColumnName))
                    {
                        if (!dtExcel.Columns.Contains(prop.Name))
                            column.ColumnName = prop.Name;
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    var dgvCol = dgvCargaMasiva.Columns[column.ColumnName];
                    if (dgvCol != null)
                        _columnasNoMapeadas.Add(dgvCol.Index);
                }
            }

            dgvCargaMasiva.Invalidate();
        }

        private void PintarHeaderNoMapeado(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex != -1 || !_columnasNoMapeadas.Contains(e.ColumnIndex)) return;

            e.PaintBackground(e.CellBounds, false);
            using (var brush = new SolidBrush(Color.Crimson))
                e.Graphics.FillRectangle(brush, e.CellBounds);

            string texto = (e.Value?.ToString() ?? "") + "  ▼";
            var formato = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using (var brush = new SolidBrush(Color.White))
                e.Graphics.DrawString(texto, e.CellStyle.Font, brush, e.CellBounds, formato);
            e.Handled = true;
        }

        private void MostrarMenuMapeo(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (_propiedades == null) return;

            var menu = new ContextMenuStrip();
            int colIndex = e.ColumnIndex;

            foreach (var prop in _propiedades)
            {
                var item = new ToolStripMenuItem(prop);
                item.Click += (s, args) => AplicarMapeo(colIndex, prop);
                menu.Items.Add(item);
            }

            Rectangle rect = dgvCargaMasiva.GetCellDisplayRectangle(colIndex, -1, true);
            menu.Show(dgvCargaMasiva.PointToScreen(new Point(rect.Left, rect.Bottom)));
        }

        private void AplicarMapeo(int colIndex, string propiedadSeleccionada)
        {
            var dgvCol = dgvCargaMasiva.Columns[colIndex];
            if (dgvCol == null) return;

            string nombreActual = dgvCol.HeaderText;
            if (nombreActual == propiedadSeleccionada)
            {
                // Mismo nombre, solo limpiar el estado rojo si estaba sin mapear
                _columnasNoMapeadas.Remove(colIndex);
                dgvCargaMasiva.InvalidateCell(colIndex, -1);
                return;
            }

            if (dtExcel.Columns.Contains(propiedadSeleccionada))
            {
                MessageBox.Show($"Ya hay una columna asignada como '{propiedadSeleccionada}'.", "Mapeo duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtExcel.Columns.Contains(nombreActual))
                dtExcel.Columns[nombreActual].ColumnName = propiedadSeleccionada;

            dgvCol.HeaderText = propiedadSeleccionada;
            _columnasNoMapeadas.Remove(colIndex);
            dgvCargaMasiva.InvalidateCell(colIndex, -1);
        }

        public bool BuscarCoincidencia(string nombrePropiedad, string nombreExcel)
        {
            XDocument doc = XDocument.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "CargaMasivaMap.xml"));
            var dic = doc.Root.Elements()
                .ToDictionary(
                    n => Validaciones.CrearSlug(n.Name.LocalName),
                    n => n.Elements().Select(x => x.Value).ToList()
                );

            nombrePropiedad = Validaciones.CrearSlug(nombrePropiedad);
            nombreExcel = Validaciones.CrearSlug(nombreExcel);

            return dic.ContainsKey(nombrePropiedad) && dic[nombrePropiedad].Any(c => nombreExcel == Validaciones.CrearSlug(c));
        }

        private void btnAceptarCargaMasiva_Click(object sender, EventArgs e)
        {
            if (this.dtExcel != null)
            {


            }
        }
    }
}
