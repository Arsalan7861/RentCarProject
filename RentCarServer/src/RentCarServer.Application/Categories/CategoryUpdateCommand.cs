using FluentValidation;
using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Categories;
using RentCarServer.Domain.Shared;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Categories;
[Permission("category:edit")]
public sealed record CategoryUpdateCommand(
    Guid Id,
    string Name,
    bool IsActive) : IRequest<Result<string>>;

internal sealed class CategoryUpdateCommandValidator : AbstractValidator<CategoryUpdateCommand>
{
    public CategoryUpdateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Geçerli bir kategori adı girin.");
    }
}

internal sealed class CategoryUpdateCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CategoryUpdateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CategoryUpdateCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (category is null)
        {
            return Result<string>.Failure("Kategori bulunamadı.");
        }

        // Aynı isimde başka bir kategori var mı kontrol et (güncellenen hariç)
        var nameExists = await categoryRepository.AnyAsync(
            x => x.Name.Value == request.Name && x.Id != request.Id,
            cancellationToken);

        if (nameExists)
        {
            return Result<string>.Failure("Bu isimde bir kategori zaten mevcut.");
        }

        Name name = new(request.Name);
        category.SetName(name);
        category.SetStatus(request.IsActive);
        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return "Kategori bilgisi başarıyla güncellendi.";
    }
}