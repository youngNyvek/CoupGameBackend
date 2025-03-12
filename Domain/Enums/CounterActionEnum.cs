namespace Domain.Enums
{
    public record CounterActionEnum : ActionsEnum
    {
        public static readonly CounterActionEnum COUNTESS = new() 
        { 
            Name = "BLOCK_ASSASSINATE",
            Role = RolesEnum.Countess
        };
    }
}
