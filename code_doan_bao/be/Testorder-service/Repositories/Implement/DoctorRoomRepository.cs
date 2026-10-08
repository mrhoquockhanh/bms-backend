using Microsoft.EntityFrameworkCore;
using TestOrder.Repositories.Interface;
using Testorder_service.Data;
using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Implement
{
    public class DoctorRoomRepository : IDoctorRoomRepository
    {
        private readonly TestOrderManagementDbContext _context;

        public DoctorRoomRepository(TestOrderManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoctorRoom>> GetAllAsync()
        {
            return await _context.DoctorRooms.ToListAsync();
        }

        public async Task<DoctorRoom?> GetByIdAsync(string id)
        {
            return await _context.DoctorRooms.FindAsync(id);
        }

        public async Task<DoctorRoom> CreateAsync(DoctorRoom room)
        {
            _context.DoctorRooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<DoctorRoom?> UpdateAsync(string id, DoctorRoom room)
        {
            var existingRoom = await _context.DoctorRooms.FindAsync(id);
            if (existingRoom == null)
                return null;

            existingRoom.RoomName = room.RoomName;
            existingRoom.RoomLocation = room.RoomLocation;

            await _context.SaveChangesAsync();
            return existingRoom;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var room = await _context.DoctorRooms.FindAsync(id);
            if (room == null)
                return false;

            _context.DoctorRooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}