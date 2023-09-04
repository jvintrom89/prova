using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        Socket server;
        Thread atender;
        int num_mapa;
        int Identificador;
        delegate void DelegadoParaActualizarLista(string[] trozos);
        delegate void DelegadoParaVisualizarJugMasPunt();
        delegate void DelegadoParaVisualizarColRob3();
        delegate void DelegadoParaVisualizarTiempo();
        delegate void DelegadoParaVisualizarenviar();
        delegate void DelegadoParaVisualizardesconectar();
        delegate void DelegadoParaVisualizarlabel2();
        delegate void DelegadoParaVisualizarcontrasena();
        delegate void DelegadoParaVisualizarnombre();
        delegate void DelegadoParaVisualizarBoxContrasena();
        delegate void DelegadoParaVisualizarconectar();
        delegate void DelegadoParaVisualizarpartida();
        delegate void DelegadoParaVisualizarJugar();

        public Form1()
        {
            InitializeComponent();
            //Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

           
        }
        private void VisualizarJugMasPunt()
        {
            ListadoOponentes.Visible = true;
        }
        private void VisualizarColRob3()
        {
            NumPartidas.Visible = true;
        }
        private void VisualizarTiempo()
        {
            Tiempo.Visible = true;
        }
        private void Visualizarenviar()
        {
            enviar.Visible = true;
        }
        private void Visualizardesconectar()
        {
            desconectar.Visible = true;
        }
        private void Visualizarlabel2()
        {
            label2.Visible = false;
        }
        private void Visualizarcontrasena()
        {
            contrasena.Visible = false;
        }
        private void Visualizarnombre()
        {
            nombre.Visible = false;
        }
        private void VisualizarBoxContrasena()
        {
            BoxContrasena.Visible = false;
        }
        private void Visualizarconectar()
        {
            conectar.Visible = false;
        }
        private void Visualizarpartida()
        {
            partida.Visible = true;
        }
        private void VisualizarJugar()
        {
            Jugar.Visible = true;
        }

        public void ActualizaGrid(string[] trozos)
        {
            dataGridView1.Visible = true;

            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            int numconectados = Convert.ToInt32(trozos[2]);
            dataGridView1.ColumnCount = 1;
            if(numconectados==0)
                dataGridView1.Visible = false;
            else
                dataGridView1.RowCount = numconectados;
            int k = 3;
            int i = 0;
            while (i < numconectados)
            {
                dataGridView1.Rows[i].Cells[0].Value = trozos[k].Split('/')[0];
                i++;
                k++;
            }
        }
    

        private void AtenderServidor()
        {
            while (true)
            {
                //Recibimos la respuesta del servidor
                
                byte[] msg = new byte[80];
                server.Receive(msg);
                string msg2 = System.Text.Encoding.ASCII.GetString(msg).Split('\0')[0];
                string[] trozos = msg2.Split('/');
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje;
                
                switch (codigo)
                {

                    case 1:  //Consulta 1 (Listado de jugadores con los que he echado alguna partida)
                        mensaje = trozos[1].Split('\0')[0];
                        MessageBox.Show(mensaje);
                        break;

                    case 2:      //Consulta 2 (Nu￺mero de partidas jugadas)
                        mensaje = trozos[1].Split('\0')[0];

                        MessageBox.Show(mensaje);

                        break;
                    case 3:       //Consulta 3 (Tiempo partida de Juan)
                        mensaje = trozos[1].Split('\0')[0];

                        MessageBox.Show(mensaje);
                        break;
                    case 4:     //Registrarse
                        mensaje = trozos[1].Split('\0')[0];

                        MessageBox.Show(mensaje);

                        // Nos desconectamos del servidor
                        this.BackColor = Color.Snow;
                        atender.Abort();

                        break;

                    case 5: // Conectarse y desconectarse
                        mensaje = trozos[1].Split('\0')[0];
                        if (mensaje == "Conectado correctamente")
                        {
                            MessageBox.Show("Conectado correctamente");
                            ListadoOponentes.Invoke(new DelegadoParaVisualizarJugMasPunt(VisualizarJugMasPunt));
                            NumPartidas.Invoke(new DelegadoParaVisualizarColRob3(VisualizarColRob3));
                            Tiempo.Invoke(new DelegadoParaVisualizarTiempo(VisualizarTiempo));
                            enviar.Invoke(new DelegadoParaVisualizarenviar(Visualizarenviar));
                            desconectar.Invoke(new DelegadoParaVisualizardesconectar(Visualizardesconectar));
                            label2.Invoke(new DelegadoParaVisualizarlabel2(Visualizarlabel2));
                            contrasena.Invoke(new DelegadoParaVisualizarcontrasena(Visualizarcontrasena));
                            nombre.Invoke(new DelegadoParaVisualizarnombre(Visualizarnombre));
                            BoxContrasena.Invoke(new DelegadoParaVisualizarBoxContrasena(VisualizarBoxContrasena));
                            conectar.Invoke(new DelegadoParaVisualizarconectar(Visualizarconectar));
                            partida.Invoke(new DelegadoParaVisualizarpartida(Visualizarpartida));

                            dataGridView1.Invoke(new DelegadoParaActualizarLista(ActualizaGrid), new object[] { trozos });
                        }
                        else if (mensaje == "Desconectado correctamente")
                        {
                            dataGridView1.Invoke(new DelegadoParaActualizarLista(ActualizaGrid), new object[] { trozos });

                        }
                        else
                            MessageBox.Show("Registrate primero");
                        break;

                    case 6: //Notificación de partida
                        string nombren = trozos[1].Split('\0')[0];
                        string id = trozos[2].Split('\0')[0];
                        Identificador = Int32.Parse(id);

                        DialogResult result = MessageBox.Show("¿Deseas jugar con "+ nombren + "?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            // Acciones si el usuario selecciona "Sí"
                            //Codigo/Nombre/IdPartida/Aceptar o Rechazar

                            string mensaje2 = "7/" + nombre.Text + "/" + id + "/1" ;
                            byte[] msg3 = System.Text.Encoding.ASCII.GetBytes(mensaje2);
                            server.Send(msg3);
                        }
                        else if (result == DialogResult.No)
                        {
                            // Acciones si el usuario selecciona "No"
                            string mensaje2 = "7/" + nombre.Text + "/" + id + "/0";
                            byte[] msg3 = System.Text.Encoding.ASCII.GetBytes(mensaje2);
                            server.Send(msg3);
                        }

                        break;

                    case 7://Respuesta de jugador
                        nombren = trozos[1].Split('\0')[0];
                        string aceptado = trozos[2].Split('\0')[0];

                        if (aceptado=="1")
                        {
                            MessageBox.Show(nombren + " ha aceptado tu solicitud :)");
                        }
                        else
                        {
                            MessageBox.Show(nombren + " ha rechazado tu solicitud :(");
                        }

                        break;

                    case 8://Respuesta de partida
                        //nombren = trozos[1].Split('\0')[0];
                        string completa = trozos[1].Split('\0')[0];
                        String n_map = trozos[2].Split('\0')[0];
                        //MessageBox.Show(completa+" "+n_map);

                        if (completa == "1")
                        {
                            MessageBox.Show("Todos han aceptado la partida. Vamos a jugar.");
                            num_mapa = Int32.Parse(n_map);
                            Jugar.Invoke(new DelegadoParaVisualizarJugar(VisualizarJugar));
                        }
                        else
                        {
                            MessageBox.Show("Algun usuario ha cancelado la partida. No se juega.");
                        }

                        break;
                    case 11:  //Darse de baja
                        mensaje = trozos[1].Split('\0')[0];
                        MessageBox.Show(mensaje);

                        // Nos desconectamos del servidor
                        this.BackColor = Color.Snow;
                        atender.Abort();

                        break;
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (NumPartidas.Checked)
            {   //N￺umero de partidas jugadas
                //Enviamos nombre y contraseña
                string mensaje = "2/" + nombre.Text + "/" + BoxContrasena.Text;

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else if (ListadoOponentes.Checked)
            {
                //Enviamos nombre y contraseña
                string mensaje = "1/" + nombre.Text + "/" + BoxContrasena.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else if (Tiempo.Checked)
            {   //Encontrar el timepo de la partida de Juan
                //Enviamos nombre y contraseña
                    string mensaje = "3/" + nombre.Text + "/" + BoxContrasena.Text;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
            }
        }

        private void conectar_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9000);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
                

                //Enviamos nombre y contraseña
                string mensaje = "5/" + nombre.Text + "/" + BoxContrasena.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                
            }

            catch (SocketException ex)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }

            //pongo en marcha el thread que atenderá los mensajes del servidor
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();

        }

        private void desconectar_Click(object sender, EventArgs e)
        {

            ListadoOponentes.Visible = false;
            NumPartidas.Visible = false;
            Tiempo.Visible = false;
            enviar.Visible = false;
            desconectar.Visible = false;
            label2.Visible = true;
            contrasena.Visible = true;
            nombre.Visible = true;
            BoxContrasena.Visible = true;
            conectar.Visible = true;
            partida.Visible = false;

            dataGridView1.Visible = false;

            //Mensaje de desconexión
            string mensaje = "0/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            atender.Abort();
            this.BackColor = Color.Gray;
            MessageBox.Show("Desconectado correctamente");
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }


        private void Registrate_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9000);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
                
                //Enviamos los datos para realizar el registro
                string mensaje = "4/" + nombreregistro.Text + "/" + contrasenaregistro.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            catch (SocketException ex)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }

            //pongo en marcha el thread que atenderá los mensajes del servidor
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Jugar_Click(object sender, EventArgs e)
        {
            atender.Suspend();

            // Crear una instancia del formulario que se desea abrir
            Form2 Form2 = new Form2(num_mapa, Identificador, nombre.Text, server);
            this.Hide();
            // Mostrar el formulario secundario
            Form2.Show();

        }

        private void incia_sesion_Paint(object sender, PaintEventArgs e)
        {

        }

        private void sesion_inicidada_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //int fila = e.rowindex;
            //int columna = e.columnindex;

            //verificar que se ha clicado una celda válida
            //if (fila >= 0 && columna >= 0)
            //{
            //    // obtener el valor de la celda clicada
            //    object valorcelda = datagridview1.rows[fila].cells[columna].value;

            //    // hacer algo con el valor obtenido


            //    string mensaje = "6/" + valorcelda.tostring();
            //    byte[] msg = system.text.encoding.ascii.getbytes(mensaje);
            //    server.send(msg);
            //}
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void invitar_Click(object sender, EventArgs e)
        {
            int longitud = dataGridView1.SelectedCells.Count;
            string mensaje = "6/" + nombre.Text + "/";
            //bucle for para comprobar a que jugadores se invita
            if (longitud > 4 || longitud == 0)
            {
                MessageBox.Show("4 Jugadores máximo y 2 mínimo");
            }
            else
            {
                for (int i = 0; i < longitud; ++i)
                {

                    object valorcelda = dataGridView1.SelectedCells[i].Value;
                    if (nombre.Text == valorcelda.ToString())
                    {
                        MessageBox.Show("No te puedes autoinvitar a una partida.");
                    }

                    else
                    {
                        mensaje += valorcelda + "/";
                    }
                }
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }

        private void DarBaja_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9000);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;

                //Enviamos los datos para realizar la baja
                string mensaje = "10/" + nombreregistro.Text + "/" + contrasenaregistro.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            catch (SocketException ex)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }

            //pongo en marcha el thread que atenderá los mensajes del servidor
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }
    }
}
