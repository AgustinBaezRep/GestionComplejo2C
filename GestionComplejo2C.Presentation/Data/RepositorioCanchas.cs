using GestionComplejo2C.Presentation.Models;

namespace GestionComplejo2C.Presentation.Data
{
    public static class RepositorioCanchas
    {
        private static readonly List<Cancha> canchas = new List<Cancha>();

        public static void Agregar(Cancha cancha) => canchas.Add(cancha);

        public static IReadOnlyList<Cancha> ObtenerTodas() => canchas.AsReadOnly();

        public static Cancha? ObtenerPorId(int id) => canchas.FirstOrDefault(c => c.Id == id);

        public static bool Eliminar(Cancha cancha) => canchas.Remove(cancha);
    }
}
