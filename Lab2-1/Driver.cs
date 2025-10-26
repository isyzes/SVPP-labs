using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2_1
{
    public enum GENDER { male, female, variant};
    public enum COLOREYES { brown, blue, green, gray};
    public class Driver
    {
        int number;
        char clazz;
        double hgt;
        string name;
        string address;
        GENDER gender;
        COLOREYES eyes;
        DateTime dob;
        DateTime iss;
        DateTime exp;
        bool donor;
        string uriImage;

        public Driver()
        {
        }

        public override string ToString()
        {
            return $"Driver:\n" +
                   $"  Number: {Number}\n" +
                   $"  Class: {Clazz}\n" +
                   $"  Height: {Hgt}\n" +
                   $"  Name: {Name}\n" +
                   $"  Address: {Address}\n" +
                   $"  Gender: {Gender}\n" +
                   $"  Eye Color: {Eyes}\n" +
                   $"  Date of Birth: {Dob:yyyy-MM-dd}\n" +
                   $"  Issued: {Iss:yyyy-MM-dd}\n" +
                   $"  Expires: {Exp:yyyy-MM-dd}\n" +
                   $"  Organ Donor: {(Donor ? "Yes" : "No")}\n" +
                   $"  Image URI: {UriImage}";
        }

        public int Number { get => number; set => number = value; }
        public char Clazz { get => clazz; set => clazz = value; }
        public double Hgt { get => hgt; set => hgt = value; }
        public string Name { get => name; set => name = value; }
        public string Address { get => address; set => address = value; }
        public GENDER Gender { get => gender; set => gender = value; }
        public COLOREYES Eyes { get => eyes; set => eyes = value; }
        public DateTime Dob { get => dob; set => dob = value; }
        public DateTime Iss { get => iss; set => iss = value; }
        public DateTime Exp { get => exp; set => exp = value; }
        public bool Donor { get => donor; set => donor = value; }
        public string UriImage { get => uriImage; set => uriImage = value; }
    }
}
