using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pruebas
{
    public partial class Form1 : Form
    {
        DataTable unidadesTable = new DataTable();
        SqlDataAdapter dataAdapterUnidades;
        DataTable tieneTable = new DataTable();
        SqlDataAdapter dataAdapterTiene;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'pasteleriaPruebasDataSet1.Unidades' table. You can move, or remove it, as needed.
            this.unidadesTableAdapter.Fill(this.pasteleriaPruebasDataSet1.Unidades);
            // TODO: This line of code loads data into the 'pasteleriaPruebasDataSet1.Ingredientes' table. You can move, or remove it, as needed.
            this.ingredientesTableAdapter.Fill(this.pasteleriaPruebasDataSet1.Ingredientes);
            // TODO: This line of code loads data into the 'pasteleriaPruebasDataSet.Tiene' table. You can move, or remove it, as needed.
            this.tieneTableAdapter.Fill(this.pasteleriaPruebasDataSet.Tiene);
            // TODO: This line of code loads data into the 'pasteleriaPruebasDataSet.Origen' table. You can move, or remove it, as needed.
            this.origenTableAdapter.Fill(this.pasteleriaPruebasDataSet.Origen);
            // TODO: This line of code loads data into the 'pasteleriaPruebasDataSet.Unidades' table. You can move, or remove it, as needed.
            this.unidadesTableAdapter.Fill(this.pasteleriaPruebasDataSet.Unidades);
            // TODO: This line of code loads data into the 'pasteleriaPruebasDataSet.Recetas' table. You can move, or remove it, as needed.
            this.recetasTableAdapter.Fill(this.pasteleriaPruebasDataSet.Recetas);

            textBoxNombreReceta.Text = pasteleriaPruebasDataSet.Recetas.FindByID(1).Nombre;
            comboBoxPaises.SelectedValue = pasteleriaPruebasDataSet.Recetas.FindByID(1).Origen_ID;
            textBoxPorciones.Text = pasteleriaPruebasDataSet.Recetas.FindByID(1).Porciones.ToString();
            comboBoxUnidades.SelectedValue = pasteleriaPruebasDataSet.Recetas.FindByID(1).Unidad_ID;
            textBoxPreparacion.Text = pasteleriaPruebasDataSet.Recetas.FindByID(1).Preparacion;

            DataRow[] recetas = pasteleriaPruebasDataSet.Tiene.Select("Receta_ID = 1");

            int idIngrediente = 1;
            foreach (var receta in recetas)
            {
                AgregarIngredienteAFLowLayout(idIngrediente, (int)receta["Receta_ID"], (int)receta["Ingrediente_ID"], (decimal)receta["Cantidad"], (int)receta["Unidad_ID"]);

                idIngrediente++;
            }

            bloquearControles();
        }

        public void bloquearControles()
        {
            textBoxNombreReceta.BorderStyle = BorderStyle.None;
            textBoxNombreReceta.BackColor = SystemColors.Control;
            textBoxNombreReceta.ReadOnly = true;

            comboBoxPaises.Visible = false;

            label9.Visible = true;
        }

        public void desbloquarControles()
        {

        }

        private void BtnActualizarReceta_Click(object sender, EventArgs e)
        {
            SqlDataAdapter dataAdapterRecetas = new SqlDataAdapter("SELECT * FROM Recetas", recetasTableAdapter.Connection);

            dataAdapterRecetas.UpdateCommand = new SqlCommand("UPDATE Recetas " +
                "SET Nombre = @RecetaNombre" +
                ",Origen_ID = @Origen_ID" +
                ",Porciones = @Porciones" +
                ",Unidad_ID = @Unidad_ID" +
                ",Preparacion = @Preparacion " +
                "WHERE ID = @RecetaID", recetasTableAdapter.Connection);

            dataAdapterRecetas.UpdateCommand.Parameters.Add("@RecetaNombre", SqlDbType.VarChar,50,"Nombre");
            dataAdapterRecetas.UpdateCommand.Parameters.Add("@Origen_ID", SqlDbType.Int, 4, "Origen_ID");
            dataAdapterRecetas.UpdateCommand.Parameters.Add("@Porciones", SqlDbType.Decimal, 5, "Porciones");
            dataAdapterRecetas.UpdateCommand.Parameters.Add("@Unidad_ID", SqlDbType.Int, 4, "Unidad_ID");
            dataAdapterRecetas.UpdateCommand.Parameters.Add("@Preparacion", SqlDbType.VarChar, -1, "Preparacion");

            SqlParameter parameter = dataAdapterRecetas.UpdateCommand.Parameters.Add("@RecetaID", SqlDbType.Int);

            parameter.SourceColumn = "ID";
            parameter.SourceVersion = DataRowVersion.Original;

            DataTable recetaTable = new DataTable();
            dataAdapterRecetas.Fill(recetaTable);

            Console.WriteLine("Valor seleccionado Button Update: " + comboBoxUnidades.SelectedValue);
            Console.WriteLine("Texto Seleccionado Button Update: " + comboBoxUnidades.SelectedText);

            //update (Rows[0]) primera fila
            //update (Rows[1]) segunda fila
            //IndexOfRowToUpdate = ID - 1
            //Rows[IndexOfRowToUpdate]

            DataRow recetaRow = recetaTable.Rows[0];
            recetaRow["Nombre"] = textBoxNombreReceta.Text;
            recetaRow["Origen_ID"] = comboBoxPaises.SelectedValue;
            recetaRow["Porciones"] = textBoxPorciones.Text;
            recetaRow["Unidad_ID"] = comboBoxUnidades.SelectedValue;
            recetaRow["Preparacion"] = textBoxPreparacion.Text;

            dataAdapterRecetas.Update(recetaTable);






            tieneTable.Clear();

            dataAdapterTiene = new SqlDataAdapter("SELECT * FROM Tiene WHERE Receta_ID = 1", tieneTableAdapter.Connection);
            dataAdapterTiene.Fill(tieneTable);

            bool ingredienteExiste = false;

            foreach (FlowLayoutPanel IngredienteControl in flowLayoutPanel1.Controls)
            {
                ComboBox Ingrediente = (ComboBox)IngredienteControl.Controls[0];
                TextBox Cantidad = (TextBox)IngredienteControl.Controls[1];
                ComboBox Unidad = (ComboBox)IngredienteControl.Controls[2];

                Console.WriteLine("Ingrediente en control: " + (int)Ingrediente.SelectedValue);

                //Actualiza filas existentes
                foreach (DataRow recetaTiene in tieneTable.Rows)
                {
                    Console.WriteLine("Ingrediente en tiene: " + (int)recetaTiene["Ingrediente_ID"]);
                    
                    if ((int)Ingrediente.SelectedValue == (int)recetaTiene["Ingrediente_ID"])
                    {
                        recetaTiene["Cantidad"] = decimal.Parse(Cantidad.Text);
                        recetaTiene["Unidad_ID"] = Unidad.SelectedValue;
                        ingredienteExiste = true;
                    }
                }

                Console.WriteLine("Ingrediente existe: " + ingredienteExiste);
                Console.WriteLine("Agregar Ingrediente: " + !ingredienteExiste);

                //si le ingrediente no existe lo agrega
                if (!ingredienteExiste)
                {
                    DataRow nuevoTiene = tieneTable.NewRow();
                    nuevoTiene["Receta_ID"] = 1;
                    nuevoTiene["Ingrediente_ID"] = Ingrediente.SelectedValue;
                    nuevoTiene["Cantidad"] = decimal.Parse(Cantidad.Text);
                    nuevoTiene["Unidad_ID"] = Unidad.SelectedValue;

                    tieneTable.Rows.Add(nuevoTiene);

                    Console.WriteLine("tieneTable Ingrediente Agregado");

                    foreach (DataRow fila in tieneTable.Rows)
                    {
                        Console.WriteLine("Estado de fila: " + fila.RowState);
                        Console.Write(fila["Ingrediente_ID"]);
                        Console.Write(fila["Cantidad"]);
                        Console.WriteLine(fila["Unidad_ID"]);
                    }
                }

                ingredienteExiste = false;
            }



            dataAdapterTiene.InsertCommand = new SqlCommand("INSERT INTO Tiene(Receta_ID, Ingrediente_ID, Cantidad, Unidad_ID) " +
            "VALUES (@Receta_ID, @Ingrediente_ID, @Cantidad, @Unidad_ID)", tieneTableAdapter.Connection);

            dataAdapterTiene.InsertCommand.Parameters.Add("@Receta_ID", SqlDbType.Int);
            dataAdapterTiene.InsertCommand.Parameters.Add("@Ingrediente_ID", SqlDbType.Int);
            dataAdapterTiene.InsertCommand.Parameters.Add("@Cantidad", SqlDbType.Decimal);
            dataAdapterTiene.InsertCommand.Parameters.Add("@Unidad_ID", SqlDbType.Int);

            dataAdapterTiene.UpdateCommand = new SqlCommand("UPDATE Tiene " +
               "SET Cantidad = @Cantidad" +
               ",Unidad_ID = @Unidad_ID " +
               "WHERE Receta_ID = @Receta_ID AND Ingrediente_ID = @Ingrediente_ID", tieneTableAdapter.Connection);

            dataAdapterTiene.UpdateCommand.Parameters.Add("@Receta_ID", SqlDbType.Int);
            dataAdapterTiene.UpdateCommand.Parameters.Add("@Ingrediente_ID", SqlDbType.Int);
            dataAdapterTiene.UpdateCommand.Parameters.Add("@Cantidad", SqlDbType.Decimal);
            dataAdapterTiene.UpdateCommand.Parameters.Add("@Unidad_ID", SqlDbType.Int);

            dataAdapterTiene.DeleteCommand = new SqlCommand("DELETE FROM Tiene WHERE Receta_ID = @Receta_ID AND Ingrediente_ID = @Ingrediente_ID", tieneTableAdapter.Connection);

            dataAdapterTiene.DeleteCommand.Parameters.Add("@Receta_ID", SqlDbType.Int);
            dataAdapterTiene.DeleteCommand.Parameters.Add("@Ingrediente_ID", SqlDbType.Int);

            ingredienteExiste = false;

            foreach (DataRow tiene in tieneTable.Rows)
            {
                foreach (FlowLayoutPanel IngredienteControl in flowLayoutPanel1.Controls)
                {
                    ComboBox Ingrediente = (ComboBox)IngredienteControl.Controls[0];
                    if ((int)tiene["Ingrediente_ID"] == (int)Ingrediente.SelectedValue)
                    {
                        ingredienteExiste = true;
                    }
                }

                if (!ingredienteExiste)
                {
                    dataAdapterTiene.DeleteCommand.Parameters["@Receta_ID"].Value = 1;
                    dataAdapterTiene.DeleteCommand.Parameters["@Ingrediente_ID"].Value = tiene["Ingrediente_ID"];

                    dataAdapterTiene.DeleteCommand.Connection.Open();
                    dataAdapterTiene.DeleteCommand.ExecuteNonQuery();
                    dataAdapterTiene.DeleteCommand.Connection.Close();
                    tiene.Delete();
                }

                ingredienteExiste = false;
            }

            Console.WriteLine("Numero de ingredientes en tieneTable: " + tieneTable.Rows.Count);

            foreach (DataRow row in tieneTable.Rows)
            {
                Console.WriteLine("Estado de columna: " + row.RowState);

                if (row.RowState == DataRowState.Added)
                {
                    dataAdapterTiene.InsertCommand.Parameters["@Receta_ID"].Value = 1;
                    dataAdapterTiene.InsertCommand.Parameters["@Ingrediente_ID"].Value = row["Ingrediente_ID"];
                    dataAdapterTiene.InsertCommand.Parameters["@Cantidad"].Value = row["Cantidad"];
                    dataAdapterTiene.InsertCommand.Parameters["@Unidad_ID"].Value = row["Unidad_ID"];

                    dataAdapterTiene.InsertCommand.Connection.Open();
                    dataAdapterTiene.InsertCommand.ExecuteNonQuery();
                    dataAdapterTiene.InsertCommand.Connection.Close();

                }
                else if (row.RowState == DataRowState.Modified)
                {

                    dataAdapterTiene.UpdateCommand.Parameters["@Receta_ID"].Value = 1;
                    dataAdapterTiene.UpdateCommand.Parameters["@Ingrediente_ID"].Value = row["Ingrediente_ID"];
                    dataAdapterTiene.UpdateCommand.Parameters["@Cantidad"].Value = row["Cantidad"];
                    dataAdapterTiene.UpdateCommand.Parameters["@Unidad_ID"].Value = row["Unidad_ID"];

                    dataAdapterTiene.UpdateCommand.Connection.Open();
                    dataAdapterTiene.UpdateCommand.ExecuteNonQuery();
                    dataAdapterTiene.UpdateCommand.Connection.Close();
                }
            }

            foreach (DataRow fila in tieneTable.Rows)
            {
                Console.WriteLine(fila.RowState);
            }

            tieneTable.Clear();

            dataAdapterTiene.Fill(tieneTable);

            foreach (DataRow fila in tieneTable.Rows)
            {
                Console.Write(fila["Ingrediente_ID"] + " ");
                Console.Write(fila["Cantidad"] + " ");
                Console.WriteLine(fila["Unidad_ID"] + " ");
            }
        }

        public void actualizarDBUnidades(ComboBox ComboBoxUnidades)
        {
            dataAdapterUnidades = new SqlDataAdapter("SELECT * FROM Unidades", unidadesTableAdapter.Connection);

            //llena la tabla con la informacion y esquema de la tabla de la DB
            //dataAdapterUnidades.Fill(unidadesTable);

            Console.WriteLine("Unidad " + ComboBoxUnidades.Text + " no existe en la tabla Unidades");

            SqlCommand comandoInsertar = new SqlCommand("INSERT INTO Unidades (Nombre) " +
            "VALUES ('" + ComboBoxUnidades.Text + "')", unidadesTableAdapter.Connection);

            dataAdapterUnidades.InsertCommand = comandoInsertar;

            //crea una fila con el mismo esquema que la tabla local
            DataRow unidadRow1 = unidadesTable.NewRow();

            //asigna el valor del combobox al campo de la fila
            unidadRow1["Nombre"] = ComboBoxUnidades.Text;

            //agrega una fila en la tabla Unidades local
            unidadesTable.Rows.Add(unidadRow1);

            //actualiza la tabla Unidades en la DB
            dataAdapterUnidades.Update(unidadesTable);

            Console.WriteLine("Cantidades de filas en DataTable: " + unidadesTable.Rows.Count);

            Console.WriteLine("Cantidad de items en unidadesBindingSource: " + unidadesBindingSource.Count);

            unidadesTableAdapter.Fill(pasteleriaPruebasDataSet.Unidades);
            unidadesTableAdapter.Fill(pasteleriaPruebasDataSet1.Unidades);

            Console.WriteLine("Cantidad de items en unidadesBindingSource: " + unidadesBindingSource.Count);

            Console.WriteLine("Index del objeto agregado: " + unidadesTable.Rows.Count);

            comboBoxUnidades.SelectedValue = pasteleriaPruebasDataSet.Recetas.FindByID(1).Unidad_ID;

            dataAdapterTiene = new SqlDataAdapter("SELECT * FROM Tiene WHERE Receta_ID = 1", tieneTableAdapter.Connection);

            dataAdapterTiene.Fill(tieneTable);

            for (int i = 0; i < tieneTable.Rows.Count; i++)
            {
                ComboBox Unidad = (ComboBox)flowLayoutPanel1.Controls[i].Controls[2];
                Unidad.SelectedValue = (int)tieneTable.Rows[i]["Unidad_ID"];
            }
            tieneTable.Clear();
        }

        public void AgregarIngredienteAFLowLayout(int IngredeinteID, int recetaID, int ingredienteID, decimal cantidad, int unidadID)
        {
            var flowLayoutIngrediente = new FlowLayoutPanel();
            var comboBoxIngredientes = new ComboBox();
            var textBoxUnidades = new TextBox();
            var comboBoxUnidades = new ComboBox();
            var buttonEliminar = new Button();

            Console.WriteLine("Id Ingrediente: "+ingredienteID.ToString());

            BindingSource newIngredientesBindigSource = new BindingSource();
            newIngredientesBindigSource.DataSource = pasteleriaPruebasDataSet1;
            newIngredientesBindigSource.DataMember = "Ingredientes";

            BindingSource newUnidadesBindigSource = new BindingSource();
            newUnidadesBindigSource.DataSource = pasteleriaPruebasDataSet;
            newUnidadesBindigSource.DataMember = "Unidades";

            comboBoxIngredientes.Parent = flowLayoutIngrediente;
            textBoxUnidades.Parent = flowLayoutIngrediente;
            comboBoxUnidades.Parent = flowLayoutIngrediente;
            buttonEliminar.Parent = flowLayoutIngrediente;

            textBoxUnidades.Width = 50;
            comboBoxUnidades.Width = 80;

            flowLayoutIngrediente.Name = "flowIngrediente"+ IngredeinteID;
            comboBoxIngredientes.Name = "ComBoxIngredientes" + IngredeinteID;
            buttonEliminar.Name = "btnEliminar" + IngredeinteID;

            comboBoxIngredientes.DataSource = newIngredientesBindigSource;
            comboBoxIngredientes.DisplayMember = "Nombre";
            comboBoxIngredientes.ValueMember = "ID";

            comboBoxUnidades.DataSource = newUnidadesBindigSource;
            comboBoxUnidades.DisplayMember = "Nombre";
            comboBoxUnidades.ValueMember = "ID";

            buttonEliminar.Text = "Eliminar";

            flowLayoutIngrediente.Controls.Add(comboBoxIngredientes);
            flowLayoutIngrediente.Controls.Add(textBoxUnidades);
            flowLayoutIngrediente.Controls.Add(comboBoxUnidades);
            flowLayoutIngrediente.Controls.Add(buttonEliminar);

            flowLayoutIngrediente.Width = comboBoxIngredientes.Width + textBoxUnidades.Width + comboBoxUnidades.Width + buttonEliminar.Width + 26;
            flowLayoutIngrediente.Height = 30;
            flowLayoutIngrediente.BorderStyle = BorderStyle.FixedSingle;

            flowLayoutPanel1.Controls.Add(flowLayoutIngrediente);

            comboBoxIngredientes.SelectedValue = ingredienteID;
            textBoxUnidades.Text = cantidad.ToString();
            comboBoxUnidades.SelectedValue = unidadID;

            buttonEliminar.Click += new EventHandler(Eliminar_Ingrediente);

            comboBoxIngredientes.SelectedValueChanged += new EventHandler(valorSeleccionadoCambiado);
            comboBoxIngredientes.Leave += new EventHandler(dejarComboBoxIngrediente);

            Console.WriteLine("Numero de Unidades: "+comboBoxUnidades.Items.Count);
        }

        private void valorSeleccionadoCambiado(object sender, EventArgs e)
        {
            ComboBox ingrediente = (ComboBox)sender;
            Console.WriteLine("Valor seleccionado nuevo: " + ingrediente.SelectedValue);

            foreach (FlowLayoutPanel ingredienteLayout in flowLayoutPanel1.Controls)
            {
                ComboBox ingredienteExistente = (ComboBox)ingredienteLayout.Controls[0];
                Button eliminarIngrediente = (Button)ingredienteLayout.Controls[3];

                Console.WriteLine("Valor seleccionado existente: "+ingredienteExistente.SelectedValue);

                if (ingrediente.Name != ingredienteExistente.Name)
                {
                    Console.WriteLine("Nombre del combobox de la nueva seleccion: "+ingrediente.Name);
                    Console.WriteLine("Nombre del combobox ingrediente existente: " + ingredienteExistente.Name);

                    Console.WriteLine("Los valores seleccionados son iguales?: " +( (int)ingrediente.SelectedValue == (int)ingredienteExistente.SelectedValue));

                    if ((int)ingrediente.SelectedValue == (int)ingredienteExistente.SelectedValue)
                    {
                        if (MessageBox.Show("El ingrediente ya existe, seleccione otro", "Ingrediente duplicado", MessageBoxButtons.OK) == DialogResult.OK)
                        {
                            ingrediente.Focus();
                        }
                    }                    
                }
            }
        }

        private void dejarComboBoxIngrediente(object sender, EventArgs e)
        {
            ComboBox ingrediente = (ComboBox)sender;
            Console.WriteLine("Valor seleccionado nuevo: " + ingrediente.SelectedValue);

            foreach (FlowLayoutPanel ingredienteLayout in flowLayoutPanel1.Controls)
            {
                ComboBox ingredienteExistente = (ComboBox)ingredienteLayout.Controls[0];
                Button eliminarIngrediente = (Button)ingredienteLayout.Controls[3];

                
                Console.WriteLine("Valor seleccionado existente: " + ingredienteExistente.SelectedValue);

                if (ingrediente.Name != ingredienteExistente.Name)
                {
                    Console.WriteLine("Nombre del combobox de la nueva seleccion: " + ingrediente.Name);
                    Console.WriteLine("Nombre del combobox ingrediente existente: " + ingredienteExistente.Name);

                    Console.WriteLine("Los valores seleccionados son iguales?: " + ((int)ingrediente.SelectedValue == (int)ingredienteExistente.SelectedValue));

                    if ((int)ingrediente.SelectedValue == (int)ingredienteExistente.SelectedValue)
                    {
                        if (MessageBox.Show("Elimine el ingrediente o seleccione otro\n\nDesea eliminar?", "Ingrediente duplicado", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            eliminarIngrediente.PerformClick();
                        }
                        else
                        {
                            ingrediente.Focus();
                        }
                    }
                }
            }
        }

        private void Eliminar_Ingrediente(object sender, EventArgs e)
        {
            //var button = (Button)sender;
            Button button = (Button)sender;

            Console.WriteLine(button.Name);

            Control control = button.Parent;

            Console.WriteLine(control.Name);

            flowLayoutPanel1.Controls.Remove(control);
        }

        private void BtnAgregarNuevoIngrediente_Click(object sender, EventArgs e)
        {
            int idIngrediente = flowLayoutPanel1.Controls.Count + 1;

            bool ingredienteExiste = false;

            foreach (FlowLayoutPanel IngredienteFlowLayout in flowLayoutPanel1.Controls)
            {
                ComboBox comboIngredientes = (ComboBox) IngredienteFlowLayout.Controls[0];

                Console.WriteLine("Combo Ingrediente Selected Value: "+comboIngredientes.SelectedValue);
                Console.WriteLine("Combo Nuevo Ingrediente Selecte Value: " + CmbBoxNevoIngrediente.SelectedValue);
                
                if ((int)CmbBoxNevoIngrediente.SelectedValue == (int)comboIngredientes.SelectedValue)
                {
                    ingredienteExiste = true;
                }

            }
            Console.WriteLine("Ingrediente Existe? " + ingredienteExiste);
            if (TxtBoxNuevaCantidad.Text == "")
            {
                MessageBox.Show("Agrege una cantidad", "Cantidad Vacía", MessageBoxButtons.OK);
            }
            else
            {
                if (!ingredienteExiste)
                {
                    AgregarIngredienteAFLowLayout(idIngrediente, 1, (int)CmbBoxNevoIngrediente.SelectedValue, int.Parse(TxtBoxNuevaCantidad.Text), (int)CmbBoxNuevaUnidad.SelectedValue);
                }
                else
                {
                    MessageBox.Show("El ingrediente ya esta agregado, puede cambiar la cantidad", "Ingrediente Repetido", MessageBoxButtons.OK);
                }
            }
        }

        private void ComboBoxUnidadLeave(object sender, EventArgs e)
        {
            ComboBox comboBoxUnidades = (ComboBox)sender;

            SqlDataAdapter dataAdapterUnidades = new SqlDataAdapter("SELECT * FROM Unidades", unidadesTableAdapter.Connection);

            //llena la tabla con la informacion y esquema de la tabla de la DB
            dataAdapterUnidades.Fill(unidadesTable);

            Console.WriteLine(unidadesTable.Rows.Count);

            bool valorExiste = false;

            for (int i = 0; i < unidadesTable.Rows.Count; i++)
            {
                DataRow unidadRow = unidadesTable.Rows[i];
                Console.Write(unidadRow["ID"]);
                Console.Write("   ");
                Console.WriteLine(unidadRow["Nombre"]);
                if (comboBoxUnidades.Text == unidadRow["Nombre"].ToString())
                {
                    valorExiste = true;
                }
            }

            if (!valorExiste)
            {
                var confirmarUpdate = MessageBox.Show("Deseas agregar: " + comboBoxUnidades.Text + " a la lista de Unidades?",
                                     "Agregar Item",
                                     MessageBoxButtons.YesNo);
                if (confirmarUpdate == DialogResult.Yes)
                {
                    comboBoxUnidades.Focus();
                    actualizarDBUnidades(comboBoxUnidades);
                }
                else
                {
                    comboBoxUnidades.Focus();
                }
            }
            else
            {
                unidadesTable.Clear();
            }
        }
    }
}