using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Infrastructure.Services
{
    public class TrackingNumberGenerator
    {
        private readonly ApplicationDbContext _context;
        private static readonly object _lock = new object();
        private static int _lastSequence = 0;

        public TrackingNumberGenerator(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateTrackingNumberAsync()
        {
            lock (_lock)
            {
                _lastSequence++;
                var date = DateTime.UtcNow;
                return $"ORD-{date:yyyyMMdd}-{_lastSequence:D6}";
            }
        }

        public async Task InitializeLastSequenceAsync()
        {
            if (_lastSequence == 0)
            {
                var lastOrder = await _context.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .FirstOrDefaultAsync();

                if (lastOrder != null && !string.IsNullOrEmpty(lastOrder.TrackingNumber))
                {
                    var parts = lastOrder.TrackingNumber.Split('-');
                    if (parts.Length == 3 && int.TryParse(parts[2], out int sequence))
                    {
                        _lastSequence = sequence;
                    }
                }
            }
        }
    }
} 