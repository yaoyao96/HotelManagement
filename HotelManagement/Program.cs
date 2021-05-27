using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace HotelManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            Hotel h = new Hotel();
            for (int i = 0; i < 20; i++)
            {
                h.GetRoomForCheckin();
            }
            Console.ReadKey();
        }
    }

    public class Hotel
    {
        ConcurrentDictionary<string, Room> RoomNum2Room;

        public Hotel()
        {
            // Initialze 
            RoomNum2Room = new ConcurrentDictionary<string, Room>();
            for (int i = 1; i < 5; i++)
            {
                for (char c = 'A'; c < 'F'; c++)
                {
                    string rm = i.ToString() + c;
                    Room room = new Room(rm);
                    RoomNum2Room[rm] = room;
                }
            }
        }

                
        public List<Room> GetAllAvalibelRooms()
        {
            // O(n)
            List<Room> availableRooms = new List<Room>(); 
            foreach (var kvp in RoomNum2Room)
            {
                if (kvp.Value.Status == RoomStatus.Available) availableRooms.Add(kvp.Value);
            }
            return availableRooms;
        }

        // guest check in
        public string GetRoomForCheckin()
        {
            List<Room> allRooms = GetAllAvalibelRooms();
            if (allRooms.Count == 0) { Log("No room to check in ");  return null; }

            // O(n) search
            var assigned = allRooms[0];
            for (int i = 1; i < allRooms.Count; i++)
            {
                if(assigned.CompareTo(allRooms[i])==1) assigned= allRooms[i];
            }
            
            assigned.Status = RoomStatus.Occupied;
            Log("Customer checked in Room= " + assigned.ToString());
            return assigned.RoomNumber;
        }

        // guest check out
        public void CheckOutRoom(string roomNumber)
        {
            string error = MarkVacant(roomNumber);
            if (!String.IsNullOrEmpty(error)) { Log(String.Format("Check-out failed. Reason: {0}, Room: {1}",error, roomNumber)); return; }
            
            Log("Customer checked out Room=" + roomNumber);
        }

        // To mark a room as Vacant, used when checkout only
        string MarkVacant(string roomNumber)
        {
            Room r;
            if (!RoomNum2Room.TryGetValue(roomNumber, out r)) { return "Room does not exist"; }
            r.Status = RoomStatus.Vacant;
            Log("Mark Vacant. Room=" + r.ToString());
            return null;
        }

        // To mark a room as Repair, used by repair person
        public string MarkRepair(string roomNumber)
        {
            Room r;
            if (!RoomNum2Room.TryGetValue(roomNumber, out r)) { return "Room does not exist"; }
            if (r.Status != RoomStatus.Vacant) { return "Room is not in vacant status"; }
            r.Status = RoomStatus.Repair;
            Log("Mark Repair. Room=" + r.ToString());
            return null;
        }

        // clean the room and change to available
        public void DoCleaning(string roomNumber)
        {
            Room r;
            if (!RoomNum2Room.TryGetValue(roomNumber, out r)) { Log("Clean failed. Room does not exist, Room:" + roomNumber); return; }
            if (r.Status != RoomStatus.Vacant) { Log("Clean failed. Room status is not in vacant status, Room:" + r.ToString()); return; }
            r.Status = RoomStatus.Available;
            Log("HouseKeeper cleaned Room=" + r.ToString());
        }

        // repair the room and change to vacant
        public void DoRepair(string roomNumber)
        {
            Room r;
            if (!RoomNum2Room.TryGetValue(roomNumber, out r)) { Log("Repair failed. Room does not exist, Room:" + roomNumber); return; }
            if (r.Status != RoomStatus.Repair) { Log("Repaire failed. Room status is not in repair status, Room:" + r.ToString()); return; }
            r.Status = RoomStatus.Vacant;
            Log("HouseKeeper repaired Room=" + r.ToString());
        }

        public Room GetRoomByRoomNum(string roomNum)
        {
            Room room;
            if (RoomNum2Room.TryGetValue(roomNum, out room)) return room;
            return null;
        }

        public List<string> GetAllRoomNumbers()
        {
            return RoomNum2Room.Keys.ToList();
        }
        void Log(string text)
        {
            Console.WriteLine(DateTime.Now.ToShortTimeString() + " " + text);
        }
    }
    
    public enum RoomStatus { Available, Occupied, Vacant, Repair }
    public class Room : IComparable<Room>
    {
        public string RoomNumber { get; set; }

        public RoomStatus Status { get; set; }

        public Room(string RoomNumber)
        {
            this.RoomNumber = RoomNumber;
            this.Status = RoomStatus.Available;
        }

        public override string ToString()
        {
            return RoomNumber + ":" + Status;
        }

        public int CompareTo(Room other)
        {
            int level = int.Parse(RoomNumber.Substring(0, 1));
            int levelO = int.Parse(other.RoomNumber.Substring(0, 1));
            if (level != levelO)
            {
                return level.CompareTo(levelO);
            }

            char alph = RoomNumber[1];
            char alphO = other.RoomNumber[1];
            if (alph != alphO)
            {
                if (level % 2 != 0) return alph.CompareTo(alphO);
                else return alphO.CompareTo(alph);
            }

            return 0;
        }
    }

}
