using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ISFDyT93.Datos.Core;

namespace ISFDyT93.Datos.Daos
{
    public class InscripcionAlumnoDao : DaoBase
    {
        public DataTable ObtenerMateriasVigentes(int alumnoId, string anio)
        {
            string filtroAnio = "";
            if (anio != "") filtroAnio = $"AND Anio= '{anio}'";
            string query = "SELECT AnioLectivo AS [Ciclo Lectivo], Materia, Anio AS Año, Carrera, MateriaId, CursoId FROM MateriasCarrerasVigentes " +
                "WHERE MateriaId NOT IN (SELECT MateriaId FROM AlumnoMateriaCursoAnioCarrera " +
                "WHERE Estado != 'DE' " +
                $"AND AlumnoId= {alumnoId}) {filtroAnio} " +
                $"AND CarreraId IN (SELECT CarreraId FROM AlumnosCarreras WHERE AlumnoId= {alumnoId})";

            return this.Conexion.ObtenerRegistros(query);
        }

        public DataTable ObtenerAniosVigentes(int alumnoId)
        {
            string query = "SELECT CAST(Anio AS VARCHAR) AS Anio FROM MateriasCarrerasVigentes " +
                "WHERE MateriaId NOT IN (SELECT MateriaId FROM AlumnoMateriaCursoAnioCarrera " +
                "WHERE Estado != 'DE' " +
                $"AND AlumnoId= {alumnoId}) " +
                $"AND CarreraId IN (SELECT CarreraId FROM AlumnosCarreras WHERE AlumnoId= {alumnoId}) " +
                $"GROUP BY Anio";

            return this.Conexion.ObtenerRegistros(query);
        }

        public DataRow obtenerFechaIncripcion()
        {
            string query = "SELECT TOP 1 FechaInscripcionInicio, FechaInscripcionFinal FROM CicloLectivo ORDER BY FechaInscripcionInicio";
            return this.Conexion.ObtenerRegistro(query);
        }

        public int actualizarEstadoCursada(Modelos.InscripcionMateriasModelo Modelo)
        {
            string query = "UPDATE CursadaAlumnoCarreras SET Estado = '" + Modelo.estado + "', Cursada = '" + Modelo.cursada + "' WHERE CursadaAlumnoCarreraId =" + Modelo.cursadaAlumnoId;
            return Conexion.EjecutarAccion(query);
        }

        public int GuardarMateriaAsignada(int alumnoId, int materiaId, int cursoId)
        {
            string queryAlumnoCarrera = @"
        SELECT TOP 1 AC.AlumnoCarreraId
        FROM AlumnosCarreras AC
        INNER JOIN Cursos CU ON CU.CursoId = " + cursoId + @"
        INNER JOIN AniosCarreras AN ON AN.AnioCarreraId = CU.AnioCarreraId
        WHERE AC.AlumnoId = " + alumnoId + @"
          AND AC.CarreraId = AN.CarreraId
        ORDER BY AC.AlumnoCarreraId DESC";

            DataRow drAlumnoCarrera = this.Conexion.ObtenerRegistro(queryAlumnoCarrera);
            if (drAlumnoCarrera == null)
                return 0;

            int alumnoCarreraId = Convert.ToInt32(drAlumnoCarrera["AlumnoCarreraId"]);

            string queryCursada = @"
        SELECT TOP 1 C.CursadaId, C.AnioLectivo
        FROM Cursadas C
        INNER JOIN CursoMaterias CM ON CM.CursoMateriaId = C.CursoMateriaId
        WHERE CM.MateriaId = " + materiaId + @"
          AND CM.CursoId = " + cursoId + @"
        ORDER BY C.AnioLectivo DESC, C.CursadaId DESC";

            DataRow drCursada = this.Conexion.ObtenerRegistro(queryCursada);
            if (drCursada == null)
                return 0;

            int cursadaId = Convert.ToInt32(drCursada["CursadaId"]);
            int anioLectivo = Convert.ToInt32(drCursada["AnioLectivo"]);

            string queryExiste = @"
        SELECT TOP 1 CursadaAlumnoCarreraId
        FROM CursadaAlumnoCarreras
        WHERE AlumnoCarreraId = " + alumnoCarreraId + @"
          AND CursadaId = " + cursadaId;

            DataRow drExiste = this.Conexion.ObtenerRegistro(queryExiste);
            if (drExiste != null)
                return 1;

            string queryInsert = @"
        INSERT INTO CursadaAlumnoCarreras
        (
            AlumnoCarreraId,
            CursadaId,
            AnioCicloLectivo,
            Estado,
            HorasCursadas,
            UltimoPresentismo,
            PorcentajeAsistencia,
            Cursada,
            Activo
        )
        VALUES
        (
            " + alumnoCarreraId + @",
            " + cursadaId + @",
            " + anioLectivo + @",
            NULL,
            0,
            NULL,
            0,
            NULL,
            1
        )";

            return Conexion.EjecutarAccion(queryInsert);
        }
    }
}