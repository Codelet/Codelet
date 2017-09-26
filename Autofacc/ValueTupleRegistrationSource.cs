namespace Codelet.Autofacc
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Immutable;
  using System.Linq;
  using global::Autofac;
  using global::Autofac.Builder;
  using global::Autofac.Core;

  /// <summary>
  /// Autofac registration source to resolve generic versions of <see cref="ValueTuple"/>.
  /// </summary>
  public class ValueTupleRegistrationSource : IRegistrationSource
  {
    /// <inheritdoc />
    public bool IsAdapterForIndividualComponents => false;

    private static ImmutableArray<Type> ValueTupleTypes { get; }
      = ImmutableArray.Create(
        typeof(ValueTuple<>),
        typeof(ValueTuple<,>),
        typeof(ValueTuple<,,>),
        typeof(ValueTuple<,,,>),
        typeof(ValueTuple<,,,,>),
        typeof(ValueTuple<,,,,,>),
        typeof(ValueTuple<,,,,,,>),
        typeof(ValueTuple<,,,,,,,>));

    /// <inheritdoc />
    public IEnumerable<IComponentRegistration> RegistrationsFor(
      Service service,
      Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
    {
      var tupleType = (service as IServiceWithType)?.ServiceType;

      if (!(tupleType?.IsGenericType ?? false) || !ValueTupleTypes.Contains(tupleType.GetGenericTypeDefinition()))
      {
        yield break;
      }

      yield return RegistrationBuilder
        .ForDelegate((context, parameters) => CreateTuple(tupleType, context, parameters))
        .As(service)
        .CreateRegistration();
    }

    private static object CreateTuple(Type tupleType, IComponentContext context, IEnumerable<Parameter> parameters)
    {
      var arguments = tupleType
        .GenericTypeArguments
        .Select(argumentType => context.ResolveOptional(argumentType, parameters))
        .ToArray();

      return tupleType
        .GetConstructor(tupleType.GenericTypeArguments)
        ?.Invoke(arguments);
    }
  }
}