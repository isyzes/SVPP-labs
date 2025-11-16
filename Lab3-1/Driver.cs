using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lab3_1
{
    public enum GENDER { male, female, other }
    public enum COLOREYES { brown, green, gray, blue }

    public class Driver : INotifyPropertyChanged, IDataErrorInfo
    {
        int? number;
        char? class1;
        string? name;
        string? adress;
        double? hgt;
        DateTime? dob;
        DateTime? iss;
        DateTime? exp;
        GENDER? gender;
        COLOREYES? coloreyes;
        bool? donor;
        string? uriImage;

        public Driver()
        {
        }

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "Number":
                        if (Number == null || Number < 100000 || Number > 99999999)
                            error = "Номер должен быть от 100000 до 99999999";
                        break;
                    case "Class1":
                        if (Class1 == null || Class1 < 'A' || Class1 > 'E')
                            error = "Класс должен быть от A до E";
                        break;
                    case "Name":
                        if (String.IsNullOrWhiteSpace(Name))
                            error = "Имя не должно быть пустым";
                        break;
                    case "Adress":
                        if (String.IsNullOrWhiteSpace(Adress))
                            error = "Адрес не должен быть пустым";
                        break;
                    case "Hgt":
                        if (Hgt == null || Hgt < 100.0 || Hgt > 250.0)
                            error = "Рост должен быть от 100.0 до 250.0 см";
                        break;
                    case "Dob":
                        if (Dob == null || Dob > DateTime.Now.AddYears(-18) || Dob < DateTime.Now.AddYears(-100))
                            error = "Дата рождения должна быть не менее 18 лет и не более 100 лет";
                        break;
                    case "Iss":
                        if (Iss == null || Iss > DateTime.Now || (Dob != null && Iss < Dob.Value.AddYears(18)))
                            error = "Дата выдачи должна быть не позже сегодняшней даты и не раньше чем через 18 лет после даты рождения";
                        break;
                    case "Exp":
                        if (Exp == null || Iss == null || Exp < Iss || Exp > Iss.Value.AddYears(10))
                            error = "Дата окончания должна быть не раньше даты выдачи и не позже чем через 10 лет после даты выдачи";
                        break;
                }
                return error;
            }
        }

        public int? Number { get => number; set { number = value; OnPropertyChanged("Number"); } }
        public char? Class1 { get => class1; set { class1 = value; OnPropertyChanged("Class1"); } }
        public string? Name { get => name; set { name = value; OnPropertyChanged("Name"); } }
        public string? Adress { get => adress; set { adress = value; OnPropertyChanged("Adress"); } }
        public double? Hgt { get => hgt; set { hgt = value; OnPropertyChanged("Hgt"); } }
        public DateTime? Dob { get => dob; set { dob = value; OnPropertyChanged("Dob"); } }
        public DateTime? Iss { get => iss; set { iss = value; OnPropertyChanged("Iss"); } }
        public DateTime? Exp { get => exp; set { exp = value; OnPropertyChanged("Exp"); } }
        public GENDER? Gender { get => gender; set { gender = value; OnPropertyChanged("Gender"); } }
        public COLOREYES? Coloreyes { get => coloreyes; set { coloreyes = value; OnPropertyChanged("Coloreyes"); } }
        public bool? Donor { get => donor; set { donor = value; OnPropertyChanged("Donor"); } }
        public string? UriImage { get => uriImage; set { uriImage = value; OnPropertyChanged("UriImage"); } }

        public string Error => throw new NotImplementedException();

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public override string? ToString()
        {
            return $"№{Number?.ToString()} {Class1?.ToString()}  {Name?.ToString()} " +
                $"{Adress?.ToString()} {Hgt?.ToString()}  {Dob?.ToString("dd.MM.yyyy")} " +
           $" от {Iss?.ToString("dd.MM.yyyy")} до {Exp?.ToString("dd.MM.yyyy")} пол {Gender} " +
           $"глаза {coloreyes?.ToString()} {(Donor == true ? "Донор" : "Не донор")} \n {UriImage?.ToString()}";
        }
    }
}
