using FluentValidation.Results;
using System.Security.Cryptography.X509Certificates;

namespace BuildingBlocks.CommandErrors
{
// Usamos ValidationFailure de la libreria FluentValidation
	public record ValidationFailed(IEnumerable<ValidationFailure> Errors)
	{
		public ValidationFailed(ValidationFailure error) : this([error]) { }
	}
}
