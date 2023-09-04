#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
//#include <mysql.h>
//#include <mysql.h>
#include <pthread.h>
//Estructura necesaria para acceso excluyente
pthread_mutex_t mutex =PTHREAD_MUTEX_INITIALIZER;

//Estructura de datos para almacenar 100 conectados
typedef struct{
	char nombre[20];
	int socket;
	
}Conectado;

typedef struct{
	Conectado conectados[100];
	int num;
}ListaConectados;

//Tabla para almacenar los jugadores de las partidas
typedef struct{
	char nombre1[20];
	int socket1;
	int aceptado1;
	int pos1x;
	int pos1y;
	int puntos1;
	char dir1[20];
	char color1[1];
	int listo1;
	
	char nombre2[20];
	int socket2;
	int aceptado2;
	int pos2x;
	int pos2y;
	int puntos2;
	char dir2[20];
	char color2[1];
	int listo2;
	
	char nombre3[20];
	int socket3;
	int aceptado3;
	int pos3x;
	int pos3y;
	int puntos3;
	char dir3[20];
	char color3[1];
	int listo3;
	
	char nombre4[20];
	int socket4;
	int aceptado4;
	int pos4x;
	int pos4xy;
	int puntos4;
	char dir4[20];
	char color4[1];
	int listo4;
	
}Partida;

typedef Partida TablaPartidas[100];

TablaPartidas tabla;

//Tabla para almacenar los datos del mapa
typedef struct 
{
	char puntosx[5];
	char puntosy[5];
	char paredx[20];
	char paredy[20];
	
}Mapa;

Mapa mapa[3];

void rellenarmapa()
{
	
}

int NumPartida(TablaPartidas tabla) //Devuelve la primera fila libre de partidas
{
	int result = strlen(tabla);
	return result;
}
int QueJugador(TablaPartidas tabla, int id, char nombre[20]) //Devuelve la posicion,1,2,3,4, del jugador introducido en la partida id
{
	printf("Quejugador nombre: %s\n", nombre);
	int result;
	printf("Quejugador nombre1: %s\n", tabla[id].nombre1);
	printf("Quejugador nombre2: %s\n", tabla[id].nombre2);
	printf("Quejugador nombre3: %s\n", tabla[id].nombre3);
	printf("Quejugador nombre4: %s\n", tabla[id].nombre4);
	if (strcmp(tabla[id].nombre1, nombre)==0)
	{
		result = 1;
	}
	else if (strcmp(tabla[id].nombre2, nombre)==0)
	{
		result = 2;
	}
	else if (strcmp(tabla[id].nombre3, nombre)==0)
	{
		result = 3;
	}
	else if (strcmp(tabla[id].nombre4, nombre)==0)
	{
		result = 4;
	}
	printf("Quejugador result: %d\n", result);	
	return result;	
}
void ActualizarAceptado(TablaPartidas tabla,int id, char nombre[20], int aceptado ) //Acutualiza el campo aceptado de un jugador
{
	int quejugador = QueJugador(tabla,id,nombre);
	
	if (aceptado==1){
		
		if(quejugador==2){
			tabla[id].aceptado2 = 1;
		}
		else if (quejugador==3){
			tabla[id].aceptado3 = 1;
		}
		else if (quejugador==4){
			tabla[id].aceptado4 = 1;
		}
	}
	else if (aceptado==2){
		
		if(quejugador==2){
			tabla[id].aceptado2 = 2;
		}
		else if (quejugador==3){
			tabla[id].aceptado3 = 2;
		}
		else if (quejugador==4){
			tabla[id].aceptado4 = 2;
		}
	}
	
}
void CambiarDireccion(char nombre[20], int id, char direccion[20])	//Modifica la dirección del jugador, esta puede ser derecha, izquierda, arriba o abajo
{
	printf("%s\n", direccion);
	char notificacion[200];
	if(strcmp(tabla[id].nombre1, nombre)==0)
	{
		strcpy (tabla[id].dir1, direccion);
		
		sprintf(notificacion, "10/%s/%s",nombre, tabla[id].dir1);
		printf("%s\n", notificacion);
		if(tabla[id].listo2 == 1) write (tabla[id].socket2, notificacion, strlen(notificacion));
		if(tabla[id].listo3 == 1) write (tabla[id].socket3, notificacion, strlen(notificacion));
		if(tabla[id].listo4 == 1) write (tabla[id].socket4, notificacion, strlen(notificacion));
		write (tabla[id].socket1, notificacion, strlen(notificacion));
	}
	else if(strcmp(tabla[id].nombre2, nombre)==0)
	{
		strcpy (tabla[id].dir2, direccion);
		printf("hola\n");
		printf("%s\n",tabla[id].dir2);
		sprintf(notificacion, "10/%s/%s",nombre, tabla[id].dir2);
		printf("%s\n", notificacion);
		
		if(tabla[id].listo1 == 1) write (tabla[id].socket1, notificacion, strlen(notificacion));
		if(tabla[id].listo3 == 1) write (tabla[id].socket3, notificacion, strlen(notificacion));
		if(tabla[id].listo4 == 1) write (tabla[id].socket4, notificacion, strlen(notificacion));
		write (tabla[id].socket2, notificacion, strlen(notificacion));
	}
	else if(strcmp(tabla[id].nombre3, nombre)==0)
	{
		strcpy (tabla[id].dir3, direccion);
		sprintf(notificacion, "10/%s/%s",nombre, tabla[id].dir3);
		printf("%s\n", notificacion);
		if(tabla[id].listo1 == 1) write (tabla[id].socket1, notificacion, strlen(notificacion));
		if(tabla[id].listo2 == 1) write (tabla[id].socket2, notificacion, strlen(notificacion));
		if(tabla[id].listo4 == 1) write (tabla[id].socket4, notificacion, strlen(notificacion));
		write (tabla[id].socket3, notificacion, strlen(notificacion));
	}
	else
	{
		strcpy (tabla[id].dir4, direccion);
		sprintf(notificacion, "10/%s/%s",nombre, tabla[id].dir4);
		printf("%s\n", notificacion);
		if(tabla[id].listo1 == 1) write (tabla[id].socket1, notificacion, strlen(notificacion));
		if(tabla[id].listo2 == 1) write (tabla[id].socket2, notificacion, strlen(notificacion));
		if(tabla[id].listo3 == 1) write (tabla[id].socket3, notificacion, strlen(notificacion));
		write (tabla[id].socket4, notificacion, strlen(notificacion));
	}
}

