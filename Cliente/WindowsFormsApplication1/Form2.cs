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
using System.Reflection.Emit;
using System.Collections;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security.Policy;
using System.Timers;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        List<PictureBox> ListaSerpientes = new List<PictureBox>();
        List<PictureBox> ListaPuntos = new List<PictureBox>();
        List<PictureBox> ListaMuros = new List<PictureBox>();
        List<PictureBox> ListaSpawn = new List<PictureBox>();
        int n_mapa;
        String nombre;
        Socket server;
        int id;
        PictureBox Pnts = new PictureBox();
        String Direccion; // Dirección del jugador
        int n_jug = 0;
        private static System.Timers.Timer timer1, crono;
        private TimeSpan tiempoRestante;

        //Parametros de inicio constantes de jugadores
        char[] C_COLOR = {'y', 'r', 'g', 'b' };
        String[] C_COLORN = { "Amarillo", "Rojo", "Verde", "Azul" };
        string[] C_DIRECCION = { "r", "l", "u", "d" };
        int[] C_CABX = { 78, 780, 0, 858 };
        int[] C_CABY = { 0, 546, 468, 78 };
        int[] C_COL1X = { 52, 806, 0, 858 };
        int[] C_COL1Y = { 0, 546, 494, 52 };
        int[] C_COL2X = { 26, 832, 0, 858 };
        int[] C_COL2Y = { 0, 546, 520, 26 };
        int[] C_COL3X = { 0, 858, 0, 858 };
        int[] C_COL3Y = { 0, 546, 546, 0 };



        delegate void DelegadoParaBtnEmpezar();
        delegate void DelegadoAñadirSerpientes(PictureBox p);
        delegate void DelegadoBorrarSerpientes(PictureBox p);
        delegate void DelegadoActualizarPuntos(string a, string b, string c, string d);
        delegate void DelegadoNegrita(System.Windows.Forms.Label C);
        delegate void Delegadocrono(String tiempo);

        Thread atender;
        Jugador[] listajugadores;
        int j_cliente;
        
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

                    case 9:  //
                        mensaje = trozos[1].Split('\0')[0];
                        if (Int32.Parse(mensaje) != 0)
                        {
                            n_jug = Int32.Parse(trozos[2].Split('\0')[0]);
                            //MessageBox.Show(mensaje);
                            listajugadores = new Jugador[n_jug];
                            for (int j = 0; j < n_jug; ++j)
                            {
                                AñadirJugador(j, trozos[3+j].Split('\0')[0]);
                            }
                            PintarSerpientes(mapa,"crear");
                            //timer1.Interval = 1000;
                            //timer1.Start();
                            timer1.Enabled = true;
                            crono.Enabled = true;
                            marcar_jugador();
                        }
                        break;

                    case 10:
                        string user = trozos[1].Split('\0')[0];
                        string direcc = trozos[2].Split('\0')[0];
                        CambioDireccion(user,direcc);

                        break;
                }
            }
        }

        private void FinalizarPartida(string j)
        {
            crono.Stop();
            timer1.Stop();
            Jugador campeon = new Jugador();
            campeon.Puntos = 0;
            TimeSpan tiempo1 = TimeSpan.FromSeconds(90);
            TimeSpan diferencia = tiempo1 - tiempoRestante;
            foreach (Jugador x in listajugadores)
            {
                if (x.Puntos > campeon.Puntos) campeon = x;
            }
            string mensaje;
            if (j == "Puntos") mensaje = "Partida finalizada por maximo de puntos";
            else mensaje = "Partida finalizada por tiempo";
            MessageBox.Show(mensaje+"/n"+campeon.Nombre + "ha ganado!/nPuntos: "+campeon.Puntos+"/nLa partida ha durado: "+ diferencia.ToString());
        }

        private void PintarSerpientes(PictureBox mapa, String opcion) 
        {
            if (opcion == "borrar")
            {
                foreach (PictureBox x in ListaSerpientes)
                {
                    object[] arg1 = { x };
                    mapa.Invoke(new DelegadoBorrarSerpientes(BorrarSerpiente), arg1);
                }
                ListaSerpientes.Clear();
            }
            foreach (Jugador x in listajugadores)
            {
                string recurso;
                if (x.Color == 'y') recurso = "amarillo";
                else if (x.Color == 'r') recurso = "rojo";
                else if (x.Color == 'b') recurso = "azul";
                else recurso = "verde";
                PictureBox pb = new PictureBox();
                pb = new PictureBox();
                pb.Location = new Point(x.Poscabx, x.Poscaby);
                pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(recurso);
                pb.BackColor = Color.Transparent;
                pb.SizeMode = PictureBoxSizeMode.AutoSize;
                pb.Tag = recurso;
                ListaSerpientes.Add(pb);

                object[] arg1 = { pb };
                mapa.Invoke(new DelegadoAñadirSerpientes(DibujarSerpiente), arg1);
                //----------------
                pb = new PictureBox();
                pb.Location = new Point(x.Cola1x, x.Cola1y);
                pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("cola"+recurso);
                pb.BackColor = Color.Transparent;
                pb.SizeMode = PictureBoxSizeMode.AutoSize;
                pb.Tag = "cola" + recurso;
                ListaSerpientes.Add(pb);

                object[] arg2 = { pb };
                mapa.Invoke(new DelegadoAñadirSerpientes(DibujarSerpiente), arg2);
                //----------------
                pb = new PictureBox();
                pb.Location = new Point(x.Cola2x, x.Cola2y);
                pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("cola" + recurso);
                pb.BackColor = Color.Transparent;
                pb.SizeMode = PictureBoxSizeMode.AutoSize;
                pb.Tag = "cola" + recurso;
                ListaSerpientes.Add(pb);

                object[] arg3 = { pb };
                mapa.Invoke(new DelegadoAñadirSerpientes(DibujarSerpiente), arg3);
                //----------------
                pb = new PictureBox();
                pb.Location = new Point(x.Cola3x, x.Cola3y);
                pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("cola" + recurso);
                pb.BackColor = Color.Transparent;
                pb.SizeMode = PictureBoxSizeMode.AutoSize;
                pb.Tag = "cola" + recurso;
                ListaSerpientes.Add(pb);

                object[] arg4 = { pb };
                mapa.Invoke(new DelegadoAñadirSerpientes(DibujarSerpiente), arg4);
            }
        }

        void CambioDireccion(string user, string direcc)
        {
            foreach (Jugador x in listajugadores)
            {
                if (x.Nombre == user) 
                {
                    x.Direccion = direcc;
                    Console.WriteLine(x.Nombre + "Cambia la dirección a: " + direcc);
                }
            }
        }

        private void AñadirJugador(int j, String x)
        {
            //MessageBox.Show("Añadiendo Nombre: " + x);
            if (nombre == x)  
            { 
                j_cliente = j;
                //MessageBox.Show(x + " eres el jugador" + C_COLORN[j]);
            }
            Jugador jugador = new Jugador();
            jugador.Nombre = x;
            jugador.Color = C_COLOR[j];
            jugador.Direccion = C_DIRECCION[j];
            jugador.Poscabx = C_CABX[j];
            jugador.Poscaby = C_CABY[j];
            jugador.Cola1x = C_COL1X[j];
            jugador.Cola1y = C_COL1Y[j];
            jugador.Cola2x = C_COL2X[j];
            jugador.Cola2y = C_COL2Y[j];
            jugador.Cola3x = C_COL3X[j];
            jugador.Cola3y = C_COL3Y[j];
            jugador.Puntos = 0;
            listajugadores[j] = jugador;
        }
        
        public Form2(int n, int i, string nom, Socket srv)
        {
            InitializeComponent();
            DefinirPartida(n);
            nombre = nom;
            server = srv;
            id = i;
        }

        private void btnEmpezar()
        {
            BotonEmpezar.Text = "Esperando...";
            BotonEmpezar.Enabled = false;
        }

        private void DibujarSerpiente(PictureBox p)
        {
            mapa.Controls.Add(p);
            p.BringToFront();
        }

        private void MostrarPuntos(string a, string b, string c, string d)
        {
            PuntosY.Text = a;
            PuntosR.Text = b;
            if (c != null && c != "c") PuntosG.Text = c;
            if (d != null && d != "c") PuntosB.Text = c;
        }

        private Jugador BuscarJugadorActual()
        {
            foreach (Jugador x in listajugadores)
            {
                if (x.Nombre == nombre) {
                    //Console.WriteLine("Calculo jugador! Resultado: " + x.Nombre + "-" + x.Color);
                    return x;
                } 
            }
            return null;
        }

        private void BorrarSerpiente(PictureBox p)
        {
            mapa.Controls.Remove(p);
        }
        private void Negrita(System.Windows.Forms.Label C)
        {
            C.Font = new System.Drawing.Font(C.Font, FontStyle.Bold);
            Size size = TextRenderer.MeasureText(C.Text, C.Font);
            C.Width = size.Width;
            C.Height = size.Height;
        }
        private void tiempo(string s)
        {
            cronometro1.Text = s;
        }
        private void DefinirPartida(int n)
        {
            //timer1.Stop();
            Pnts.Text = "0";
            ListaSerpientes = new List<PictureBox>();
            ListaPuntos = new List<PictureBox>();
            ListaMuros = new List<PictureBox>();
            ListaSpawn = new List<PictureBox>();

            timer1 = new System.Timers.Timer();
            timer1.Interval = 500;
            timer1.Elapsed += timer1_Tick;
            timer1.AutoReset = true;

            tiempoRestante = TimeSpan.FromSeconds(90); // Establecer el tiempo inicial a 2 minutos

            // Configurar el temporizador
            crono = new System.Timers.Timer();
            crono.Interval = 1000; // Actualizar cada segundo
            crono.Elapsed += crono_Tick;
            //crono.AutoReset = true;

            n_mapa = n;
            GenerarMapa(n_mapa);
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }

        private void GenerarMapa(int j)
        {
            if (j == 1 || j == 2 || j == 3)
            {
                CrearPuntos(ListaPuntos, mapa, 208, 130);
                CrearPuntos(ListaPuntos, mapa, 104, 312);
                CrearPuntos(ListaPuntos, mapa, 208, 442);
                CrearPuntos(ListaPuntos, mapa, 416, 156);
                CrearPuntos(ListaPuntos, mapa, 390, 364);
                CrearPuntos(ListaPuntos, mapa, 702, 52);
                CrearPuntos(ListaPuntos, mapa, 650, 260);
                CrearPuntos(ListaPuntos, mapa, 598, 442);

                CrearSpawn(ListaSpawn, mapa);


                for (int i = 52; i <= 260; i = i + 26)
                {
                    CrearMuro(ListaMuros, mapa, 312, i);
                }

                for (int i = 52; i <= 420; i = i + 26)
                {
                    CrearMuro(ListaMuros, mapa, 468, i);
                }

                for (int i = 52; i <= 364; i = i + 26)
                {
                    CrearMuro(ListaMuros, mapa, i, 416);
                }

                for (int i = 598; i <= 832; i = i + 26)
                {
                    CrearMuro(ListaMuros, mapa, i, 364);
                }
            }
        }

        public void CrearSpawn(List<PictureBox> Lista, PictureBox mapa)
        {
            PictureBox pb = new PictureBox();
            pb.Location = new Point(0,0);
            pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("cuadrado1");
            pb.BackColor = Color.Transparent;
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            pb.Tag = "spawn";
            Lista.Add(pb);
            mapa.Controls.Add(pb);
            pb = new PictureBox();
            pb.Location = new Point(858, 0);
            pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("cuadrado2");
            pb.BackColor = Color.Transparent;
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            pb.Tag = "spawn";
            Lista.Add(pb);
            mapa.Controls.Add(pb);
            pb = new PictureBox();
            pb.Location = new Point(0, 546);
            pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("cuadrado3");
            pb.BackColor = Color.Transparent;
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            pb.Tag = "spawn";
            Lista.Add(pb);
            mapa.Controls.Add(pb);
            pb = new PictureBox();
            pb.Location = new Point(858, 546);
            pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("cuadrado4");
            pb.BackColor = Color.Transparent;
            pb.Tag = "spawn";
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            Lista.Add(pb);
            mapa.Controls.Add(pb);
        }

        public void CrearPuntos(List<PictureBox> Lista, PictureBox mapa, int posicionx, int posiciony)
        {
            PictureBox pb = new PictureBox();
            pb.Location = new Point(posicionx, posiciony);
            pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("Puntos_estelares");
            pb.BackColor = Color.Transparent;
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            pb.Tag = "puntos";
            Lista.Add(pb);
            mapa.Controls.Add(pb);
        }

        public void ActualizarPuntos()
        {
            String a = null;
            String b = null;
            String c = null;
            String d = null;
            if (n_jug == 2)
            {
                a = listajugadores[0].Puntos.ToString();
                b = listajugadores[1].Puntos.ToString();
                c = null;
                d = null;
                if (Int32.Parse(a) == 30 || Int32.Parse(b) == 30) FinalizarPartida("puntos");
            }
            else if (n_jug == 3)
            {
                a = listajugadores[0].Puntos.ToString();
                b = listajugadores[1].Puntos.ToString();
                c = listajugadores[2].Puntos.ToString();
                d = null;
                if (Int32.Parse(a) == 30 || Int32.Parse(b) == 30 || Int32.Parse(c) == 30) FinalizarPartida("puntos");
            }
            else if (n_jug == 4)
            {
                a = listajugadores[0].Puntos.ToString();
                b = listajugadores[1].Puntos.ToString();
                c = listajugadores[2].Puntos.ToString();
                d = listajugadores[3].Puntos.ToString();
                if (Int32.Parse(a) == 30 || Int32.Parse(b) == 30 || Int32.Parse(c) == 30 || Int32.Parse(d) == 30) FinalizarPartida("puntos");
            }
            Object[] arg1 = {a, b, c, d};
            mapa.Invoke(new DelegadoActualizarPuntos(MostrarPuntos), arg1);
        }

        public void CrearMuro(List<PictureBox> Lista, PictureBox mapa, int posicionx, int posiciony)
        {
            PictureBox pb = new PictureBox();
            pb.Location = new Point(posicionx, posiciony);
            pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("murito");
            pb.BackColor = Color.Transparent;
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            pb.Tag = "muros";
            Lista.Add(pb);
            mapa.Controls.Add(pb);
        }
        //Teclas de movimiento de Jugador
        private void MoverJugador(object sender, KeyEventArgs e)
        {
            int i = 0;
            int n = 0;

            Direccion = ((e.KeyCode & Keys.Up) == Keys.Up) ? "u" : Direccion;
            Direccion = ((e.KeyCode & Keys.Down) == Keys.Down) ? "d" : Direccion;
            Direccion = ((e.KeyCode & Keys.Left) == Keys.Left) ? "l" : Direccion;
            Direccion = ((e.KeyCode & Keys.Right) == Keys.Right) ? "r" : Direccion;

            foreach (Jugador z in listajugadores)
            {
                if (z.Nombre == nombre) n = i;
                ++i;
            }

            if ((listajugadores[n].Direccion == "r" && listajugadores[n].Direccion != Direccion && Direccion != "l") ||
                (listajugadores[n].Direccion == "l" && listajugadores[n].Direccion != Direccion && Direccion != "r") ||
                (listajugadores[n].Direccion == "u" && listajugadores[n].Direccion != Direccion && Direccion != "d") ||
                (listajugadores[n].Direccion == "d" && listajugadores[n].Direccion != Direccion && Direccion != "u"))
            {
                string mensaje = "9/" + nombre + "/" + id + "/" + Direccion;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void marcar_jugador()
        {
            Jugador yo = BuscarJugadorActual();
            if (yo.Color == 'y')
            {
                Y.BackColor = Color.Yellow;
                object[] arg1 = { Y };
                mapa.Invoke(new DelegadoNegrita(Negrita), arg1);
            } else if (yo.Color == 'r')
            {
                R.BackColor = Color.Red;
                object[] arg1 = { R };
                mapa.Invoke(new DelegadoNegrita(Negrita), arg1);
            }
            else if (yo.Color == 'b')
            {
                B.BackColor = Color.Blue;
                object[] arg1 = { R };
                mapa.Invoke(new DelegadoNegrita(Negrita), arg1);
            }
            else
            {
                G.BackColor = Color.Green;
                object[] arg1 = { G };
                mapa.Invoke(new DelegadoNegrita(Negrita), arg1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BotonEmpezar.Invoke(new DelegadoParaBtnEmpezar(btnEmpezar));
            string mensaje = "8/" + nombre + "/" +id;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void crono_Tick(object sender, EventArgs e)
        {
            tiempoRestante = tiempoRestante.Subtract(TimeSpan.FromSeconds(1));

            if (tiempoRestante.TotalSeconds <= 0)
            {
                FinalizarPartida("tiempo");
            }
            object[] arg1 = { tiempoRestante.ToString(@"mm\:ss") };
            mapa.Invoke(new Delegadocrono(tiempo), arg1);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //MessageBox.Show("Hola? ");
            foreach (Jugador x in listajugadores)
            {
                //MessageBox.Show("Moviendo jugador "+x.Nombre);
                PictureBox pb = new PictureBox();
                if (x.Direccion == "r")
                {
                    Console.WriteLine(x.Nombre + "Me muevo a la derecha");
                    pb.Location = new Point(x.Poscabx + 26, x.Poscaby);
                    pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("rojo");
                    pb.BackColor = Color.Transparent;
                    pb.SizeMode = PictureBoxSizeMode.AutoSize;
                    pb.Tag = "nuevo";
                    bool colision = false;
                    foreach (PictureBox y in ListaMuros)
                    {
                        if (!colision && (pb.Bounds.IntersectsWith(y.Bounds) || x.Poscabx + 26 > 859))
                        {
                            Console.WriteLine(x.Nombre + "Me he chocado con un Muro o Fin mapa(" + y.Tag.ToString() + ")");
                            Console.WriteLine("--Posicion Jugador: ("+x.Poscabx+","+x.Poscaby+ ")("+pb.Width+","+pb.Height+")");
                            Console.WriteLine("--Posicion Objeto: (" + y.Location.X + "," + y.Location.Y + ")(" + y.Width + "," + y.Height + ")");
                            colision = true;
                        }
                    }
                    for (int contador = 0; contador < ListaPuntos.Count && !colision; contador++)
                    {
                        if (ListaPuntos[contador].Bounds.IntersectsWith(pb.Bounds))
                        {

                            Console.WriteLine(x.Nombre + "Consigo puntos(" + ListaPuntos[contador].Tag.ToString() + ")");
                            Console.WriteLine("--Posicion Jugador: (" + x.Poscabx + "," + x.Poscaby + ")(" + pb.Width + "," + pb.Height + ")");
                            Console.WriteLine("--Posicion Objeto: (" + ListaPuntos[contador].Location.X + "," + ListaPuntos[contador].Location.Y + ")(" + ListaPuntos[contador].Width + "," + ListaPuntos[contador].Height + ")");
                            colision = true;
                            x.Puntos += 5;
                            ActualizarPuntos();
                            object[] arg1 = { ListaPuntos[contador] };
                            mapa.Invoke(new DelegadoBorrarSerpientes(BorrarSerpiente), arg1);
                            ListaPuntos.Remove(ListaPuntos[contador]);
                            x.Cola3y = x.Cola2y;
                            x.Cola3x = x.Cola2x;
                            x.Cola2y = x.Cola1y;
                            x.Cola2x = x.Cola1x;
                            x.Cola1y = x.Poscaby;
                            x.Cola1x = x.Poscabx;
                            x.Poscabx += 26;
                        }
                    }
                    foreach (PictureBox y in ListaSerpientes)
                    {
                        if (!colision)
                        {
                            char co;
                            int n = 0;
                            string c = y.Tag.ToString().Replace("cola", "").Substring(0, 2);
                            if (c == "am") co = 'y';
                            else if (c == "az") co = 'b';
                            else if (c == "ro") co = 'r';
                            else co = 'g';
                            int i = 0;
                            foreach (Jugador z in listajugadores)
                            {
                                if (z.Color == co) n = i;
                                ++i;
                            }
                            //MessageBox.Show("XNueva: "+Convert.ToString(po.X)+", XSerpiente: " + Convert.ToString(ListaSerpientes[contador].Location.X));
                            if (y.Bounds.IntersectsWith(pb.Bounds)
                                && x.Color != co)
                            {
                                //Console.WriteLine(x.Nombre + " Colisiona con: " + y.Tag.ToString());
                                Console.WriteLine(x.Nombre + "Me han comido. Yo: " + x.Color + "-" + co + y.Tag.ToString());
                                colision = true;
                                int j;
                                if (y.Tag.ToString().Substring(0, 4) != "cola")
                                {
                                    j = Array.IndexOf(C_COLOR, listajugadores[n].Color);
                                    listajugadores[n].Direccion = C_DIRECCION[j];
                                    listajugadores[n].Poscabx = C_CABX[j];
                                    listajugadores[n].Poscaby = C_CABY[j];
                                    listajugadores[n].Cola1x = C_COL1X[j];
                                    listajugadores[n].Cola1y = C_COL1Y[j];
                                    listajugadores[n].Cola2x = C_COL2X[j];
                                    listajugadores[n].Cola2y = C_COL2Y[j];
                                    listajugadores[n].Cola3x = C_COL3X[j];
                                    listajugadores[n].Cola3y = C_COL3Y[j];
                                    listajugadores[n].Puntos = 0;
                                    ActualizarPuntos();

                                }
                                j = Array.IndexOf(C_COLOR, x.Color);
                                x.Direccion = C_DIRECCION[j];
                                x.Poscabx = C_CABX[j];
                                x.Poscaby = C_CABY[j];
                                x.Cola1x = C_COL1X[j];
                                x.Cola1y = C_COL1Y[j];
                                x.Cola2x = C_COL2X[j];
                                x.Cola2y = C_COL2Y[j];
                                x.Cola3x = C_COL3X[j];
                                x.Cola3y = C_COL3Y[j];
                                listajugadores[n].Puntos += x.Puntos;
                                x.Puntos = 0;
                                ActualizarPuntos();
                            }
                        }
                        
                    }
                    if (!colision)
                    {
                        //MessageBox.Show("Me muevoooo " + x.Nombre);
                        x.Cola3y = x.Cola2y;
                        x.Cola3x = x.Cola2x;
                        x.Cola2y = x.Cola1y;
                        x.Cola2x = x.Cola1x;
                        x.Cola1y = x.Poscaby;
                        x.Cola1x = x.Poscabx;
                        x.Poscabx += 26;
                    }
                }
                if (x.Direccion == "l")
                {
                    Console.WriteLine(x.Nombre + "Me muevo a la izquierda");
                    pb.Location = new Point(x.Poscabx - 26, x.Poscaby);
                    pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("rojo");
                    pb.BackColor = Color.Transparent;
                    pb.SizeMode = PictureBoxSizeMode.AutoSize;
                    pb.Tag = "nuevo";
                    bool colision = false;
                    foreach (PictureBox y in ListaMuros)
                    {
                        if (!colision && (pb.Bounds.IntersectsWith(y.Bounds) || x.Poscabx - 26 < 0))
                        {
                            Console.WriteLine(x.Nombre + "Me he chocado con un Muro o Fin mapa(" + y.Tag.ToString() + ")");
                            Console.WriteLine("--Posicion Jugador: (" + x.Poscabx + "," + x.Poscaby + ")(" + pb.Width + "," + pb.Height + ")");
                            Console.WriteLine("--Posicion Objeto: (" + y.Location.X + "," + y.Location.Y + ")(" + y.Width + "," + y.Height + ")");
                            colision = true;
                        }
                    }
                    for (int contador = 0; contador < ListaPuntos.Count && !colision; contador++)
                    {
                        if (ListaPuntos[contador].Bounds.IntersectsWith(pb.Bounds))
                        {
                            Console.WriteLine(x.Nombre + "Consigo puntos(" + ListaPuntos[contador].Tag.ToString() + ")");
                            Console.WriteLine("--Posicion Jugador: (" + x.Poscabx + "," + x.Poscaby + ")(" + pb.Width + "," + pb.Height + ")");
                            Console.WriteLine("--Posicion Objeto: (" + ListaPuntos[contador].Location.X + "," + ListaPuntos[contador].Location.Y + ")(" + ListaPuntos[contador].Width + "," + ListaPuntos[contador].Height + ")");
                            colision = true;
                            x.Puntos += 5;
                            ActualizarPuntos();
                            object[] arg1 = { ListaPuntos[contador] };
                            mapa.Invoke(new DelegadoBorrarSerpientes(BorrarSerpiente), arg1);
                            ListaPuntos.Remove(ListaPuntos[contador]);
                            x.Cola3y = x.Cola2y;
                            x.Cola3x = x.Cola2x;
                            x.Cola2y = x.Cola1y;
                            x.Cola2x = x.Cola1x;
                            x.Cola1y = x.Poscaby;
                            x.Cola1x = x.Poscabx;
                            x.Poscabx -= 26;
                        }
                    }
                    foreach (PictureBox y in ListaSerpientes)
                    {
                        if(!colision)
                        {
                            char co;
                            int n = 0;
                            string c = y.Tag.ToString().Replace("cola", "").Substring(0, 2);
                            if (c == "am") co = 'y';
                            else if (c == "az") co = 'b';
                            else if (c == "ro") co = 'r';
                            else co = 'g';
                            int i = 0;
                            foreach (Jugador z in listajugadores)
                            {
                                if (z.Color == co) n = i;
                                ++i;
                            }
                            //MessageBox.Show("XNueva: "+Convert.ToString(po.X)+", XSerpiente: " + Convert.ToString(ListaSerpientes[contador].Location.X));
                            if (y.Bounds.IntersectsWith(pb.Bounds)
                                && x.Color != co)
                            {
                                //Console.WriteLine(x.Nombre + " Colisiona con: " + y.Tag.ToString());
                                colision = true;
                                int j;
                                Console.WriteLine(x.Nombre + "Me han comido. Yo: " + x.Color + "-" + co + y.Tag.ToString());
                                if (y.Tag.ToString().Substring(0, 4) != "cola")
                                {
                                    j = Array.IndexOf(C_COLOR, listajugadores[n].Color);
                                    listajugadores[n].Direccion = C_DIRECCION[j];
                                    listajugadores[n].Poscabx = C_CABX[j];
                                    listajugadores[n].Poscaby = C_CABY[j];
                                    listajugadores[n].Cola1x = C_COL1X[j];
                                    listajugadores[n].Cola1y = C_COL1Y[j];
                                    listajugadores[n].Cola2x = C_COL2X[j];
                                    listajugadores[n].Cola2y = C_COL2Y[j];
                                    listajugadores[n].Cola3x = C_COL3X[j];
                                    listajugadores[n].Cola3y = C_COL3Y[j];
                                    listajugadores[n].Puntos = 0;
                                    ActualizarPuntos();

                                }
                                j = Array.IndexOf(C_COLOR, x.Color);
                                x.Direccion = C_DIRECCION[j];
                                x.Poscabx = C_CABX[j];
                                x.Poscaby = C_CABY[j];
                                x.Cola1x = C_COL1X[j];
                                x.Cola1y = C_COL1Y[j];
                                x.Cola2x = C_COL2X[j];
                                x.Cola2y = C_COL2Y[j];
                                x.Cola3x = C_COL3X[j];
                                x.Cola3y = C_COL3Y[j];
                                listajugadores[n].Puntos += x.Puntos;
                                x.Puntos = 0;
                                ActualizarPuntos();
                            }
                        }
                    }
                    if (!colision)
                    {
                        x.Cola3y = x.Cola2y;
                        x.Cola3x = x.Cola2x;
                        x.Cola2y = x.Cola1y;
                        x.Cola2x = x.Cola1x;
                        x.Cola1y = x.Poscaby;
                        x.Cola1x = x.Poscabx;
                        x.Poscabx -= 26;
                    }
                }
                if (x.Direccion == "u")
                {
                    Console.WriteLine(x.Nombre + "Me muevo arriba");
                    pb.Location = new Point(x.Poscabx, x.Poscaby - 26);
                    pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("rojo");
                    pb.BackColor = Color.Transparent;
                    pb.SizeMode = PictureBoxSizeMode.AutoSize;
                    pb.Tag = "nuevo";
                    bool colision = false;
                    foreach (PictureBox y in ListaMuros)
                    {
                        if (!colision && (pb.Bounds.IntersectsWith(y.Bounds) || 0 > x.Poscaby - 26))
                        {
                            Console.WriteLine(x.Nombre + "Me he chocado con un Muro o Fin mapa(" + y.Tag.ToString() + ")");
                            Console.WriteLine("--Posicion Jugador: (" + x.Poscabx + "," + x.Poscaby + ")(" + pb.Width + "," + pb.Height + ")");
                            Console.WriteLine("--Posicion Objeto: (" + y.Location.X + "," + y.Location.Y + ")(" + y.Width + "," + y.Height + ")");
                            colision = true;
                        }
                    }
                    for (int contador = 0; contador < ListaPuntos.Count && !colision; contador++)
                    {
                        if (ListaPuntos[contador].Bounds.IntersectsWith(pb.Bounds))
                        {
                            Console.WriteLine(x.Nombre + "Consigo puntos(" + ListaPuntos[contador].Tag.ToString() + ")");
                            Console.WriteLine("--Posicion Jugador: (" + x.Poscabx + "," + x.Poscaby + ")(" + pb.Width + "," + pb.Height + ")");
                            Console.WriteLine("--Posicion Objeto: (" + ListaPuntos[contador].Location.X + "," + ListaPuntos[contador].Location.Y + ")(" + ListaPuntos[contador].Width + "," + ListaPuntos[contador].Height + ")");
                            colision = true;
                            x.Puntos += 5;
                            ActualizarPuntos();
                            object[] arg1 = { ListaPuntos[contador] };
                            mapa.Invoke(new DelegadoBorrarSerpientes(BorrarSerpiente), arg1);
                            ListaPuntos.Remove(ListaPuntos[contador]);
                            x.Cola3y = x.Cola2y;
                            x.Cola3x = x.Cola2x;
                            x.Cola2y = x.Cola1y;
                            x.Cola2x = x.Cola1x;
                            x.Cola1y = x.Poscaby;
                            x.Cola1x = x.Poscabx;
                            x.Poscaby -= 26;
                        }
                    }
                    foreach (PictureBox y in ListaSerpientes)
                    {
                        if (!colision)
                        {
                            char co;
                            int n = 0;
                            string c = y.Tag.ToString().Replace("cola", "").Substring(0, 2);
                            if (c == "am") co = 'y';
                            else if (c == "az") co = 'b';
                            else if (c == "ro") co = 'r';
                            else co = 'g';
                            int i = 0;
                            foreach (Jugador z in listajugadores)
                            {
                                if (z.Color == co) n = i;
                                ++i;
                            }
                            //MessageBox.Show("XNueva: "+Convert.ToString(po.X)+", XSerpiente: " + Convert.ToString(ListaSerpientes[contador].Location.X));
                            if (y.Bounds.IntersectsWith(pb.Bounds)
                                && x.Color != co)
                            {
                                //Console.WriteLine(x.Nombre + " Colisiona con: " + y.Tag.ToString());
                                Console.WriteLine(x.Nombre + "Me han comido. Yo: " + x.Color + "-" + co + y.Tag.ToString());
                                colision = true;
                                int j;
                                if (y.Tag.ToString().Substring(0, 4) != "cola")
                                {
                                    j = Array.IndexOf(C_COLOR, listajugadores[n].Color);
                                    listajugadores[n].Direccion = C_DIRECCION[j];
                                    listajugadores[n].Poscabx = C_CABX[j];
                                    listajugadores[n].Poscaby = C_CABY[j];
                                    listajugadores[n].Cola1x = C_COL1X[j];
                                    listajugadores[n].Cola1y = C_COL1Y[j];
                                    listajugadores[n].Cola2x = C_COL2X[j];
                                    listajugadores[n].Cola2y = C_COL2Y[j];
                                    listajugadores[n].Cola3x = C_COL3X[j];
                                    listajugadores[n].Cola3y = C_COL3Y[j];
                                    listajugadores[n].Puntos = 0;
                                    ActualizarPuntos();

                                }
                                j = Array.IndexOf(C_COLOR, x.Color);
                                x.Direccion = C_DIRECCION[j];
                                x.Poscabx = C_CABX[j];
                                x.Poscaby = C_CABY[j];
                                x.Cola1x = C_COL1X[j];
                                x.Cola1y = C_COL1Y[j];
                                x.Cola2x = C_COL2X[j];
                                x.Cola2y = C_COL2Y[j];
                                x.Cola3x = C_COL3X[j];
                                x.Cola3y = C_COL3Y[j];
                                listajugadores[n].Puntos += x.Puntos;
                                x.Puntos = 0;
                                ActualizarPuntos();
                            }
                        }
                    }
                    if (!colision)
                    {
                        x.Cola3y = x.Cola2y;
                        x.Cola3x = x.Cola2x;
                        x.Cola2y = x.Cola1y;
                        x.Cola2x = x.Cola1x;
                        x.Cola1y = x.Poscaby;
                        x.Cola1x = x.Poscabx;
                        x.Poscaby -= 26;
                    }
                }
                if (x.Direccion == "d")
                {
                    Console.WriteLine(x.Nombre + "Me muevo abajo");
                    pb.Location = new Point(x.Poscabx, x.Poscaby + 26);
                    pb.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("rojo");
                    pb.BackColor = Color.Transparent;
                    pb.SizeMode = PictureBoxSizeMode.AutoSize;
                    pb.Tag = "nuevo";
                    pb.Location = new Point(x.Poscabx, x.Poscaby + 26);
                    bool colision = false;

                    foreach (PictureBox y in ListaMuros)
                    {
                        if (!colision && (pb.Bounds.IntersectsWith(y.Bounds) || x.Poscaby + 26 > 546))
                        {
                            Console.WriteLine(x.Nombre + "Me he chocado con un Muro o Fin mapa(" + y.Tag.ToString() + ")"); ;
                            Console.WriteLine("--Posicion Jugador: (" + x.Poscabx + "," + x.Poscaby + ")(" + pb.Width + "," + pb.Height + ")");
                            Console.WriteLine("--Posicion Objeto: (" + y.Location.X + "," + y.Location.Y + ")(" + y.Width + "," + y.Height + ")");
                            colision = true;
                        }
                    }
                    for (int contador = 0; contador < ListaPuntos.Count && !colision; contador++)
                    {
                        if (ListaPuntos[contador].Bounds.IntersectsWith(pb.Bounds))
                        {
                            Console.WriteLine(x.Nombre + "Consigo puntos(" + ListaPuntos[contador].Tag.ToString() + ")");
                            Console.WriteLine("--Posicion Jugador: (" + x.Poscabx + "," + x.Poscaby + ")(" + pb.Width + "," + pb.Height + ")");
                            Console.WriteLine("--Posicion Objeto: (" + ListaPuntos[contador].Location.X + "," + ListaPuntos[contador].Location.Y + ")(" + ListaPuntos[contador].Width + "," + ListaPuntos[contador].Height + ")");
                            colision = true;
                            x.Puntos += 5;
                            ActualizarPuntos();
                            object[] arg1 = { ListaPuntos[contador] };
                            mapa.Invoke(new DelegadoBorrarSerpientes(BorrarSerpiente), arg1);
                            ListaPuntos.Remove(ListaPuntos[contador]);
                            x.Cola3y = x.Cola2y;
                            x.Cola3x = x.Cola2x;
                            x.Cola2y = x.Cola1y;
                            x.Cola2x = x.Cola1x;
                            x.Cola1y = x.Poscaby;
                            x.Cola1x = x.Poscabx;
                            x.Poscaby += 26;
                        }
                    }
                    foreach (PictureBox y in ListaSerpientes)
                    {
                        if (!colision)
                        {
                            char co;
                            int n = 0;
                            string c = y.Tag.ToString().Replace("cola", "").Substring(0, 2);
                            if (c == "am") co = 'y';
                            else if (c == "az") co = 'b';
                            else if (c == "ro") co = 'r';
                            else co = 'g';
                            int i = 0;
                            foreach (Jugador z in listajugadores)
                            {
                                if (z.Color == co) n = i;
                                ++i;
                            }
                            //MessageBox.Show("XNueva: "+Convert.ToString(po.X)+", XSerpiente: " + Convert.ToString(ListaSerpientes[contador].Location.X));
                            if (y.Bounds.IntersectsWith(pb.Bounds)
                                && x.Color != co)
                            {
                                //Console.WriteLine(x.Nombre + " Colisiona con: " + y.Tag.ToString());
                                Console.WriteLine(x.Nombre + "Me han comido. Yo: " + x.Color + "-" + co + y.Tag.ToString());
                                colision = true;
                                int j;
                                if (y.Tag.ToString().Substring(0, 4) != "cola")
                                {
                                    j = Array.IndexOf(C_COLOR, listajugadores[n].Color);
                                    listajugadores[n].Direccion = C_DIRECCION[j];
                                    listajugadores[n].Poscabx = C_CABX[j];
                                    listajugadores[n].Poscaby = C_CABY[j];
                                    listajugadores[n].Cola1x = C_COL1X[j];
                                    listajugadores[n].Cola1y = C_COL1Y[j];
                                    listajugadores[n].Cola2x = C_COL2X[j];
                                    listajugadores[n].Cola2y = C_COL2Y[j];
                                    listajugadores[n].Cola3x = C_COL3X[j];
                                    listajugadores[n].Cola3y = C_COL3Y[j];
                                    listajugadores[n].Puntos = 0;
                                    ActualizarPuntos();

                                }
                                j = Array.IndexOf(C_COLOR, x.Color);
                                x.Direccion = C_DIRECCION[j];
                                x.Poscabx = C_CABX[j];
                                x.Poscaby = C_CABY[j];
                                x.Cola1x = C_COL1X[j];
                                x.Cola1y = C_COL1Y[j];
                                x.Cola2x = C_COL2X[j];
                                x.Cola2y = C_COL2Y[j];
                                x.Cola3x = C_COL3X[j];
                                x.Cola3y = C_COL3Y[j];
                                listajugadores[n].Puntos += x.Puntos;
                                x.Puntos = 0;
                                ActualizarPuntos();
                            }
                        }
                    }
                    if (!colision)
                    {
                        x.Cola3y = x.Cola2y;
                        x.Cola3x = x.Cola2x;
                        x.Cola2y = x.Cola1y;
                        x.Cola2x = x.Cola1x;
                        x.Cola1y = x.Poscaby;
                        x.Cola1x = x.Poscabx;
                        x.Poscaby += 26;
                    }
                }
            }
            PintarSerpientes(mapa, "borrar");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

    public class Jugador
    {
        private string nombre;
        private char color;
        private string direccion;
        private int poscabx;
        private int poscaby;
        private int cola1x;
        private int cola1y;
        private int cola2x;
        private int cola2y;
        private int cola3x;
        private int cola3y;
        private int puntos;

        public string Nombre { get => nombre; set => nombre = value; }
        public char Color { get => color; set => color = value; }
        public string Direccion { get => direccion; set => direccion = value; }
        public int Poscabx { get => poscabx; set => poscabx = value; }
        public int Poscaby { get => poscaby; set => poscaby = value; }
        public int Puntos { get => puntos; set => puntos = value; }
        public int Cola1x { get => cola1x; set => cola1x = value; }
        public int Cola1y { get => cola1y; set => cola1y = value; }
        public int Cola2x { get => cola2x; set => cola2x = value; }
        public int Cola2y { get => cola2y; set => cola2y = value; }
        public int Cola3x { get => cola3x; set => cola3x = value; }
        public int Cola3y { get => cola3y; set => cola3y = value; }
    }
}
