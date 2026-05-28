namespace HoraCerta.Aplicacao.Comunicacao.Ports;

public interface INormalizadorTelefone
{
    string Normalizar(string telefone);

    bool SaoEquivalentes(string telefoneA, string telefoneB);
}
