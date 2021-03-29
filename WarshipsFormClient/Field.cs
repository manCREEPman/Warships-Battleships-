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

namespace WarshipsFormClient
{   
    // Структура для ячеек поля
    enum CellStatus
    {
        Empty,
        Ship,
        FiredEmpty,
        FiredShip
    }
    // класс хранит координаты и статус ячейки
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
    // класс для отправки ссообщений на клиент
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
    // класс корабля
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
    // класс поля
    class Field
    {   
        public int x { get; set; }
        public int y { get; set; }
        private List<Coordinates> Cells; // наше поле 10х10
        private List<Ship> Ships; // все корабли на поле

        // геттеры для приватных полей
        public List<Coordinates> FieldCells
        {
            get { return Cells;  }
        }
        public List<Ship> FieldShips
        {
            get { return Ships; }
        }

        public Field()
        {
            Cells = new List<Coordinates>();
            Ships = new List<Ship>();
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++) Cells.Add(new Coordinates(i, j, CellStatus.Empty));
        }
        private Boolean CanPlaceShip(int x, int y, int direction, int width)
        {
            int leftSide, highSide, rightSide = 0, bottomSide = 0;
            if (x - 1 >= 0) leftSide = x - 1;
            else leftSide = x;
            if (y - 1 >= 0) highSide = y - 1;
            else highSide = y;
            switch (direction)
            {
                case 0:
                {
                    if (x + width - 1 + 1 <= 9) rightSide = x + width - 1 + 1;
                    else
                    if (x + width - 1 <= 9) rightSide = x + width - 1;
                    else return false;
                    if (highSide + 2 <= 9 && highSide != 0 && highSide != 8) bottomSide = highSide + 2;
                    else
                    if (highSide + 1 <= 9) bottomSide = highSide + 1;
                }
                break;
                case 1:
                {
                    if (x + 1 <= 9) rightSide = x + 1;
                    else rightSide = x;
                    if (y + width - 1 + 1 <= 9) bottomSide = y + width - 1 + 1;
                    else
                    if (y + width - 1 <= 9) bottomSide = y + width - 1;
                    else return false;
                }
                break;
            }
            //MessageBox.Show(
            //    "current cell: " + "\n" +
            //    "x: " + x.ToString() + " y: " + y.ToString() + "\n" + "\n" +
            //    "up: " + highSide.ToString() + "\n" +
            //    "down: " + bottomSide.ToString() + "\n" +
            //    "left: " + leftSide.ToString() + "\n" +
            //    "right: " + rightSide.ToString() + "\n");
            return !Cells.FindAll(
                        t => t.x >= leftSide && t.x <= rightSide &&
                        t.y >= highSide && t.y <= bottomSide)
                        .Exists(t => t.status == CellStatus.Ship);
        }
        // возвращает истину, если корабль размещён на поле
        public Boolean PlaceShip(Coordinates startCoords, int direction, int width)
        {
            if (CanPlaceShip(startCoords.x, startCoords.y, direction, width))
            {
                List<Coordinates> sCoords = new List<Coordinates>();
                int ind;
                switch (direction)
                {
                    case 0:
                    {
                        for (int i = 0; i < width; i++)
                        {
                            ind = -1;
                            ind = Cells.FindIndex(t => t.x == startCoords.x + i && t.y == startCoords.y);
                            if (ind >= 0)
                            {
                                Cells[ind].status = CellStatus.Ship;
                                sCoords.Add(Cells[ind]);
                            }
                        }
                    }
                    break;
                    case 1:
                    {
                        for (int i = 0; i < width; i++)
                        {
                            ind = -1;
                            ind = Cells.FindIndex(t => t.x == startCoords.x && t.y == startCoords.y + i);
                            if (ind >= 0)
                            {
                                Cells[ind].status = CellStatus.Ship;
                                sCoords.Add(Cells[ind]);
                            }
                        }
                    }
                    break;
                }
                Ships.Add(new Ship(sCoords));
                return true;
            }
            return false;
        }

        private Ship FindShipByCoords(int x, int y)
        {
            foreach(Ship ship in Ships)
            {
                foreach (Coordinates coords in ship.ShipCoords)
                    if (coords.x == x && coords.y == y) return ship;
            }
            return null;
        }
        // если в переданных координатах есть корабль - удаляет его с поля 
        public Boolean DeleteShipByCoords(Coordinates coords)
        {
            int ind;
            Ship target = FindShipByCoords(coords.x, coords.y);
            if(target != null)
            {
                foreach(Coordinates c in target.ShipCoords)
                {
                    ind = -1;
                    ind = Cells.FindIndex(t => t.x == c.x && t.y == c.y);
                    if (ind != -1) Cells[ind].status = CellStatus.Empty;
                }
                Ships.Remove(target);
                return true;
            }
            return false;
        }
        // проверяет, можно ли атаковать ячейку ( не подбита ли уже)
        public Boolean isLegalForAttack(Coordinates coords)
        {
            return (Cells.Exists(t => t.x == coords.x && t.y == coords.y && t.status == CellStatus.Empty));
        }
        // Присваивает статус переданной ячейке
        public void Attack(Coordinates coords, CellStatus st)
        {
            int ind = -1;
            ind = Cells.FindIndex(t => t.x == coords.x && t.y == coords.y);
            if(ind != -1) Cells[ind].status = st;
        }
    }

}
