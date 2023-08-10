using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace MrCMS.Helpers;

public static class CsvHelper
{
    public static List<T> ReadCsv<T>(Stream fileStream, bool hasHeaderRecord = false, int skipCount = 0)
    {
        var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = hasHeaderRecord,
        };

        using var csvReader = new CsvReader(new StreamReader(fileStream), csvConfig);
        return csvReader.GetRecords<T>().Skip(skipCount).ToList();
    }
    
    public static async Task<byte[]> GenerateCsvBytes<T>(List<T> data, Encoding encoding, bool addHeaderRecord = true)
    {
        byte[] bytes = null;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Encoding = encoding ?? Encoding.UTF8,
            HasHeaderRecord = addHeaderRecord,
            TrimOptions = TrimOptions.Trim
        };

        var utf8WithoutBom = new UTF8Encoding(false);

        await using var ms = new MemoryStream();
        await using TextWriter tw = new StreamWriter(ms, utf8WithoutBom);
        await using (var csv = new CsvWriter(tw, config))
        {
            if (addHeaderRecord)
            {
                csv.WriteHeader<T>();
                await csv.NextRecordAsync();
            }

            await csv.WriteRecordsAsync(data);
        }

        bytes = ms.ToArray();

        return bytes;
    }
}