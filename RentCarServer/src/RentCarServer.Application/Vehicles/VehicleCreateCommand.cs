using FluentValidation;
using GenericFileService.Files;
using GenericRepository;
using Microsoft.AspNetCore.Http;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Shared;
using RentCarServer.Domain.Vehicles;
using RentCarServer.Domain.Vehicles.ValueObjects;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Vehicles;
[Permission("vehicle:create")]
public sealed record VehicleCreateCommand(
    string Brand,
    string Model,
    int ModelYear,
    string Color,
    string Plate,
    Guid CategoryId,
    Guid BranchId,
    string VinNumber,
    string EngineNumber,
    string Description,
    string FuelType,
    string Transmission,
    decimal EngineVolume,
    int EnginePower,
    string TractionType,
    decimal FuelConsumption,
    int SeatCount,
    int Kilometer,
    decimal DailyPrice,
    decimal WeeklyDiscountRate,
    decimal MonthlyDiscountRate,
    string InsuranceType,
    DateOnly LastMaintenanceDate,
    int LastMaintenanceKm,
    int NextMaintenanceKm,
    DateOnly InspectionDate,
    DateOnly InsuranceEndDate,
    DateOnly? CascoEndDate,
    string TireStatus,
    string GeneralStatus,
    List<string> Features,
    bool IsActive,
    IFormFile File
) : IRequest<Result<string>>;

internal sealed class VehicleCreateCommandValidator : AbstractValidator<VehicleCreateCommand>
{
    public VehicleCreateCommandValidator()
    {
        RuleFor(x => x.Brand).NotEmpty().WithMessage("Lütfen araç markasını giriniz.");
        RuleFor(x => x.Model).NotEmpty().WithMessage("Lütfen araç modelini giriniz.");
        RuleFor(x => x.ModelYear).GreaterThan(1900).WithMessage("Lütfen geçerli bir model yılı giriniz. 1900'den büyük olmal?");
        RuleFor(x => x.Color).NotEmpty().WithMessage("Lütfen araç rengini giriniz.");
        RuleFor(x => x.Plate).NotEmpty().WithMessage("Lütfen araç plakasını giriniz.");
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Lütfen araç kategorisini seçiniz.");
        RuleFor(x => x.BranchId).NotEmpty().WithMessage("Lütfen araç şubesini seçiniz.");
        RuleFor(x => x.File).NotNull().WithMessage("Araç resmi zorunludur.");
        RuleFor(x => x.Features)
            .Must(features => features.Count > 0)
            .WithMessage("Lütfen en az bir özellik ekleyiniz.");
    }
}

internal sealed class VehicleCreateCommandHandler(
    IVehicleRepository vehicleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<VehicleCreateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(VehicleCreateCommand request, CancellationToken cancellationToken)
    {
        // Plaka benzersizli?i kontrolü
        bool plateExists = await vehicleRepository.AnyAsync(
            x => x.Plate.Value == request.Plate, cancellationToken);
        if (plateExists)
            return Result<string>.Failure("Bu plakaya sahip bir araç zaten mevcut.");

        // Resmi kaydet
        string fileName = FileService.FileSaveToServer(request.File, "wwwroot/images/");
        ImageUrl imageUrl = new(fileName);

        var brand = new Brand(request.Brand);
        var model = new Model(request.Model);
        var modelYear = new ModelYear(request.ModelYear);
        var color = new Color(request.Color);
        var plate = new Plate(request.Plate);
        var categoryId = new IdentityId(request.CategoryId);
        var branchId = new IdentityId(request.BranchId);
        var vinNumber = new VinNumber(request.VinNumber);
        var engineNumber = new EngineNumber(request.EngineNumber);
        var description = new Description(request.Description);
        var fuelType = new FuelType(request.FuelType);
        var transmission = new Transmission(request.Transmission);
        var engineVolume = new EngineVolume(request.EngineVolume);
        var enginePower = new EnginePower(request.EnginePower);
        var tractionType = new TractionType(request.TractionType);
        var fuelConsumption = new FuelConsumption(request.FuelConsumption);
        var seatCount = new SeatCount(request.SeatCount);
        var kilometer = new Kilometer(request.Kilometer);
        var dailyPrice = new DailyPrice(request.DailyPrice);
        var weeklyDiscountRate = new WeeklyDiscountRate(request.WeeklyDiscountRate);
        var monthlyDiscountRate = new MonthlyDiscountRate(request.MonthlyDiscountRate);
        var insuranceType = new InsuranceType(request.InsuranceType);
        var lastMaintenanceDate = new LastMaintenanceDate(request.LastMaintenanceDate);
        var lastMaintenanceKm = new LastMaintenanceKm(request.LastMaintenanceKm);
        var nextMaintenanceKm = new NextMaintenanceKm(request.NextMaintenanceKm);
        var inspectionDate = new InspectionDate(request.InspectionDate);
        var insuranceEndDate = new InsuranceEndDate(request.InsuranceEndDate);
        var cascoEndDate = request.CascoEndDate is not null ? new CascoEndDate(request.CascoEndDate.Value) : null;
        var tireStatus = new TireStatus(request.TireStatus);
        var generalStatus = new GeneralStatus(request.GeneralStatus);
        var features = request.Features.Select(f => new Feature(f)).ToList();

        var vehicle = new Vehicle(
            brand,
            model,
            modelYear,
            color,
            plate,
            categoryId,
            branchId,
            vinNumber,
            engineNumber,
            description,
            imageUrl,
            fuelType,
            transmission,
            engineVolume,
            enginePower,
            tractionType,
            fuelConsumption,
            seatCount,
            kilometer,
            dailyPrice,
            weeklyDiscountRate,
            monthlyDiscountRate,
            insuranceType,
            lastMaintenanceDate,
            lastMaintenanceKm,
            nextMaintenanceKm,
            inspectionDate,
            insuranceEndDate,
            cascoEndDate,
            tireStatus,
            generalStatus,
            features,
            request.IsActive
        );

        await vehicleRepository.AddAsync(vehicle, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return "Araç ba?ar?yla olu?turuldu.";
    }
}