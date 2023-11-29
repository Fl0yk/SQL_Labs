namespace Crematorium.Domain.Entities
{
    public class Order : Base
    {
        public User Customer { get; set; }
        //public Date? RegistrationDate { get; set; }

        public StateOrder State { get; set; } = StateOrder.Decorated;

        public DateTime DateOfActual { get; set; }

        public Corpose CorposeId { get; set; } = null!;

        public RitualUrn? RitualUrnId { get; set; }

        public Hall? HallId { get; set; }

        public Coffin? CoffinId { get; set; }

        public List<Ceremony> Ceremonies { get; set; } = new();
    }

    public enum StateOrder
    {
        None,
        Decorated,  //оформленный
        Approved,   //подтвержденный
        Closed,     //закрытый
        Cancelled   //отмененный
    }
}
