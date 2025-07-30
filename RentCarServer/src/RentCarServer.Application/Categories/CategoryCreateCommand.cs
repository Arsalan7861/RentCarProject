using FluentValidation;
using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Categories;
using RentCarServer.Domain.Shared;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Categories;
[Permission("category:create")]
public sealed record CategoryCreateCommand(
    string Name,
    bool IsActive) : IRequest<Result<string>>;

internal sealed class CategoryCreateCommandValidator : AbstractValidator<CategoryCreateCommand>
{
    public CategoryCreateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Geçerli bir kategori adı girin.");
    }
}

internal sealed class CategoryCreateCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CategoryCreateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CategoryCreateCommand request, CancellationToken cancellationToken)
    {
        var nameExists = await categoryRepository.AnyAsync(x => x.Name.Value == request.Name, cancellationToken);
        if (nameExists)
        {
            return Result<string>.Failure("Bu isimde bir kategori zaten mevcut.");
        }

        Name name = new(request.Name);
        var category = new Category(name, request.IsActive);
        await categoryRepository.AddAsync(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return "Kategori başarıyla oluşturuldu.";
    }
}