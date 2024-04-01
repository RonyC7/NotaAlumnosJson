using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace NotaAlumnosJson
{
    public partial class Form1 : Form
    {
        private List<Alumno> listaAlumnos;
        private string rutaArchivo = "DAlumnos.json";

        public Form1()
        {
            InitializeComponent();
            listaAlumnos = new List<Alumno>();

            if (File.Exists(rutaArchivo))
            {
                CargarDatosDesdeJson();
            }
        }

        private void buttonGuardar_Click(object sender, EventArgs e)
        {
            string nombreAlumno = textBoxNombre.Text.Trim();
            string cursoSeleccionado = comboBoxCursos.SelectedItem?.ToString();
            int notaCurso;

            if (string.IsNullOrEmpty(nombreAlumno))
            {
                MessageBox.Show("Por favor, ingrese el nombre del alumno.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(cursoSeleccionado))
            {
                MessageBox.Show("Por favor, seleccione un curso.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBoxNotas.Text, out notaCurso) || notaCurso < 0 || notaCurso > 100)
            {
                MessageBox.Show("Por favor, ingrese una nota válida (entre 0 y 100).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Alumno alumnoExistente = listaAlumnos.Find(alumno => alumno.Nombre == nombreAlumno);
            if (alumnoExistente != null)
            {
                if (alumnoExistente.Cursos.Exists(curso => curso.Nombre == cursoSeleccionado))
                {
                    MessageBox.Show("El alumno ya tiene una nota asignada para este curso.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                alumnoExistente.Cursos.Add(new Curso { Nombre = cursoSeleccionado, Nota = notaCurso });
            }
            else
            {
                Alumno nuevoAlumno = new Alumno { Nombre = nombreAlumno, Cursos = new List<Curso> { new Curso { Nombre = cursoSeleccionado, Nota = notaCurso } } };
                listaAlumnos.Add(nuevoAlumno);
            }

            GuardarDatosEnJson();
            LimpiarCampos();
        }

        private void CargarDatosDesdeJson()
        {
            try
            {
                string json = File.ReadAllText(rutaArchivo);
                listaAlumnos = JsonConvert.DeserializeObject<List<Alumno>>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos desde el archivo JSON: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GuardarDatosEnJson()
        {
            try
            {
                string json = JsonConvert.SerializeObject(listaAlumnos);
                File.WriteAllText(rutaArchivo, json);
                MessageBox.Show("Datos guardados correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los datos en el archivo JSON: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            textBoxNombre.Clear();
            comboBoxCursos.SelectedIndex = -1;
            textBoxNotas.Clear();
        }

        private void buttonMostrar_Click(object sender, EventArgs e)
        {
            if (dataGridViewDatos.IsHandleCreated)
            {
                dataGridViewDatos.Rows.Clear();

                foreach (var alumno in listaAlumnos)
                {
                    DataGridViewRow row = new DataGridViewRow();

                    row.Cells.Add(new DataGridViewTextBoxCell { Value = alumno.Nombre });

                    foreach (var curso in ObtenerNombresCursosPredeterminados())
                    {
                        row.Cells.Add(new DataGridViewTextBoxCell { Value = ObtenerNotaCurso(alumno, curso) });
                    }

                    dataGridViewDatos.Rows.Add(row);
                }
            }
            else
            {
                MessageBox.Show("El control DataGridView aún no ha sido creado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> ObtenerNombresCursosPredeterminados()
        {
            return new List<string>
    {
        "Matematicas",
        "Fisica",
        "Programacion",
        "Algebra",
        "Quimica",
        "Estructura de datos",
        "Compiladores",
        "Automatas y lenguajes"
    };
        }

        private string ObtenerNotaCurso(Alumno alumno, string nombreCurso)
        {
            var curso = alumno.Cursos.FirstOrDefault(c => c.Nombre == nombreCurso);
            return curso != null ? curso.Nota.ToString() : "";
        }

        private void buttonEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridViewDatos.SelectedRows.Count > 0)
            {
                string nombreAlumno = dataGridViewDatos.SelectedRows[0].Cells[0].Value.ToString();
                DialogResult result = MessageBox.Show("¿Está seguro de eliminar al alumno '" + nombreAlumno + "'?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    EliminarAlumno(nombreAlumno);
                    ActualizarDataGridView();
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un alumno para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarAlumno(string nombreAlumno)
        {
            listaAlumnos.RemoveAll(alumno => alumno.Nombre == nombreAlumno);
        }

        private void ActualizarDataGridView()
        {
            dataGridViewDatos.Rows.Clear();
            foreach (var alumno in listaAlumnos)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.Add(new DataGridViewTextBoxCell { Value = alumno.Nombre });
                foreach (var curso in ObtenerNombresCursosPredeterminados())
                {
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = ObtenerNotaCurso(alumno, curso) });
                }
                dataGridViewDatos.Rows.Add(row);
            }
        }

        private void dataGridViewDatos_SelectionChanged(object sender, EventArgs e)
        {
            dataGridViewDatos.ClearSelection();
        }
        

    }

    public class Curso
    {
        public string Nombre { get; set; }
        public int Nota { get; set; }
    }

    public class Alumno
    {
        public string Nombre { get; set; }
        public List<Curso> Cursos { get; set; }
    }
}
