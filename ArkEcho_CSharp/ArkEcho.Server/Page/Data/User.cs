namespace ArkEcho.Server
{
    public class User
    {
        public string EmailAddress { get; internal set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserRole Role { get; set; }
        public class UserRole
        {
            public short RoleId { get; set; }
            public string RoleDesc { get; set; }
        }
    }

}
