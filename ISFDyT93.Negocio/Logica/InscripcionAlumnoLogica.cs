using ISFDyT93.Datos.Daos;
using ISFDyT93.Datos.Modelos;
using ISFDyT93.Negocio.Core;
using System;
using System.Collections.Generic;
using System.Data;

namespace ISFDyT93.Negocio.Logica
{
    public class InscripcionAlumnoLogica : LogicaBase
    {
        InscripcionAlumnoDao inscripcionAlumnoDao = new InscripcionAlumnoDao();

        public DataTable ObtenerMateriasVigentes(int alumnoId, string anio)
        {
            return inscripcionAlumnoDao.ObtenerMateriasVigentes(alumnoId, anio); ;
        }

        public DataTable ObtenerAniosVigentes(int alumnoId)
        {
            return inscripcionAlumnoDao.ObtenerAniosVigentes(alumnoId);
        }

        public bool obtenerFechaIncripcion()
        {
            bool OK;
            DataRow dr = inscripcionAlumnoDao.obtenerFechaIncripcion();
            OK = (DateTime.Today > Convert.ToDateTime(dr["FechaInscripcionInicio"]) && DateTime.Today <= Convert.ToDateTime(dr["FechaInscripcionFinal"]));
            return OK;
        }
        public int actualizarEstadoCursada(InscripcionMateriasModelo Modelo)
        {
            return inscripcionAlumnoDao.actualizarEstadoCursada(Modelo);
        }

        public bool ActualizarCursadas(List<InscripcionMateriasModelo> lista)
        {
            int count = 0;
            int countOk = 0;

            foreach (var item in lista)
            {
                if (!string.IsNullOrEmpty(item.estado) || !string.IsNullOrEmpty(item.cursada))
                {
                    count++;
                    int result = inscripcionAlumnoDao.actualizarEstadoCursada(item);
                    if (result == 1)
                        countOk++;
                }
            }

            return count == countOk;
        }

        private List<string> materiasAsignadas = new List<string>();

        public void AsignarMateria(string materiaId, string cursoId)
        {

            string key = materiaId + cursoId;

            if (!materiasAsignadas.Contains(key))
            {
                materiasAsignadas.Add(key);
            }
        }

        public class MateriaAsignadaModelo
        {
            public int MateriaId { get; set; }
            public int CursoId { get; set; }
        }

        public bool GuardarMateriasAsignadas(int alumnoId, List<MateriaAsignadaModelo> materiasAsignadas)
        {
            if (materiasAsignadas == null || materiasAsignadas.Count == 0)
                return false;

            int total = 0;
            int ok = 0;

            foreach (var item in materiasAsignadas)
            {
                total++;

                int resultado = inscripcionAlumnoDao.GuardarMateriaAsignada(
                    alumnoId,
                    item.MateriaId,
                    item.CursoId
                );

                if (resultado > 0)
                    ok++;
            }

            return total == ok;
        }
    }
}