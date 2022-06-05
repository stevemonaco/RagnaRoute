using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagnaRoute.Data;
public class MonsterStore
{
    public IReadOnlyCollection<MonsterModel> Monsters { get; }

    private MonsterStore(List<MonsterModel> monsters)
    {
        Monsters = monsters;
    }

    public static MonsterStore LoadMonstersFromCsv(string mobCsvPath)
    {
        using var streamReader = new StreamReader(mobCsvPath);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            PrepareHeaderForMatch = args => args.Header.ToLower()
        };
        using var reader = new CsvReader(streamReader, config);

        var monsters = reader.GetRecords<MonsterModel>().ToList();

        return new MonsterStore(monsters);
    }

    public static async Task<MonsterStore> LoadMonstersFromCsvAsync(string mobCsvPath)
    {
        using var streamReader = new StreamReader(mobCsvPath, new FileStreamOptions() { Options = FileOptions.Asynchronous});

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            PrepareHeaderForMatch = args => args.Header.ToLower()
        };
        using var reader = new CsvReader(streamReader, config);

        var monsters = await reader.GetRecordsAsync<MonsterModel>().ToListAsync();

        return new MonsterStore(monsters);
    }
}
