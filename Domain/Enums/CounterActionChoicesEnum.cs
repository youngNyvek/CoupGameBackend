namespace Domain.Enums
{
    public record CounterActionChoicesEnum
    {
        public required string Name { get; set; }
        public RolesEnum Role { get; set; }

        public static readonly CounterActionChoicesEnum COUNTESS = new() 
        { 
            Name = "BLOCK_ASSASSINATE",
            Role = RolesEnum.Countess
        };
    }
}
