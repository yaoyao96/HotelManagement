using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HotelManagement;


namespace HotelManagementTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetRoomForCheckinTest()
        {
            Hotel h = new Hotel();
                        
            string roomNum1 = h.GetRoomForCheckin();
            // Only available rooms can be assigned to guests which will then become occupied.
            Room r1 = h.GetRoomByRoomNum(roomNum1);
            Assert.AreEqual(RoomStatus.Occupied, r1.Status);

            // Next nearest one and first one are not equal
            string roomNum2 = h.GetRoomForCheckin();
            Assert.AreNotEqual(roomNum1, roomNum2);

            // when the firest one is released, should return the first room for next check in
            r1.Status = RoomStatus.Available;
            string roomNum3 = h.GetRoomForCheckin();
            Room r3 = h.GetRoomByRoomNum(roomNum1);
            Assert.AreEqual(r1, r3);
        }

        [TestMethod]
        public void GetAllAvalibelRoomsTest()
        {
            Hotel h = new Hotel();
            
            List<Room> availRooms = h.GetAllAvalibelRooms();

            foreach (var room in availRooms)
            {
                Assert.AreEqual(RoomStatus.Available, room.Status);
            }

            // create a ramdom room
            List<string> roomNums = h.GetAllRoomNumbers();
            Random rnd = new Random();
            int index = rnd.Next(roomNums.Count-1);
            string roomNum = roomNums[index];
            Room r = h.GetRoomByRoomNum(roomNum);

            r.Status = RoomStatus.Occupied;
            availRooms = h.GetAllAvalibelRooms();
            // should not contain occupied room
            Assert.IsFalse(availRooms.Contains(r));
        }

        [TestMethod]
        public void CheckOutRoomTest()
        {
            Hotel h = new Hotel();
            Room r = h.GetRoomByRoomNum("1A");
            r.Status = RoomStatus.Occupied;

            //After the guest checks out of the room, the room becomes vacant
            h.CheckOutRoom(r.RoomNumber);
            Assert.AreEqual(RoomStatus.Vacant, r.Status);

        }

        [TestMethod]
        public void MarkRepairTest()
        {
            Hotel h = new Hotel();
            Room r = h.GetRoomByRoomNum("1A");
            r.Status = RoomStatus.Vacant;
            //Housekeeping may take a vacant room out of service for repairs by marking the room as repair.
            h.MarkRepair(r.RoomNumber);
            Assert.AreEqual(RoomStatus.Repair, r.Status);
        }

        [TestMethod]
        public void DoCleaningTest()
        {
            Hotel h = new Hotel();
            Room r = h.GetRoomByRoomNum("1A");
            r.Status = RoomStatus.Vacant;
            //After housekeeping cleans the vacant room, it becomes available.
            h.DoCleaning(r.RoomNumber);
            Assert.AreEqual(RoomStatus.Available, r.Status);
        }

        [TestMethod]
        public void DoRepairTest()
        {
            Hotel h = new Hotel();
            Room r = h.GetRoomByRoomNum("1A");
            r.Status = RoomStatus.Repair;
            // Once a room is repaired, it will become vacant again for cleaning.
            h.DoRepair(r.RoomNumber);
            Assert.AreEqual(RoomStatus.Vacant, r.Status);

            //Available and occupied rooms cannot be repaired.
            r.Status = RoomStatus.Available;
            h.DoRepair(r.RoomNumber);
            Assert.AreEqual(RoomStatus.Available, r.Status);

            r.Status = RoomStatus.Occupied;
            h.DoRepair(r.RoomNumber);
            Assert.AreEqual(RoomStatus.Occupied, r.Status);
        }

    }
}
