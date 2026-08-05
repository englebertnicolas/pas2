using System.Globalization;
using Microsoft.Extensions.Hosting;

namespace PAS.AspNetCore.Configuration;

public static class CultureExtensions {

    public static IHostApplicationBuilder SetDefaultCulture(this IHostApplicationBuilder builder) {
        var cultureInfo = (CultureInfo)new CultureInfo("en-US", false).Clone();
        cultureInfo.NumberFormat.CurrencySymbol = "€";
        cultureInfo.NumberFormat.CurrencyDecimalSeparator = ",";
        cultureInfo.NumberFormat.CurrencyGroupSeparator = " ";
        cultureInfo.NumberFormat.NumberDecimalSeparator = ",";
        cultureInfo.NumberFormat.NumberGroupSeparator = " ";
        cultureInfo.NumberFormat.PercentDecimalSeparator = ",";
        cultureInfo.NumberFormat.PercentGroupSeparator = " ";
        cultureInfo.DateTimeFormat.ShortTimePattern = "HH:mm";
        cultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss";
        cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        cultureInfo.DateTimeFormat.LongDatePattern = "dddd d MMMM yyyy";
        cultureInfo.DateTimeFormat.MonthDayPattern = "d MMMM";

        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        return builder;
    }
}
