using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace WarshipsFormClient
{	
	// структура для размещения/удаления кораблей при клике мышью
	enum ShipManagement
	{
		DELETE,
		PLACE
	}
	// структура, определяющая этапы игрового процесса (не все используются как надо)
	enum GameStage
	{
		TACTICS,
		WAITCONNECT,
		WAITTURN,
		TURN
	}


	public partial class Form1 : Form
	{
		private readonly int CellSize; // размер ячеек поля
		private GameStage GameStage; 

		private Field PlayerField; // поле игрока, используем для отрисовки тактики и непосредственно для игры
		private Field EnemyField; // поле вражины, появляется только после тактики
		private int SelectedShipSize; // выбранный корабль
		private int CurrentShipDirection; // выбранное направление размещения корабля
		private int[] RemainedForPlaceShips; // хранит по размеру, сколько ещё кораблей надо поставить
		private Socket server; // сокет соединения с сервером
		
		
		public Form1()
		{
			CellSize = 25;
			CurrentShipDirection = 0;

			GameStage = GameStage.TACTICS;
			SelectedShipSize = 0;
			RemainedForPlaceShips = new int[4] { 4, 3, 2, 1 };
			PlayerField = new Field();
			PlayerField.x = 30;
			PlayerField.y = 30;
			EnemyField = new Field();
			EnemyField = new Field();
			EnemyField.x = 340;
			EnemyField.y = 30;
			InitializeComponent();
		}

		// проверяем, находятся ли координаты клика в нужной нам области
		private Boolean isClickInCorrectArea(Field area, MouseEventArgs mouse)
		{	
			return mouse.X >= area.x && mouse.X <= area.x + 10 * CellSize
				&& mouse.Y >= area.y && mouse.Y <= area.y + 10 * CellSize;
		}

		// получаем координаты клика для нужного нам поля
		private Coordinates GetClickCellCoordinates(Field field, MouseEventArgs mouse)
		{
			int x = 0, y = 0;
			if (isClickInCorrectArea(field, mouse))
			{
				for (int i = 0; i <= 9; i++)
					if (mouse.X >= field.x + i * CellSize && mouse.X < field.x + (i + 1) * CellSize)
						x = i;
				for (int i = 0; i <= 9; i++)
					if (mouse.Y >= field.y + i * CellSize && mouse.Y < field.y + (i + 1) * CellSize)
						y = i;
				return new Coordinates(x, y, CellStatus.Empty);
			};
			return null;
		}

		// поквадратная отрисовка нужного поля
		private void DrawField(Field field, Graphics graph)
		{	
			foreach(Coordinates coords in field.FieldCells)
			{
				Color cellColor;
				switch (coords.status)
				{
					case CellStatus.Ship:
						cellColor = Color.LightGreen;
						break;
					case CellStatus.FiredEmpty:
						cellColor = Color.Aqua;
						break;
					case CellStatus.FiredShip:
						cellColor = Color.Red;
						break;
					default:
						cellColor = Color.White;
						break;
				}
				Rectangle rect = new Rectangle(
					field.x + coords.x * CellSize, 
					field.y + coords.y * CellSize,
					CellSize, CellSize);
				graph.FillRectangle(new SolidBrush(cellColor), rect);
				graph.DrawRectangle(Pens.Black, rect);
			}
		}

		// отрисовываем в зависимости от стадии игры
		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			switch (GameStage)
			{
				case GameStage.TACTICS:
					DrawField(PlayerField, e.Graphics);
					break;
				default:
					DrawField(PlayerField, e.Graphics);
					DrawField(EnemyField, e.Graphics);
					break;
			}
		}

		// обработчик поворота корабля
		private void TurnShipBtn_Click(object sender, EventArgs e)
		{
			int switcher = CurrentShipDirection;
			if(CurrentShipDirection == 0)
			{
				ShipDirection.Text = "↓";
				CurrentShipDirection = 1;
			}
			else
			{
				ShipDirection.Text = "→";
				CurrentShipDirection = 0;
			}
		}

		// эта и последующие процедуры - обработчики нажатия на выбор кораблей
		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			SelectedShipSize = 1;
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			SelectedShipSize = 2;
		}

		private void radioButton3_CheckedChanged(object sender, EventArgs e)
		{
			SelectedShipSize = 3;
		}

		private void radioButton4_CheckedChanged(object sender, EventArgs e)
		{
			SelectedShipSize = 4;
		}

		// увеличиваем или уменьшаем кол-во выбранных кораблей
		private void ManageShips(ShipManagement action, int selectedShipWidth)
		{
			switch (action)
			{
				case ShipManagement.PLACE:
					RemainedForPlaceShips[selectedShipWidth - 1]--;
					break;
				case ShipManagement.DELETE:
					RemainedForPlaceShips[selectedShipWidth - 1]++;
					break;
			}
			switch (selectedShipWidth)
			{
				case 1:
					ShipsLabel1.Text = RemainedForPlaceShips[selectedShipWidth - 1].ToString();
					break;
				case 2:
					ShipsLabel2.Text = RemainedForPlaceShips[selectedShipWidth - 1].ToString();
					break;
				case 3:
					ShipsLabel3.Text = RemainedForPlaceShips[selectedShipWidth - 1].ToString();
					break;
				case 4:
					ShipsLabel4.Text = RemainedForPlaceShips[selectedShipWidth - 1].ToString();
					break;
			}
			
		}

		// основной обработчик игры
		private void Form1_MouseClick(object sender, MouseEventArgs e)
		{
			Coordinates cellCoords;
			switch (GameStage)
			{
				case GameStage.TACTICS:
					cellCoords = GetClickCellCoordinates(PlayerField, e);
					if(cellCoords != null)
					{
						if (e.Button == MouseButtons.Left && SelectedShipSize != 0)
						{
							if (RemainedForPlaceShips[SelectedShipSize - 1] != 0)
							{
								// установка корабля
								if (PlayerField.PlaceShip(cellCoords, CurrentShipDirection, SelectedShipSize))
								{
									ManageShips(ShipManagement.PLACE, SelectedShipSize);
									Invalidate();
								}
							}
						}
						if(e.Button == MouseButtons.Right)
						{
							// удаление корабля
							if (PlayerField.DeleteShipByCoords(cellCoords))
							{
								ManageShips(ShipManagement.DELETE, SelectedShipSize);
								Invalidate();
							}
						}
					}
					break;
				case GameStage.TURN:
					cellCoords = GetClickCellCoordinates(EnemyField, e);
					if(cellCoords != null)
					{	
						// если ячейка уже не была подбита, то подбиваем её
						if (EnemyField.isLegalForAttack(cellCoords))
						{	
							// каждое асинхронное действие (работа с сокетами) выполняется в своём потоке, чтобы форма не висла
							Thread sendMsgThread = new Thread(new ParameterizedThreadStart(SendMessage));
							sendMsgThread.IsBackground = true;
							sendMsgThread.Start(cellCoords);
							Thread getMsgThread = new Thread(GetMessage);
							getMsgThread.IsBackground = true;
							getMsgThread.Start();
						}
					}
					break;
			}
		}

		// когда начинается бой, скрываем элементы, которые мы видели со старта
		private void HideTaсticsElements()
		{
			ShipDirLabel.Visible = false;
			ShipDirection.Visible = false;
			TurnShipBtn.Visible = false;
			ChooseShip.Visible = false;
			StartGameBtn.Visible = false;
		}

		// сериализуем и отправляем серверу json-структуру с расставленными кораблями
		private void SerializeAndSendShips()
		{
			try
			{
				var options = new JsonSerializerOptions { WriteIndented = true }; // чтобы были отступы (было важно при дебаге)
				string jsonShips = System.Text.Json.JsonSerializer.Serialize(PlayerField.FieldShips, options); // строка json
				server.Send(Encoding.UTF8.GetBytes(jsonShips)); // отправляем серверу
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message + "\n" + "Не удалось отправить список кораблей");
				Invalidate();
			}
		}

		// отправка координат подбитой точки серверу
		private void SendMessage(object x)
		{
			// параметр типа object, чтобы можно было передать такую функцию параметризированному потоку
			try
			{
				Coordinates coords = (Coordinates)x;
				string json = System.Text.Json.JsonSerializer.Serialize(coords, typeof(Coordinates));
				byte[] msg = Encoding.UTF8.GetBytes(json);
				int byteSend = server.Send(msg);
			}
			catch
			{
				MessageBox.Show("Ошибка при отправке сообщения");
			}
		}
		// получить сообщение от сервера
		private void GetMessage()
		{
			try
			{
				byte[] bytes = new byte[4096];
				int bytesRec = server.Receive(bytes);
				string jsonGameTurn = Encoding.UTF8.GetString(bytes, 0, bytesRec);
				ServerMessage srvMsg = JsonConvert.DeserializeObject<ServerMessage>(jsonGameTurn); // десериализуем ответ
				// далее обработки разных результатов
				// можно было и оптимизировать на самом деле...
				if(srvMsg.fire_status == "Missed" && srvMsg.game == "Turn")
				{
					ServerMessages.Text = "Противник промахнулся, ваш ход";
					PlayerField.Attack(srvMsg.Dot, CellStatus.FiredEmpty);
					GameStage = GameStage.TURN;
					Invalidate();
				}
				if(srvMsg.fire_status == "Missed" && srvMsg.game == "Wait")
				{
					ServerMessages.Text = "Вы промахнулись, ход противника";
					EnemyField.Attack(srvMsg.Dot, CellStatus.FiredEmpty);
					GameStage = GameStage.WAITTURN;
					Thread waitOpponent = new Thread(GetMessage);
					waitOpponent.IsBackground = true;
					waitOpponent.Start();
					Invalidate();
				}
				if (srvMsg.fire_status == "Fired" && srvMsg.game == "Wait")
				{
					ServerMessages.Text = "Противник попал и ходит вновь";
					PlayerField.Attack(srvMsg.Dot, CellStatus.FiredShip);
					GameStage = GameStage.WAITTURN;
					Thread waitOpponent = new Thread(GetMessage);
					waitOpponent.IsBackground = true;
					waitOpponent.Start();
					Invalidate();
				}
				if (srvMsg.fire_status == "Destroyed" && srvMsg.game == "Wait")
				{
					ServerMessages.Text = "Противник уничтожил ваш корабль, он ходит вновь";
					PlayerField.Attack(srvMsg.Dot, CellStatus.FiredShip);
					GameStage = GameStage.WAITTURN;
					Thread waitOpponent = new Thread(GetMessage);
					waitOpponent.IsBackground = true;
					waitOpponent.Start();
					Invalidate();
				}
				if (srvMsg.fire_status == "Fired" && srvMsg.game == "Turn")
				{
					ServerMessages.Text = "Вы попали по кораблю и снова ходите";
					EnemyField.Attack(srvMsg.Dot, CellStatus.FiredShip);
					GameStage = GameStage.TURN;
					Invalidate();
				}
				if (srvMsg.fire_status == "Destroyed" && srvMsg.game == "Turn")
				{
					ServerMessages.Text = "Вы уничтожили корабль противника и ходите вновь";
					EnemyField.Attack(srvMsg.Dot, CellStatus.FiredShip);
					GameStage = GameStage.TURN;
					Invalidate();
				}
				if (srvMsg.fire_status == "-" && srvMsg.game == "Wait")
				{
					ServerMessages.Text = "Сервер определил, что вам надо подождать. Первым ходит оппонент";
					GameStage = GameStage.WAITTURN;
					Thread waitOpponent = new Thread(GetMessage);
					waitOpponent.IsBackground = true;
					waitOpponent.Start();
					Invalidate();
				}
				if (srvMsg.fire_status == "-" && srvMsg.game == "Turn")
				{
					ServerMessages.Text = "Сервер определил, что вы начинаете эту битву";
					GameStage = GameStage.TURN;
					Invalidate();
				}
				if (srvMsg.game == "Win")
				{
					ServerMessages.Text = "Вы выиграли";
					EnemyField.Attack(srvMsg.Dot, CellStatus.FiredShip);
					GameStage = GameStage.TURN;
					Invalidate();
				}
				if (srvMsg.game == "Defeat")
				{
					ServerMessages.Text = "Вы проиграли";
					PlayerField.Attack(srvMsg.Dot, CellStatus.FiredShip);
					GameStage = GameStage.WAITTURN;
					Invalidate();
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message + "\n" + "Ошибка при получении сообщения");
				Invalidate();
			}
		}
		// подключиться к серверу
		private void ConnectToServer()
		{
			try
			{
				IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); // получаем хост (localhost)
				IPAddress ipAddress = ipHostInfo.AddressList[0]; // получаем ip-адрес хоста
				IPEndPoint remoteEP = new IPEndPoint(ipAddress, 1234); // соединяем адрес с портом
				server = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // создаём сокет
				server.Connect(remoteEP); // подключаемся к серверу
				ServerMessages.Text = "Подключено к серверу, ожидание второго игрока";
				ServerMessages.Visible = true;
				Invalidate();
			}
			catch(Exception e)
			{	
				
				MessageBox.Show(e.Message + "\n" + "Не удаётся подключится к серверу, перезапустите игру");
				Invalidate();
			}
		}

		private void StartGameBtn_Click(object sender, EventArgs e)
		{
			for(int i = 0; i < RemainedForPlaceShips.Length; i++) 
				if(RemainedForPlaceShips[i] != 0)
				{
					MessageBox.Show("Вы разместили не все корабли!");
					return;
				}
			HideTaсticsElements(); // скрываем старые элементы
			GameStage = GameStage.WAITCONNECT;
			// коннектимся к серверу
			ConnectToServer();
			// отправляем json с кораблями, получаем ответ, и всё в разных потоках
			Thread SendShips = new Thread(SerializeAndSendShips);
			SendShips.IsBackground = true;
			SendShips.Start();
			Thread getmsg = new Thread(GetMessage);
			getmsg.IsBackground = true;
			getmsg.Start();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				if (server != null)
				{
					server.Shutdown(SocketShutdown.Both);
					server.Close();
				}
			}
			catch
			{
				MessageBox.Show("Ошибка разрыва соединения с сервером, ничего страшного...");
			}
		}
	}
}
