using System;
using System.Collections.Generic;
using System.Text;

namespace Lab_8.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Provider { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