void ActualizarListo(TablaPartidas tabla,int id, char nombre[20], int listo ) //Acutualiza el campo aceptado de un jugador
{
	printf("nombre: %s, id: %d, listo: %d\n", nombre, id, listo);
	int quejugador = QueJugador(tabla,id,nombre);
	printf("jugador: %d\n", quejugador);
	if(quejugador==1)
	{
		tabla[id].listo1 = 1;
	}	
	else if(quejugador==2)
	{
		tabla[id].listo2 = 1;
	}
	else if (quejugador==3)
	{
		tabla[id].listo3 = 1;
	}
	else if (quejugador==4)
	{
		tabla[id].listo4 = 1;
	}
}

//aceptado=1 acepta solicitud de juego 
//aceptado=2 no acepta solicitud de juego
//aceptado=0 neutro
void Inicializar (TablaPartidas tabla) //Inicializa el campo aceptado y socket a 0 de todos los jugadores
{
	int i;
	int a = NumPartida(tabla); 
	
	for (i=0;i<a;i++){
		tabla[i].aceptado1=0;
		tabla[i].aceptado2=0;
		tabla[i].aceptado3=0;
		tabla[i].aceptado4=0;
		
		tabla[i].socket1=0;
		tabla[i].socket2=0;
		tabla[i].socket3=0;
		tabla[i].socket4=0;
		
		tabla[i].listo1=0;
		tabla[i].listo2=0;
		tabla[i].listo3=0;
		tabla[i].listo4=0;
	}
}

