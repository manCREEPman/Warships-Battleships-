using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;

namespace WarshipsServer
{
    enum CellStatus
    {
        Empty,
        Ship,
        FiredEmpty,
        FiredShip
    }

    [Serializable]
    class Coordinates
    {
        public int x { get; set; }
        public int y { get; set; }
        public CellStatus status { get; set; }
        public Coordinates(int x, int y, CellStatus st)
        {
            this.x = x;
            this.y = y;
            status = st;
        }
    }

    [Serializable]
    class ServerMessage
    {
        public string fire_status { get; set; }
        public string game { get; set; }
        public Coordinates Dot { get; set; } 
        public ServerMessage(string fs, string g)
        {
            fire_status = fs; game = g;
            Dot = new Coordinates(0, 0, CellStatus.Empty);
        }
    }

    [Serializable]
    class Ship
    {
        public List<Coordinates> ShipCoords { get; set; }
        public Boolean isDrown { get; set; }
        public Ship(List<Coordinates> shipCoords)
        {
            ShipCoords = shipCoords;
            isDrown = false;
        }
        public Boolean ReleaseDrown()
        {
            if (ShipCoords.All(x => x.status == CellStatus.FiredShip))
                isDrown = true;
            return isDrown;
        }
    }

    // класс игрока - все его корабли и статус проигрыша
    class Player
    {
        public PlayersShips PShips { get; set; }
        public Boolean isLost { get; set; }
        public Player(string shipsJSON)
        {
            PShips = new PlayersShips(GetShipListFromJSON(shipsJSON));
            isLost = false;
        }
        private List<Ship> GetShipListFromJSON(string shipsJSON)
        { 
            var ships = JsonConvert.DeserializeObject<List<Ship>>(shipsJSON);
            return ships;
        }
        // проверка, затонули ли все корабли
        public Boolean ReleaseLost()
        {
            if (PShips.Ships.All(x => x.isDrown == true))
                isLost = true;
            return isLost;
        }
        // формирует серверное сообщение с параметрами игры для ждущего игрока
        public ServerMessage FormPlayerStatusByEnemyAttack(Coordinates coords)
        {   
            string fireStatus = PShips.FireCoordinate(coords);
            string gameStatus = fireStatus == "Missed" ? "Turn" : "Wait";
            if (ReleaseLost()) gameStatus = "Defeat";
            Console.WriteLine("Детализация сообщения ждущему игроку: f: {0}, g: {1}", fireStatus, gameStatus);
            ServerMessage response = new ServerMessage(fireStatus, gameStatus);
            response.Dot = coords;
            return response;
        }
    }
    // класс кораблей игрока
    class PlayersShips
    {
        public List<Ship> Ships { get; set; }
        public PlayersShips(List<Ship> ships)
        {
            Ships = ships;
        }
        // формирует статус попадания по ячейке игрока
        public string FireCoordinate(Coordinates coord)
        {
            foreach (Ship ship in Ships)
            {
                foreach (Coordinates cell in ship.ShipCoords)
                {
                    if (coord.x == cell.x && coord.y == cell.y)
                    {
                        cell.status = CellStatus.FiredShip;
                        if (ship.ReleaseDrown()) return "Destroyed";
                        return "Fired";
                    }
                }
            }
            return "Missed";
        }
    }

    // класс сервера, содержит словарь подключенных сокетов и словарь игроков 
    class Server
    {
        private Dictionary<int, Player> Players;
        private Dictionary<int, Socket> Connections;
        private int ActivePlayerNumber;
        private int WaitingPlayerNumber;
        
