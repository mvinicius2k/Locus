using FluentValidation;

namespace Api.Helpers.ValitationExtensions;

public static  class ValidationUtils
{
    /// <summary>
    /// Valida se uma chave estrangeira existe invocanto o <paramref name="predicate"/>, 
    /// que geralmente é uma função de camada repository que dado um index (identificador), retorna uma entidade
    /// </summary>
    /// <typeparam name="TValidated">Tipo a ser validado</typeparam>
    /// <typeparam name="TEntity">Tipo da entidade</typeparam>
    /// <typeparam name="TIndex">Identificador</typeparam>
    /// <param name="ruleBuilder"></param>
    /// <param name="predicate">Função que recebe o identificador e retorna a entidade</param>
    public static IRuleBuilderOptions<TValidated, TIndex> IndexExists<TValidated, TEntity, TIndex>(this IRuleBuilder<TValidated, TIndex> ruleBuilder, Func<TIndex, ValueTask<TEntity?>> predicate)
    {
        return ruleBuilder.MustAsync(async (index, token) =>
        {
            var entity = await predicate.Invoke(index);
            return entity != null;
        });

    }
}
