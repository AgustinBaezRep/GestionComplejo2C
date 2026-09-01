using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;

namespace GestionComplejo2C.Infrastructure.Repositories
{
    public class RepositorioCanchas : IRepositorioCanchas
    {
        private readonly List<Cancha> canchas = new List<Cancha>();

        public void Agregar(Cancha cancha) => canchas.Add(cancha);

        public IReadOnlyList<Cancha> ObtenerTodas() => canchas.AsReadOnly();

        public Cancha? ObtenerPorId(int id) => canchas.FirstOrDefault(c => c.Id == id);

        public bool Eliminar(Cancha cancha) => canchas.Remove(cancha);
    }
}