        public Server()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 1234);
                byte[] bytes = new byte[4096];
                string jsonShips;

                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(2);
                Connections = new Dictionary<int, Socket>();
                Players = new Dictionary<int, Player>();
                int connectedPlayers = 0;
                // ждём, пока 2 игрока подключатся
                while (connectedPlayers < 2)
                {
                    Socket handler = listener.Accept();
                    Console.WriteLine("Присоединился игрок");
                    Connections.Add(connectedPlayers, handler);
                    int bytesReceived = handler.Receive(bytes);
                    jsonShips = Encoding.UTF8.GetString(bytes, 0, bytesReceived);
                    Players.Add(connectedPlayers, new Player(jsonShips));
                    connectedPlayers++;
                }
                // определяем очередь хода
                Random rand = new Random();
                ActivePlayerNumber = rand.Next() % 2;
                WaitingPlayerNumber = ActivePlayerNumber == 0 ? 1 : 0;
                Console.WriteLine("Определён порядок ходов, первый ходит {0}", ActivePlayerNumber.ToString());
            }
            catch
            {
                Console.WriteLine("Что-то пошло не так");
            }
        }

        // отправляет серверное сообщение на переданный сокет
        private void SendMessage(ServerMessage msg, Socket receiver)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = System.Text.Json.JsonSerializer.Serialize(msg, options);
                byte[] bytesMsg = Encoding.UTF8.GetBytes(json);
                receiver.Send(bytesMsg);
            }
            catch
            {
                Console.WriteLine("Возникли ошибки при отправке сообщения");
            }
        }

        private Boolean isGameEnded()
        {
            return Players.Values.Any(t => t.isLost == true);
        }

        // игровой процесс
        public void Game()
        {
            try
            {   
                // будем отправлять серверные сообщения клиентам
                // msg1 для активного игрока (кто последний походил)
                // msg2 для ожидающего игрока
                ServerMessage msg1 = new ServerMessage("", "");
                ServerMessage msg2 = new ServerMessage("", "");
                int turn = 0;
                while (true)
                {
                    Console.WriteLine("Ход: {0}", turn.ToString());
                    // на первом ходу просто рассылаем сведения об очерёдности
                    if(turn == 0)
                    {   
                        msg2.fire_status = "-";
                        msg2.game = "Wait";
                        SendMessage(msg2, Connections[WaitingPlayerNumber]);
                        msg1.fire_status = "-";
                        msg1.game = "Turn";
                        SendMessage(msg1, Connections[ActivePlayerNumber]);
                        turn++;
                        Console.WriteLine("Игроки получили сведения о своём первом ходе");
                    }
                    else
                    {
                        turn++;
                        byte[] bytes = new byte[4096];
                        int bytesRec = Connections[ActivePlayerNumber].Receive(bytes);
                        string jsonCoords = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        // получили ячейку от атакующего игрока
                        Coordinates firedCoords = JsonConvert.DeserializeObject<Coordinates>(jsonCoords);
                        Console.WriteLine("Получены данные от активного игрока {0}. Атакуемая точка {1}, {2}"
                            , ActivePlayerNumber, firedCoords.x, firedCoords.y);

                        // не реализованная логика подменю с кнопкой "сдаться"
                        if (firedCoords.x == -1 && firedCoords.y == -1)
                        {
                            msg1.fire_status = "-"; msg1.game = "Defeat";
                            msg2.fire_status = "-"; msg2.game = "Win";
                            Console.WriteLine("Игра закончена, игрок {0} сдался", ActivePlayerNumber);
                        }
                        else
                        {   
                            // формируем сообщение ожидающему игроку и атакующему
                            msg2 = Players[WaitingPlayerNumber].FormPlayerStatusByEnemyAttack(firedCoords);
                            msg1.fire_status = msg2.fire_status;
                            msg1.Dot = firedCoords;
                            if (msg2.game == "Wait" && msg2.fire_status == "Fired")
                            {
                                msg1.game = "Turn";
                                Console.WriteLine("Игрок {0} повредил корабль {1}", ActivePlayerNumber, WaitingPlayerNumber);
                            }
                            if(msg2.game == "Wait" && msg2.fire_status == "Destroyed")
                            {
                                msg1.game = "Turn";
                                Console.WriteLine("Игрок {0} уничтожил корабль {1}", ActivePlayerNumber, WaitingPlayerNumber);
                            }
                            if(msg2.game == "Turn" && msg2.fire_status == "Missed")
                            {
                                msg1.game = "Wait";
                                Console.WriteLine("Игрок {0} промахнулся", ActivePlayerNumber);
                            }
                            if(msg2.game == "Defeat")
                            {
                                msg1.game = "Win";
                                Console.WriteLine("Игрок {0} победил", ActivePlayerNumber);
                            }
                        }
                        SendMessage(msg1, Connections[ActivePlayerNumber]);
                        SendMessage(msg2, Connections[WaitingPlayerNumber]);
                        Console.WriteLine("Игроки получили данные о новом ходе.");
                        if (isGameEnded())
                        {
                            Console.WriteLine("Игра закончена");
                            break;
                        }

                        if (msg2.game == "Turn") // если ожидающий игрок получил ход, то меняем местами номера игроков
                        {
                            Console.WriteLine("Смена хода игроков");
                            int buff = ActivePlayerNumber;
                            ActivePlayerNumber = WaitingPlayerNumber;
                            WaitingPlayerNumber = buff;
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Что-то пошло не так, завершение соединений");
                foreach (Socket con in Connections.Values)
                {
                    con.Shutdown(SocketShutdown.Both);
                    con.Close();
                }
            }
        }
    }

    class Programm
    {
        public static void Main()
        {
            Server s = new Server();
            s.Game();
            Console.ReadKey();
        }
    }
}

/* как выглядит структура кораблей
    [
  {
    "ShipCoords": [
      {
        "x": 475087111,
        "y": 1991014545,
        "status": 0
      },
      {
        "x": 1823688171,
        "y": 1073255979,
        "status": 0
      },
      {
        "x": 835578712,
        "y": 711958975,
        "status": 0
      }
    ],
    "isDrown": false
  },
  {
    "ShipCoords": [
      {
        "x": 1056233294,
        "y": 1356651805,
        "status": 0
      },
      {
        "x": 579059377,
        "y": 2023803104,
        "status": 0
      },
      {
        "x": 983797194,
        "y": 207587955,
        "status": 0
      }
    ],
    "isDrown": false
  },
  {
    "ShipCoords": [
      {
        "x": 236871575,
        "y": 408088176,
        "status": 0
      },
      {
        "x": 1877872463,
        "y": 1011331674,
        "status": 0
      },
      {
        "x": 873501730,
        "y": 90614756,
        "status": 0
      }
    ],
    "isDrown": false
  }
]
 */

/*
 подключенные библиотеки
 Newtonsoft.JSON
 System.Text.JSON*/
