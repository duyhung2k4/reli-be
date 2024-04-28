using System.Reflection.Metadata;

namespace _2reli_api
{
    public class User
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Phone_Number { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public string ?Email { get;set; }
        public string ?Province { get; set; }
        public string ?District { get; set; }
        public string ?Ward { get; set; }
        public string ?Address { get; set; }
        public DateTime ?Dob { get; set; }
        public int ?Gender { get; set; }
        public string ?Jwt { get; set; }
    }
}
