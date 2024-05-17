using BuildingBlocks.CQRS;
using Catalog.API.Models;
using Marten;

namespace Catalog.API.Products.CreateProduct;

internal class CreateProductCommandHandler
    (IDocumentSession session, IValidator<CreateProductCommand> validator)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(command, cancellationToken);
        var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
        
        if (errors.Any())
        {
            throw new ValidationException(errors.FirstOrDefault());
        }

        var product = new Product
        {
            Name = command.Name,
            Category = command.Category,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price
        };

        session.Store(product);

        await session.SaveChangesAsync(cancellationToken);
        return new CreateProductResult(product.Id);
    }
}
