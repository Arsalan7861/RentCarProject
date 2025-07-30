using FluentValidation;
using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Extras;
using RentCarServer.Domain.Shared;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Extras;
[Permission("extra:edit")]
public sealed record ExtraUpdateCommand(
    Guid Id,
    string Name,
    decimal Price,
    string Description,
    bool IsActive) : IRequest<Result<string>>;

internal sealed class ExtraUpdateCommandValidator : AbstractValidator<ExtraUpdateCommand>
{
    public ExtraUpdateCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Geçerli bir ekstra ad? girin.");
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Fiyat negatif olamaz.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Aç?klama zorunludur.");
    }
}

internal sealed class ExtraUpdateCommandHandler(
    IExtraRepository extraRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ExtraUpdateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ExtraUpdateCommand request, CancellationToken cancellationToken)
    {
        var extra = await extraRepository.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (extra is null)
            return Result<string>.Failure("Ekstra bulunamad?.");

        var nameExists = await extraRepository.AnyAsync(
            x => x.Name.Value == request.Name && x.Id != request.Id, cancellationToken);
        if (nameExists)
            return Result<string>.Failure("Bu isimde bir ekstra zaten mevcut.");

        Name name = new(request.Name);
        Price price = new(request.Price);
        Description description = new(request.Description);

        extra.SetName(name);
        extra.SetPrice(price);
        extra.SetDescription(description);
        extra.SetStatus(request.IsActive);

        extraRepository.Update(extra);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return "Ekstra ba?ar?yla güncellendi.";
    }
}