void ListadoRivales (TablaPartidas tabla, char nombre[20], char resultado[200])	//Lista los jugadores, separados por comas, que han jugado con el usuario con el nombre introducido
{
	int a = NumPartida(tabla); 
	strcpy(resultado, "1/");
	for (int i=0;i<a;i++){
		if(strcmp(tabla[i].nombre1, nombre)==0)
		{
			if(tabla[i].socket3==0)
			{
				sprintf(resultado, "%s%s, ", resultado, tabla[i].nombre2);
			}
			else if(tabla[i].socket4==0)
			{
				sprintf(resultado, "%s%s, %s, ", resultado, tabla[i].nombre2, tabla[i].nombre3);
			}
			else
				sprintf(resultado, "%s%s, %s, %s, ", resultado, tabla[i].nombre2, tabla[i].nombre3, tabla[i].nombre4);
		}
		else if(strcmp(tabla[i].nombre2, nombre)==0)
		{
			if(tabla[i].socket3==0)
			{
				sprintf(resultado, "%s%s, ", resultado, tabla[i].nombre1);
			}
			else if(tabla[i].socket4==0)
			{
				sprintf(resultado, "%s%s, %s, ", resultado, tabla[i].nombre1, tabla[i].nombre3);
			}
			else
				sprintf(resultado, "%s%s, %s, %s, ", resultado, tabla[i].nombre1, tabla[i].nombre3, tabla[i].nombre4);
		}
		else if(strcmp(tabla[i].nombre3, nombre)==0)
		{
			if(tabla[i].socket4==0)
			{
				sprintf(resultado, "%s%s, %s, ", resultado, tabla[i].nombre1, tabla[i].nombre2);
			}
			else
				sprintf(resultado, "%s%s, %s, %s, ", resultado, tabla[i].nombre1, tabla[i].nombre2, tabla[i].nombre4);
		}
		else if(strcmp(tabla[i].nombre4, nombre)==0)
		{
			sprintf(resultado, "%s%s, %s, %s, ", resultado, tabla[i].nombre1, tabla[i].nombre2, tabla[i].nombre3);
		}
	}
	
	if (strcmp(resultado,"1/")!=0){
		resultado[strlen(resultado)-2]="/0";
	}
	
}

int PartidasJugadas(TablaPartidas tabla, char nombre[20])	//Devuelve el número de partidas jugadas por ese jugador
{
	int res=0;
	int a = NumPartida(tabla); 
	for (int i=0;i<a;i++){
		if(strcmp(tabla[i].nombre1, nombre)==0)
		{
			res++;
		}
		else if(strcmp(tabla[i].nombre2, nombre)==0)
		{
			res++;
		}
		else if(strcmp(tabla[i].nombre3, nombre)==0)
		{
			res++;
		}
		else if(strcmp(tabla[i].nombre4, nombre)==0)
		{
			res++;
		}
	}
	return res;
}

ListaConectados lista;
char conectados[300];

int i;
int sockets[100];

int AnadirConectado(char nombre[20], int socket, ListaConectados *lista) //Añade jugador conectado a ListaConectados, devuelve -1 si está llena
{
	
	if (lista->num==100)
		return -1;
	else{
		strcpy(lista->conectados[lista->num].nombre,nombre);
		lista->conectados[lista->num].socket = socket;
		lista->num++;
		return 0;
	}
	
}

int JugadoresPartida(TablaPartidas tabla, int id)	//Número de jugadores que aceptan la partida
{
	return tabla[id].aceptado1 + tabla[id].aceptado2 + tabla[id].aceptado3 + tabla[id].aceptado4;
}

int DameSocket(ListaConectados *lista, char nombre[20]) //Devuelve el socket de el jugador pasado como parámetro y -1 si no existe
{
	int j = 0;
	int encontrado = 0;
	while (encontrado == 0 && lista->num != j){
		if(strcmp(lista->conectados[j].nombre,nombre) == 0)
			encontrado = 1;
		else
			j = j + 1;
	}
	if (encontrado == 0)
		return -1;
	else
		return lista->conectados[j].socket;
}

int DamePosicion(char nombre[20],ListaConectados *lista) //Devuelve el id del jugador, -1 en caso de que no exista
{
	
	int j = 0;
	int encontrado = 0;
	while ((encontrado == 0) && (lista->num != j)){
		if(strcmp(lista->conectados[j].nombre,nombre) == 0)
			encontrado = 1;
		else
			j = j + 1;
	}
	if (encontrado == 0)
		return -1;
	else
		return j;
}

