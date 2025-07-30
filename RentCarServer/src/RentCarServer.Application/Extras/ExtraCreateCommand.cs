using FluentValidation;
using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Extras;
using RentCarServer.Domain.Shared;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Extras;
[Permission("extra:create")]
public sealed record ExtraCreateCommand(
    string Name,
    decimal Price,
    string Description,
    bool IsActive) : IRequest<Result<string>>;

internal sealed class ExtraCreateCommandValidator : AbstractValidator<ExtraCreateCommand>
{
    public ExtraCreateCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Geçerli bir ekstra ad? girin.");
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Fiyat negatif olamaz.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Aç?klama zorunludur.");
    }
}

internal sealed class ExtraCreateCommandHandler(
    IExtraRepository extraRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ExtraCreateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ExtraCreateCommand request, CancellationToken cancellationToken)
    {
        var nameExists = await extraRepository.AnyAsync(x => x.Name.Value == request.Name, cancellationToken);
        if (nameExists)
            return Result<string>.Failure("Bu isimde bir ekstra zaten mevcut.");

        Name name = new(request.Name);
        Price price = new(request.Price);
        Description description = new(request.Description);

        var extra = new Extra(name, price, description, request.IsActive);

        await extraRepository.AddAsync(extra);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return "Ekstra ba?ar?yla olu?turuldu.";
    }
}