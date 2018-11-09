using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATAB3SO_WK4_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Needed for spatial types, for more information see:
            //  https://www.andrewcbancroft.com/2017/03/27/solving-spatial-types-and-functions-are-not-available-with-entity-framework/ 
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            SqlProviderServices.SqlServerTypesAssemblyName = "Microsoft.SqlServer.Types, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";

            //ComplexQuery();
            //NaiveReporting();
            NaiveReportingBetter();

            Console.ReadLine();
        }

        private static void ComplexQuery()
        {
            using (var context = new WideWorldImportersEntities())
            {
                var result =
                    context.OrderLines
                        .GroupBy(line => line.Order.Customer.City.StateProvince.StateProvinceName)
                        .Select(group => new { SpName = group.Key, Total = group.Sum(line => line.Quantity) })
                        .Join(context.OrderLines.GroupBy(line => new
                        {
                            Color = line.StockItem.Color,
                            SpName = line.Order.Customer.City.StateProvince.StateProvinceName
                        }), outer => outer.SpName, inner => inner.Key.SpName,
                        (outer, inner) => new
                        {
                            SpName = outer.SpName,
                            TotalInSp = outer.Total,
                            Quantity = inner.Sum(line => line.Quantity),
                            Color = inner.Key.Color,
                            Percentage = (inner.Sum(line => line.Quantity) + 0.0) / outer.Total
                        })
                        .OrderBy(val => val.SpName)
                        .ThenByDescending(val => val.Quantity);

                foreach (var item in result.ToList())
                {
                    Console.WriteLine($"{item.SpName}, {item.Color?.ColorName ?? "No Color"}: {item.Quantity} ({item.Percentage * 100:#,0.00}%)");
                }
            }
        }

        private static void NaiveReporting()
        {
            using (var context = new WideWorldImportersEntities())
            {
                List<string> results = new List<string>();
                foreach (var line in context.InvoiceLines.Take(50))
                {
                    var countryName = line.Invoice.Customer.City.StateProvince.Country.CountryName;
                    var stateProvinceName = line.Invoice.Customer.City.StateProvince.StateProvinceName;
                    var cityName = line.Invoice.Customer.City.CityName;
                    var packageTypeName = line.PackageType.PackageTypeName;
                    var quantity = line.Quantity;
                    var deliveryInstructions = line.Invoice.DeliveryInstructions;

                    results.Add($"{countryName}-{stateProvinceName}-{cityName}, {packageTypeName}: {quantity}, {deliveryInstructions}");
                }

                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
        }

        private static void NaiveReportingBetter()
        {
            using (var context = new WideWorldImportersEntities())
            {
                List<string> results = new List<string>();
                var invoiceLines = context.InvoiceLines
                                        .Include(il => il.Invoice)
                                        .Include(il => il.Invoice.Customer.City)
                                        .Include(il => il.Invoice.Customer.City.StateProvince)
                                        .Include(il => il.Invoice.Customer.City.StateProvince.Country)
                                        .Include(il => il.PackageType)
                                        .Take(50);

                foreach (var line in invoiceLines)
                {
                    var countryName = line.Invoice.Customer.City.StateProvince.Country.CountryName;
                    var stateProvinceName = line.Invoice.Customer.City.StateProvince.StateProvinceName;
                    var cityName = line.Invoice.Customer.City.CityName;
                    var packageTypeName = line.PackageType.PackageTypeName;
                    var quantity = line.Quantity;
                    var deliveryInstructions = line.Invoice.DeliveryInstructions;

                    results.Add($"{countryName}-{stateProvinceName}-{cityName}, {packageTypeName}: {quantity}, {deliveryInstructions}");
                }

                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
        }
    }
}
