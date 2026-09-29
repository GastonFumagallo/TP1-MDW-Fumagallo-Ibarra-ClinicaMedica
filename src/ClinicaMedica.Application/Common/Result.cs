namespace ClinicaMedica.Application.Common;

public enum TipoError
{
    Validacion = 1,
    NoEncontrado = 2,
    Conflicto = 3
}

public sealed record Error(string Codigo, string Descripcion, TipoError Tipo)
{
    public static readonly Error Ninguno = new(string.Empty, string.Empty, TipoError.Validacion);

    public static Error Validacion(string codigo, string descripcion) =>
        new(codigo, descripcion, TipoError.Validacion);

    public static Error NoEncontrado(string codigo, string descripcion) =>
        new(codigo, descripcion, TipoError.NoEncontrado);

    public static Error Conflicto(string codigo, string descripcion) =>
        new(codigo, descripcion, TipoError.Conflicto);
}

public class Result
{
    protected Result(bool esExitoso, Error error)
    {
        if (esExitoso && error != Error.Ninguno)
            throw new InvalidOperationException("Un resultado exitoso no puede contener un error.");
        if (!esExitoso && error == Error.Ninguno)
            throw new InvalidOperationException("Un resultado fallido debe contener un error.");

        EsExitoso = esExitoso;
        Error = error;
    }

    public bool EsExitoso { get; }
    public bool EsFallido => !EsExitoso;
    public Error Error { get; }

    public static Result Exito() => new(true, Error.Ninguno);
    public static Result Fallo(Error error) => new(false, error);

    public static Result<T> Exito<T>(T valor) => new(valor, true, Error.Ninguno);
    public static Result<T> Fallo<T>(Error error) => new(default, false, error);
}

public class Result<T> : Result
{
    private readonly T? _valor;

    internal Result(T? valor, bool esExitoso, Error error) : base(esExitoso, error)
    {
        _valor = valor;
    }

    public T Valor => EsExitoso
        ? _valor!
        : throw new InvalidOperationException("No se puede acceder al valor de un resultado fallido.");
}