int EliminarConectado(char nombre[20],ListaConectados *lista) //Elimina un jugador de la lista, -1 si este no existe
{
	int p = DamePosicion(nombre,lista);
	
	if(p == -1)
		return -1;
	else{
		int i;
		for(i = p; i < lista -> num; i++){
			lista->conectados[i].socket = lista->conectados[i+1].socket;
			strcpy(lista->conectados[i].nombre,lista->conectados[i+1].nombre);
			lista->num = lista->num -1;
		}
		return 0;
		
	}
}

void DameConectados(ListaConectados *lista, char result[200]) //Devuelve en result el número de jugadores/la lista de personas conectadas separadas por / 
{
	sprintf(result,"%d/",lista->num);
	int i;
	for(i = 0; i < lista->num; i++){
		sprintf(result,"%s%s/",result,lista->conectados[i].nombre);
	}
}


void consultarBBDD(MYSQL *conn, int codigo, char respuestaconsulta[512],char nombre[20], char contrasena[50]) //Realiza las consultas a la base de datos de las peticiones
{
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int err;
	char consulta[200]; 
	char consulta1[200]; 
	char consulta2[200]; 
	if(codigo == 3)//tiempo de la partida de Juan
	{
		strcpy (consulta,"SELECT Partidas.tiempo FROM Partidas, Participaciones, Jugadores WHERE Partidas.id = Participaciones.idP AND Participaciones.idj = Jugadores.id AND Jugadores.nombre = 'Juan'");
	}
	else if(codigo == 4||codigo == 10)//Registro del ususario y darse de baja
	{
		sprintf (consulta, "SELECT Jugadores.nombre FROM Jugadores WHERE Jugadores.nombre = '%s'",nombre);
		sprintf (consulta1, "SELECT Jugadores.id FROM Jugadores WHERE Jugadores.id = (SELECT MAX(Jugadores.id) FROM Jugadores)");
	}
	else if (codigo == 5){ //Iniciar sesión
		sprintf (consulta, "SELECT Jugadores.nombre FROM Jugadores WHERE Jugadores.nombre = '%s' AND Jugadores.contrasena = '%s'",nombre, contrasena);
	}
	
	err=mysql_query(conn,consulta);
	
	if (err!=0) {
		printf ("%d/Error al consultar datos de la base %u %s\n", codigo,
				mysql_errno(conn), mysql_error(conn));
		sprintf(respuestaconsulta,"%d/Error", codigo);
		
		exit (1);
	}
	
	else if (codigo!=4&&codigo!=10){
		resultado = mysql_store_result (conn);
		row = mysql_fetch_row (resultado);
		if (row == NULL){
			sprintf (respuestaconsulta,"%d/Error", codigo);
			
		}
		else
			sprintf(respuestaconsulta,"%d/%s",codigo,row[0]); 
	}
	
	if(codigo == 4)
	{
		resultado = mysql_store_result(conn);
		row = mysql_fetch_row (resultado);
		if (row == NULL)
		{
			printf ("4/El jugador se puede registrar\n");
			
			strcpy (respuestaconsulta, "4/El jugador se puede registrar");
			
			int id;
			
			err=mysql_query (conn, consulta1);
			if (err!=0) {
				printf ("4/Error al consultar datos de la base %u %s\n",
						mysql_errno(conn), mysql_error(conn));
				sprintf (respuestaconsulta,"%d/Error", codigo);
				exit (1);
			}
			
			resultado = mysql_store_result(conn);
			
			row = mysql_fetch_row (resultado);
			id = atoi(row[0]);
			id = id + 1;
			
			sprintf (consulta2,"INSERT INTO Jugadores VALUES (%d,'%s', '%s');", id, nombre, contrasena);
			err=mysql_query (conn, consulta2);
			if (err!=0) {
				printf ("4/Error al consultar datos de la base %u %s\n",
						mysql_errno(conn), mysql_error(conn));
				
				exit (1);
			}
			
		}
		else{
			printf ("4/El jugador ya estaba registrado\n");
			strcpy (respuestaconsulta, "4/El jugador ya estaba registrado");
			
		}
	}
	
	if(codigo == 10)
	{
		resultado = mysql_store_result(conn);
		row = mysql_fetch_row (resultado);
		if (row == NULL)
		{
			printf ("11/El usuario no existe\n");
			
			strcpy (respuestaconsulta, "11/El usuario no existe");
		}
		else{
			printf ("11/Baja realizada correctamente\n");
			strcpy (respuestaconsulta, "11/Baja realizada correctamente");
			
			int id;
			
			err=mysql_query (conn, consulta1);
			if (err!=0) {
				printf ("10/Error al consultar datos de la base %u %s\n",
						mysql_errno(conn), mysql_error(conn));
				sprintf (respuestaconsulta,"%d/Error", codigo);
				exit (1);
			}
			
			resultado = mysql_store_result(conn);
			
			row = mysql_fetch_row (resultado);
			id = atoi(row[0]);
			
			sprintf (consulta2,"DELETE FROM Jugadores WHERE id=%d;", id);
			err=mysql_query (conn, consulta2);
			if (err!=0) {
				printf ("10/Error al consultar datos de la base %u %s\n",
						mysql_errno(conn), mysql_error(conn));
				
				exit (1);
			}
		}
	}
}

