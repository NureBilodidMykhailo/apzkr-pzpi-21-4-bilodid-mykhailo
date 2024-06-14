namespace SmartTruckApi.Infrastructure.Services.Calculations;
public static class DistanceCalculator
{
    private static double ToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }

    public static double CalculateDistance(double startLat, double startLon, double endLat, double endLon)
    {
        const double EarthRadiusKm = 6371.0;

        double dLat = ToRadians(endLat - startLat);
        double dLon = ToRadians(endLon - startLon);

        double startLatRad = ToRadians(startLat);
        double endLatRad = ToRadians(endLat);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(startLatRad) * Math.Cos(endLatRad);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }
}