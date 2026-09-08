namespace GestionComplejo2C.Domain.Entities
{
    public class Reserva
    {
        public Guid Id { get; private set; }
        public Guid CanchaId { get; private set; }
        public string Cliente { get; private set; }
        public DateTime Inicio { get; private set; }
        public int Horas { get; private set; }
        public decimal Importe { get; private set; }
        public bool Cancelada { get; private set; }

        public DateTime Fin => Inicio.AddHours(Horas);

        private Reserva()
        {
            Cliente = string.Empty;
        }

        public Reserva(string cliente, DateTime inicio, int horas, decimal importe)
        {
            Id = Guid.NewGuid();
            Cliente = cliente;
            Inicio = inicio;
            Horas = horas;
            Importe = importe;
            Cancelada = false;
        }

        public void Cancelar() => Cancelada = true;

        public override string ToString() =>
            $"{Inicio:dd/MM HH:mm}-{Fin:HH:mm} | {Cliente} | ${Importe}{(Cancelada ? " | CANCELADA" : "")}";
    }
}
