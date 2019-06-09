using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreUnitTests.Domain.Models
{
    public enum DockStatus
    {
        Unavailable,
        Available,
        Reserved,
        Occupied,
        Loading,
        Unloading,
    }

    public class Dock
    {
        public string Id { get; private set; }
        public string Identifier { get; private set; }
        public string DockedShipId { get; private set; }
        public DockStatus Status { get; private set; }
        public bool IsOccupied => !string.IsNullOrWhiteSpace(DockedShipId);


        public void DockShip(Ship ship)
        {
        }

        public void UndockShip(Ship ship)
        {
        }
    }
}
