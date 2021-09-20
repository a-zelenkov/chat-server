namespace chat.sssu.Models
{
    public class User
    {
        public int Id { get; set; }
      
        public string Login { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Token { get; set; }
    }

    public class UserLogin
    {
        public string Login { get; set; }

        public string Password { get; set; }

    }

    public class UserToken
    {
        public string Token { get; set; }

    }
}
