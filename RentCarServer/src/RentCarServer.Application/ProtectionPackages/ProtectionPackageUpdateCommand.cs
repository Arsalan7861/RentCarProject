using FluentValidation;
using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.ProtectionPackages;
using RentCarServer.Domain.ProtectionPackages.ValueObjects;
using RentCarServer.Domain.Shared;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.ProtectionPackages;
[Permission("protection_package:edit")]
public sealed record ProtectionPackageUpdateCommand(
    Guid Id,
    string Name,
    decimal Price,
    bool IsRecommended,
    int OrderNumber,
    List<string> Coverages,
    bool IsActive) : IRequest<Result<string>>;

public sealed class ProtectionPackageUpdateCommandValidator : AbstractValidator<ProtectionPackageUpdateCommand>
{
    public ProtectionPackageUpdateCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("Ge�erli bir paket adı girin");
        RuleFor(p => p.Price).GreaterThan(-1).WithMessage("Fiyat pozitif olmalı");
    }
}

internal sealed class ProtectionPackageUpdateCommandHandler(
    IProtectionPackageRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<ProtectionPackageUpdateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ProtectionPackageUpdateCommand request, CancellationToken cancellationToken)
    {
        var package = await repository.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (package is null)
            return Result<string>.Failure("Güvence paketi bulunamadı");

        if (!string.Equals(package.Name.Value, request.Name, StringComparison.OrdinalIgnoreCase))
        {
            var nameExists = await repository.AnyAsync(
                p => p.Name.Value == request.Name && p.Id != request.Id,
                cancellationToken);

            if (nameExists)
                return Result<string>.Failure("Paket adı daha önce tanımlanmış");
        }

        //if (package.OrderNumber.Value != request.OrderNumber)
        //{
        //    var packages = await repository
        //        .WhereWithTracking(p => p.Id != package.Id)
        //        .OrderBy(i => i.OrderNumber.Value)
        //        .ToListAsync(cancellationToken);

        //    packages.Insert(request.OrderNumber - 1, package);

        //    foreach (var (item, index) in packages.Select((item, index) => (item, index)))
        //    {
        //        item.SetOrderNumber(new(index + 1));
        //    }
        //}

        Name name = new(request.Name);
        Price price = new(request.Price);
        IsRecommended isRecommended = new(request.IsRecommended);
        OrderNumber orderNumber = new(request.OrderNumber);
        List<Coverage> coverages = request.Coverages.Select(c => new Coverage(c)).ToList();

        package.SetName(name);
        package.SetPrice(price);
        package.SetIsRecommended(isRecommended);
        package.SetOrderNumber(orderNumber);
        package.SetCoverages(coverages);
        package.SetStatus(request.IsActive);

        repository.Update(package);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Güvence paketi başarıyla güncellendi";
    }
}