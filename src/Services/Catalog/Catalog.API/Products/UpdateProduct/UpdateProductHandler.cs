using BuildingBlocks.CommandErrors;
using Marten;
using OneOf;
using OneOf.Types;

namespace Catalog.API.Products.UpdateProduct
{ 
	public record UpdateProductCommand(
	   Guid Id, string Name, string Description, List<string> Category, string ImageFile, decimal Price)
	   : ICommand<UpdateProductResult>;
	public record UpdateProductResult(OneOf<OneOf.Types.Success, RecordNotFound, ValidationFailed> Result);

	internal class UpdateProductCommandHandler
		(IDocumentSession session) 
		: IRequestHandler<UpdateProductCommand, UpdateProductResult>
	{
		public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
		{
			var product = await session.LoadAsync<Product>(command.Id, cancellationToken);
			if (product is null)
				return new UpdateProductResult(new RecordNotFound(command.Id));

			product = command.Adapt<Product>();
			session.Update(product);
			await session.SaveChangesAsync(cancellationToken);

			return new UpdateProductResult(new Success());
		}

	}

}