void *AtenderUsuario (void *socket)
{
	int sock_conn;
	int *s;
	s = (int *) socket;
	sock_conn=*s;
	
	char peticion[512];
	char respuesta[512];
	char respuestaconsulta[512];
	
	int ret;
	
	//Realizamos conexion con mysql
	
	MYSQL *conn;
	int err;
	// Estructura especial para almacenar resultados de consultas 
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	char consulta[200];
	char consulta1[200];
	char consulta2[200];
	char consulta3 [200];
	char consulta4 [200];
	char error[50];
	int noregistrado=0;
	char notificacion[200];
	char ListaConcetados[200];
	
	//Creamos una conexion al servidor MYSQL 
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	// la conexion
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "LaserTails",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	int terminar = 0;
	while (terminar==0){
		// Ahora recibimos su peticion
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibida una petición\n");
		// Tenemos que a?adirle la marca de fin de string 
		// para que no escriba lo que hay despues en el buffer
		peticion[ret]='\0';
		
		//Escribimos la peticion en la consola
		
		printf ("La petición es: %s\n",peticion);
		char *p = strtok(peticion, "/");
		int codigo =  atoi (p);
		char nombre[20];
		char contrasena[50];
		char nombrepeticion[20];
		//codigos
		if (codigo !=0 && codigo !=6 && codigo !=7 && codigo !=8 && codigo !=9){
			p = strtok( NULL, "/");
			strcpy (nombre, p);
			p = strtok( NULL, "/");
			strcpy (contrasena, p);
			printf ("Codigo: %d, Nombre: %s, Contrasena: %s\n", codigo, nombre, contrasena);
		}
		if (codigo==0){ //Desconexión del servidor
			
			//Al modificar la lista de conectados necesitamos garantizar el acceso excluyente
			pthread_mutex_lock(&mutex);
			int res = EliminarConectado(nombre, &lista);
			pthread_mutex_unlock(&mutex);
			if (res==-1)
				printf("Error al eliminar\n");
			else
				printf("Eliminado correctamente\n");
			
			DameConectados(&lista, ListaConcetados);
			sprintf(notificacion, "5/Desconectado correctamente/%s",ListaConcetados);
			printf("Resultado:'%s'\n", notificacion);
			
			int j;
			for (j=0; j< lista.num; j++)
			{
				write(lista.conectados[j].socket,notificacion, strlen(notificacion));
				sockets[j]=lista.conectados[j].socket; //Actualizar sockets solución a clientes muertos
			}
			terminar=1;
		}
		else if (codigo ==1){	//Listado de jugadores con los que he echado alguna partida
			char listado[200];
			ListadoRivales (tabla, nombre, listado);
			if (strcmp(listado,"1/")==0)
			{
				strcpy(error, "1/No has tenido rivales");
				printf("%s", error);				
				write (sock_conn,error, strlen(error));
			}
			else {
				printf(listado);
				write (sock_conn,listado, strlen(listado));
			}
		}
		else if (codigo ==2){	//Número de partidas jugadas
			int res = PartidasJugadas(tabla, nombre);
			if (res==0)
			{
				strcpy(respuesta, "2/No has jugado ninguna partida");
				printf("%s", respuesta);				
				write (sock_conn,respuesta, strlen(respuesta));
			}
			else{
				sprintf(respuesta, "2/Hasta el momento has jugado %d partida/s", res);
				printf("%s", respuesta);				
				write (sock_conn,respuesta, strlen(respuesta));
			}
		}
		else if (codigo ==3){	//tiempo de la partida de Juan
			consultarBBDD(conn, codigo, respuestaconsulta, nombre, contrasena);
			if (strcmp(respuestaconsulta,"3/Error")==0)
			{
				strcpy(error, "3/Error");
				printf("%s", error);				
				write (sock_conn,error, strlen(error));
			}
			else
			{
				char *c = strtok(respuestaconsulta, "/");
				c = strtok( NULL, "/");
				printf ("El tiempo de la partida de Juan fue: %s min.\n", c);
				sprintf (respuesta,"3/El tiempo de la partida de Juan fue: %s min.\n", c);
				write (sock_conn,respuesta, strlen(respuesta));
			}
		}
		else if (codigo ==4){	//Registro del ususario
			consultarBBDD(conn, codigo, respuestaconsulta, nombre, contrasena);
			write (sock_conn,respuestaconsulta, strlen(respuestaconsulta));
			terminar=1;
		}
		else if (codigo ==5){	//Inicio de sesión y conexión al servidor
			consultarBBDD(conn, codigo, respuestaconsulta, nombre, contrasena);
			if (strcmp(respuestaconsulta,"5/Error")==0)
			{
				strcpy(respuesta, "5/Error");
				printf("%s", respuesta);				
				write (sock_conn,respuesta, strlen(respuesta));
			}
			else{ //Conectado correctamente
				pthread_mutex_lock(&mutex);
				int res = AnadirConectado(nombre, sock_conn, &lista);
				pthread_mutex_unlock(&mutex);
				if (res==-1) {
					printf("Lista llena\n");
					strcpy(respuesta,"5/Maximo de jugadores conectados.");
					write (sock_conn,respuesta, strlen(respuesta));
				} 
				else {
					printf("Añadido bien\n");
					int socket= DameSocket(&lista, nombre);
					printf("El socket de '%s' es: %d\n", nombre, socket);
					
					DameConectados(&lista, ListaConcetados);
					sprintf(notificacion, "5/Conectado correctamente/%s",ListaConcetados);
					printf("Resultado:'%s'\n", notificacion);
					
					int j;
					for (j=0; j< lista.num; j++)
						write(lista.conectados[j].socket,notificacion, strlen(notificacion));
				}
				
			}
		}
		else if (codigo==6){	//Partida creada por el anfitrión
			
			p = strtok( NULL, "/");
			strcpy (nombre, p);
			printf("Resultado:'%s'\n", nombre);
			
			int numpartida = NumPartida(tabla);
			
			strcpy(tabla[numpartida].nombre1, nombre);
			tabla[numpartida].socket1 = sock_conn;
			tabla[numpartida].aceptado1 = 1;
			
			sprintf (notificacion, "6/%s/%d",nombre,numpartida);
			
			p = strtok( NULL, "/");
			strcpy (nombrepeticion, p);
			
			if (nombrepeticion != NULL){
				
				strcpy(tabla[numpartida].nombre2, nombrepeticion);
				
				for(int p=0; p<lista.num; p++){
					
					if (strcmp(lista.conectados[p].nombre , nombrepeticion)==0){
						tabla[numpartida].socket2 = lista.conectados[p].socket;
						tabla[numpartida].aceptado2 = 2;
					}
				}
				// Enviamos la respuesta
				write (tabla[numpartida].socket2,notificacion, strlen(notificacion));
				p = strtok( NULL, "/");
			}
			
			if (p != NULL){
				strcpy (nombrepeticion, p);
				
				strcpy(tabla[numpartida].nombre3, nombrepeticion);
				
				for(int p=0; p<lista.num; p++){
					
					if (strcmp(lista.conectados[p].nombre , nombrepeticion)==0){
						tabla[numpartida].socket3 = lista.conectados[p].socket;
						tabla[numpartida].aceptado3 = 2;
					}
				}
				// Enviamos la respuesta
				write (tabla[numpartida].socket3,notificacion, strlen(notificacion));
				p = strtok( NULL, "/");
			}
			
			if (p != NULL){
				strcpy (nombrepeticion, p);
				printf("Resultado:'%s'\n", nombrepeticion);
				strcpy(tabla[numpartida].nombre4, nombrepeticion);
				
				for(int p=0; p<lista.num; p++){
					
					if (strcmp(lista.conectados[p].nombre , nombrepeticion)==0){
						tabla[numpartida].socket4 = lista.conectados[p].socket;
						tabla[numpartida].aceptado4 = 2;
					}
				}
				// Enviamos la respuesta
				write (tabla[numpartida].socket4,notificacion, strlen(notificacion));
			}		
		}
		else if (codigo==7){	//Respuesta de jugador y notificación a anfitrión
			p = strtok( NULL, "/");
			strcpy (nombre, p);
			printf("Resultado:'%s'\n", nombre);
			p = strtok( NULL, "/");
			int id = atoi(p);
			p = strtok( NULL, "/");
			int aceptado = atoi(p);
			
			ActualizarAceptado(tabla, id, nombre, aceptado);
			
			sprintf (notificacion, "7/%s/%d",nombre,aceptado);
			// Enviamos la respuesta
			write (tabla[id].socket1,notificacion, strlen(notificacion));
			
			if (aceptado == 2) {
				printf("Aceptado2: %i-%i-%i-%i\n", tabla[id].aceptado1, tabla[id].aceptado2, tabla[id].aceptado3, tabla[id].aceptado4);
				sprintf (notificacion, "8/0/0");
				
				if (tabla[id].aceptado1 == 1 || tabla[id].aceptado1 == 2) {
					write (tabla[id].socket1,notificacion, strlen(notificacion));
				}
				if (tabla[id].aceptado2 == 1 || tabla[id].aceptado2 == 2) {
					write (tabla[id].socket2,notificacion, strlen(notificacion));
				}
				if (tabla[id].aceptado3 == 1 || tabla[id].aceptado3 == 2) {
					write (tabla[id].socket3,notificacion, strlen(notificacion));
				}
				if (tabla[id].aceptado4 == 1 || tabla[id].aceptado4 == 2) {
					write (tabla[id].socket4,notificacion, strlen(notificacion));
				}
			} else {
				if ((tabla[id].aceptado1 == 1 || tabla[id].aceptado1 == NULL) &&
					(tabla[id].aceptado2 == 1 || tabla[id].aceptado2 == NULL) &&
					(tabla[id].aceptado3 == 1 || tabla[id].aceptado3 == NULL) &&
					(tabla[id].aceptado4 == 1 || tabla[id].aceptado4 == NULL)) {
						printf("Aceptado1: %i-%i-%i-%i\n", tabla[id].aceptado1, tabla[id].aceptado2, tabla[id].aceptado3, tabla[id].aceptado4);
						srand(time(NULL));
						int n_mapa = rand() % 3 + 1;
						sprintf (notificacion, "8/1/%d", n_mapa);
						if (tabla[id].aceptado1 == 1)
							write (tabla[id].socket1,notificacion, strlen(notificacion));
						if (tabla[id].aceptado2 == 1)
							write (tabla[id].socket2,notificacion, strlen(notificacion));
						if (tabla[id].aceptado3 == 1)
							write (tabla[id].socket3,notificacion, strlen(notificacion));
						if (tabla[id].aceptado4 == 1)
							write (tabla[id].socket4,notificacion, strlen(notificacion));
				}
			}
		}
		else if (codigo==8){	//Respuesta de jugador y notificación a anfitrión
			p = strtok( NULL, "/");
			strcpy (nombre, p);
			printf("Resultado:'%s'\n", nombre);
			p = strtok( NULL, "/");
			int id = atoi(p);
			printf("estoy\n");
			ActualizarListo(tabla, id, nombre, 1);
			
			/*sprintf (notificacion, "8/%s/%d",nombre,listo);
			// Enviamos la respuesta
			write (tabla[id].socket1,notificacion, strlen(notificacion));*/
			
			if ((tabla[id].listo1 == tabla[id].aceptado1) &&
				(tabla[id].listo2 == tabla[id].aceptado2) &&
				(tabla[id].listo3 == tabla[id].aceptado3) &&
				(tabla[id].listo4 == tabla[id].aceptado4)) {
				
					printf("Listo1: %d, Listo2: %d, Aceptado1: %d, Aceptado2: %d\n", tabla[id].listo1, tabla[id].listo2, tabla[id].aceptado1, tabla[id].aceptado2);
					sprintf (notificacion, "9/1/%d", JugadoresPartida(tabla, id));
					
					if (tabla[id].aceptado1 == 1)
						sprintf (notificacion, "%s/%s", notificacion, tabla[id].nombre1);
					if (tabla[id].aceptado2 == 1)
						sprintf (notificacion, "%s/%s", notificacion, tabla[id].nombre2);
					if (tabla[id].aceptado3 == 1)
						sprintf (notificacion, "%s/%s", notificacion, tabla[id].nombre3);
					if (tabla[id].aceptado4 == 1)
						sprintf (notificacion, "%s/%s", notificacion, tabla[id].nombre4);
					
					if (tabla[id].listo1 == 1)
						write (tabla[id].socket1,notificacion, strlen(notificacion));
					if (tabla[id].listo2 == 1)
						write (tabla[id].socket2,notificacion, strlen(notificacion));
					if (tabla[id].listo3 == 1)
						write (tabla[id].socket3,notificacion, strlen(notificacion));
					if (tabla[id].listo4 == 1)
						write (tabla[id].socket4,notificacion, strlen(notificacion));
				}
			else
			{
				printf("No listo\n");
				sprintf (notificacion, "9/0/0");
				
				if (tabla[id].listo1 == 1 || tabla[id].listo1 == NULL) {
					write (tabla[id].socket1,notificacion, strlen(notificacion));
				}
				if (tabla[id].listo2 == 1 || tabla[id].listo2 == NULL) {
					write (tabla[id].socket2,notificacion, strlen(notificacion));
				}
				if (tabla[id].listo3 == 1 || tabla[id].listo3 == NULL) {
					write (tabla[id].socket3,notificacion, strlen(notificacion));
				}
				if (tabla[id].listo4 == 1 || tabla[id].listo4 == NULL) {
					write (tabla[id].socket4,notificacion, strlen(notificacion));
				}
			}
		}
		else if (codigo==9){	//Jugador modifica su dirección
			char direccion[20];
			p = strtok( NULL, "/");
			strcpy (nombre, p);
			printf("Resultado:'%s'\n", nombre);
			p = strtok( NULL, "/");
			int id = atoi(p);
			p = strtok( NULL, "/");
			strcpy (direccion, p);
			CambiarDireccion(nombre, id, direccion);
		}
		else if (codigo ==10){	//Darse de baja
			consultarBBDD(conn, codigo, respuestaconsulta, nombre, contrasena);
			write (sock_conn,respuestaconsulta, strlen(respuestaconsulta));
			terminar=1;
		}
		else{
			strcpy (error, "Selecciona una opcion");
			// Enviamos la respuesta
			write (sock_conn,error, strlen(error));
		}
	}
	// Se acabo el servicio para este cliente
	close(sock_conn);
}
int main(int argc, char *argv[])
{
	
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	char peticion[512];
	char respuesta[512];
	
	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	// Fem el bind al port
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	// asocia el socket a cualquiera de las IP de la m?quina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// escucharemos en el port 9050
	serv_adr.sin_port = htons(9000);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	//La cola de peticiones pendientes no podr? ser superior a 4
	if (listen(sock_listen, 4) < 0)
		printf("Error en el Listen");
	
	pthread_t thread;
	i=0;
	for(;;)
	{
		printf ("Escuchando\n");
					   
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexi?n\n");
		printf("%i",sock_conn);
		sockets[i] = sock_conn;
		//sock_conn es el socket que usaremos para este cliente
					   
		//Bucle atención al cliente
		pthread_create (&thread, NULL, AtenderUsuario, &sockets[i]);
		i++;
	}
}
