using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2_2
{
    public enum LotCategory
    {
        Art,           // Искусство
        Jewelry,       // Ювелирные изделия
        Antique,       // Антиквариат
        Electronics,   // Электроника
        RealEstate     // Недвижимость
    }

    public enum LotStatus
    {
        Черновик,
        На_проверке,
        Одобрен,
        Отклонен
    }

    public class AuctionLot
    {
        public string Title { get; set; }
        public LotCategory Category { get; set; }
        public bool IsVerified { get; set; }
        public bool IsUrgent { get; set; }
        public LotStatus Status { get; set; }
        public System.DateTime? StartDate { get; set; }
        public System.DateTime? EndDate { get; set; }
        public string ImagePath { get; set; }
        public double StartPrice { get; set; }
    }
}
