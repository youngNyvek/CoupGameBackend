namespace Domain.Enums
{
    public record ActionsEnum
    {
        public required string Name { get; set; }
        public RolesEnum Role { get; set; }
    }
}
