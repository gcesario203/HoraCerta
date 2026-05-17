using System.Text.Json;
using System.Text.Json.Serialization;

namespace HoraCerta.Infaestrutura.Persistencia.Serializacao;

public static class PersistenciaJsonOptions
{
    public static JsonSerializerOptions Criar()
        => new()
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
}
