namespace GestionComplejo2C.Presentation.Models
{
    public class Reserva
    {
        public Guid Id { get; }
        public string Cliente { get; }
        public DateTime Inicio { get; }
        public int Horas { get; }
        public decimal Importe { get; }
        public bool Cancelada { get; private set; }

        public DateTime Fin => Inicio.AddHours(Horas);

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
