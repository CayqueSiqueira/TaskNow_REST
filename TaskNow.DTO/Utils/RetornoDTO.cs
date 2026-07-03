namespace TaskNow.DTO.Utils;

public class RetornoDTO<T>
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public T? Dados { get; set; }

    public static RetornoDTO<T> Ok(T dados, string? mensagem = null) => new()
    {
        Sucesso = true,
        Mensagem = mensagem,
        Dados = dados
    };

    public static RetornoDTO<T> Fail(string mensagem) => new()
    {
        Sucesso = false,
        Mensagem = mensagem,
        Dados = default
    };
}