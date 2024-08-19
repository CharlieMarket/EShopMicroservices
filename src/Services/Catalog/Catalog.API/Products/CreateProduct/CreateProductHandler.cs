using BuildingBlocks.CQRS;
using Catalog.API.Models;
using Marten;

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductCommand(
        string Name, string Description, List<string> Category, string ImageFile, decimal Price) 
        : ICommand<CreateProductResult> ;
    public record CreateProductResult(Guid Id);

    internal class CreateProductCommandHandler(IDocumentSession session) : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            // Business logic
            // Craate a model object
            // var product = command.Adapt<Product>();
            var product = new Product
            {
                Name = command.Name,
                Description = command.Description,
                Category = command.Category,
                ImageFile = command.ImageFile,
                Price = command.Price
            };
            // TODO: Save the object
            session.Store(product);

			await session.SaveChangesAsync(cancellationToken);
            
            return new CreateProductResult(product.Id);
        }
    }
}
